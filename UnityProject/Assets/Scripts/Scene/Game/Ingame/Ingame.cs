using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game
{
	[System.Serializable]
	public class Ingame : MonoBehaviour
	{
		public enum AngleType
		{
			Back,
			Front,
		}

		[SerializeField]
		private ingame.IngameWorld m_world = null;

		[SerializeField]
		private ingame.IngameApp m_app = null;

		[SerializeField]
		private Transform[] m_movieCameraAngleTransforms = null;





		private Transform m_cameraTransform = null;

		private Vector3 m_cameraDragEuler = Vector3.zero;

		private Vector3 m_cameraEuler = new Vector3(20.0f, 0.0f, 0.0f);

		private float m_cameraDistance = 20.0f;

		private float m_cameraHeight = 12.0f;



		public void Initialize(
			UnityAction<int, UnityAction<ingame.StageScene>> loadStageEvent,
			UnityAction<string, UnityAction> ingameEvent,
			UnityAction<ingame.IngameWorld.SearchInData> updateCharaActionButtonEvent,
			UnityAction callback)
		{
			StartCoroutine(InitializeCoroutine(loadStageEvent, ingameEvent, updateCharaActionButtonEvent, callback));
		}

		private IEnumerator InitializeCoroutine(
			UnityAction<int, UnityAction<ingame.StageScene>> loadStageEvent,
			UnityAction<string, UnityAction> ingameEvent,
			UnityAction<ingame.IngameWorld.SearchInData> updateCharaActionButtonEvent,
			UnityAction callback)
		{
			bool isDone = false;
			m_world.Initialize(
				loadStageEvent,
				ingameEvent,
				updateCharaActionButtonEvent,
				() => { isDone = true; });
			while (!isDone) { yield return null; }
			m_app.Initialize();

			if (callback != null)
			{
				callback();
			}
		}

		public IEnumerator ChangeMapCoroutine(int stageId, int dataIndex, UnityAction callback)
		{
			yield return m_world.ChangeMapOutCoroutine();
			yield return m_world.LoadStage(stageId, dataIndex);

			var camera = GeneralRoot.Instance.GetIngame2Camera();
			m_cameraTransform = camera.transform;
			m_cameraTransform.SetPositionAndRotation(
				GetCameraPosition(),
				Quaternion.Euler(GetCameraEuler()));

			yield return m_world.ChangeMapInCoroutine();

			if (callback != null)
			{
				callback();
			}
		}

		public void UpdateMode(Game.GameMode mode)
		{
			//m_world.UpdateMode(mode);
			m_app.UpdateMode(mode);
		}

		public void UpdateApp()
		{
			//m_world.UpdateApp();
			m_app.UpdateApp();
		}

		public void OnCameraDragEvent(Vector2 dragVector)
		{
			float dragX = dragVector.y * 0.04f;
			float dragY = dragVector.x * 0.2f;
			if (m_cameraEuler.x + dragX > 30.0f)
			{
				dragX = 30.0f - m_cameraEuler.x;
			}
			else if (m_cameraEuler.x + dragX < 10.0f)
			{
				dragX = 10.0f - m_cameraEuler.x;
			}
			m_cameraDragEuler = new Vector3(dragX, dragY, 0.0f);

			m_cameraTransform.SetPositionAndRotation(
				GetCameraPosition(),
				Quaternion.Euler(GetCameraEuler()));
		}

		public void OnCameraEndDragEvent()
		{
			m_cameraEuler += m_cameraDragEuler;
			m_cameraDragEuler = Vector3.zero;
		}

		public void OnCameraClickEvent(Vector2 clickVector)
		{
		}

		public void OnPlayerDragEvent(Vector2 dragVector)
		{
			m_world.OnPlayerDragEvent(dragVector);

			m_cameraTransform.SetPositionAndRotation(
				GetCameraPosition(),
				Quaternion.Euler(GetCameraEuler()));
		}

		public void OnPlayerEndDragEvent()
		{
			m_world.OnPlayerEndDragEvent();
		}

		public void OnPlayerClickEvent(Vector2 clickVector)
		{
		}

		public IEnumerator DropItemCoroutine(int id)
		{
			yield return m_world.DropItemCoroutine(id);
		}

		public IEnumerator GetItemCoroutine(int id)
		{
			yield return m_world.GetItemCoroutine(id);
		}

		public IEnumerator ChangeMapInCoroutine()
		{
			yield return m_world.ChangeMapInCoroutine();
		}

		public IEnumerator ChangeMapOutCoroutine()
		{
			yield return m_world.ChangeMapOutCoroutine();
		}

		public void SearchIn(int controllId)
		{
			m_world.SearchIn(controllId);
		}

		public void SearchOut(int controllId)
		{
			m_world.SearchOut(controllId);
		}

		public void RemoveTarget(int controllId)
		{
			m_world.RemoveTarget(controllId);
		}

		public void OnCharaActionButtonPressed(int controllId)
		{
			m_world.OnCharaActionButtonPressed(controllId);
		}

		public void UpdateSearch()
		{
			m_world.UpdateSearch();
		}

		public EnemyDataAsset.Data.ColorData GetColorData(int enemyId, int controllId)
		{
			return m_world.GetColorData(enemyId, controllId);
		}

		public void PlayPlayerReaction(UnityAction callback)
		{
			m_world.PlayPlayerReaction(callback);
		}

		public void PlayEnemyReaction(int controllId, UnityAction callback)
		{
			m_world.PlayEnemyReaction(controllId, callback);
		}

		private Vector3 GetCameraPosition()
		{
			float eulerY = m_cameraEuler.y + m_cameraDragEuler.y;
			float rad = eulerY * Mathf.Deg2Rad;
			float x = Mathf.Cos(rad) * m_cameraDistance;
			float z = Mathf.Sin(rad) * m_cameraDistance;
			Vector3 v = new Vector3(x, m_cameraHeight, z) + m_world.GetPlayerPosition();
			return v;
		}

		private Vector3 GetCameraEuler()
		{
			float eulerX = (m_cameraEuler.x + m_cameraDragEuler.x);
			float eulerY = (m_cameraEuler.y + m_cameraDragEuler.y) * -1.0f + 270.0f;
			if (eulerY > 360.0f)
			{
				eulerY -= 360.0f;
			}
			else if (eulerY < 0.0f)
			{
				eulerY += 360.0f;
			}
			Vector3 v = new Vector3(eulerX, eulerY, 0.0f);
			return v;
		}

		public void UpdateMovieCamera(bool isZoom, UnityAction callback)
		{
			StartCoroutine(UpdateMovieCameraCoroutine(isZoom, callback));
		}

		private IEnumerator UpdateMovieCameraCoroutine(bool isZoom, UnityAction callback)
		{
			AnimationCurve curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

			float nowTime = 0.0f;
			float moveTime = 1.0f;
			while (nowTime < moveTime)
			{
				nowTime += Time.deltaTime;
				if (nowTime > moveTime)
				{
					nowTime = moveTime;
				}

				float t = nowTime / moveTime;
				float eva = curve.Evaluate(t);
				if (isZoom == true)
				{
					m_cameraDistance = 20.0f - (20.0f - 10.0f) * eva;
					m_cameraHeight = 12.0f - (12.0f - 8.0f) * eva;
				}
				else
				{
					m_cameraDistance = 10.0f - (10.0f - 20.0f) * eva;
					m_cameraHeight = 8.0f - (8.0f - 12.0f) * eva;
				}
				m_cameraTransform.SetPositionAndRotation(
					GetCameraPosition(),
					Quaternion.Euler(GetCameraEuler()));

				yield return null;
			}

			if (callback != null)
			{
				callback();
			}
		}
	}
}