using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game
{
	[System.Serializable]
	public class Ingame : MonoBehaviour
	{
		public enum ZoomType
		{
			Normal,
			In,
			Out,
		}

		[SerializeField]
		private ingame.IngameWorld m_world = null;

		[SerializeField]
		private ingame.IngameApp m_app = null;

		[SerializeField]
		private Transform[] m_movieCameraAngleTransforms = null;



		private Transform m_cameraTransform = null;

		private Vector3 m_cameraDragEuler = Vector3.zero;

		private Vector3 m_cameraEuler = Vector3.zero;

		private float m_cameraDistance = 0.0f;

		private float m_cameraHeight = 0.0f;



		public void Initialize(
			UnityAction<int, ingame.StageScene, UnityAction<ingame.StageScene>> changeMapEvent,
			UnityAction<string, UnityAction> ingameEvent,
			UnityAction<ingame.IngameWorld.SearchInData> updateCharaActionButtonEvent,
			UnityAction callback)
		{
			StartCoroutine(InitializeCoroutine(changeMapEvent, ingameEvent, updateCharaActionButtonEvent, callback));
		}

		private IEnumerator InitializeCoroutine(
			UnityAction<int, ingame.StageScene, UnityAction<ingame.StageScene>> changeMapEvent,
			UnityAction<string, UnityAction> ingameEvent,
			UnityAction<ingame.IngameWorld.SearchInData> updateCharaActionButtonEvent,
			UnityAction callback)
		{
			bool isDone = false;

			m_world.Initialize(
				changeMapEvent,
				ingameEvent,
				updateCharaActionButtonEvent,
				() => { isDone = true; });
			while (!isDone) { yield return null; }
			m_app.Initialize();

			var camera = GeneralRoot.Instance.GetIngame2Camera();
			m_cameraEuler = new Vector3(GetCameraEulerX(), 0.0f, 0.0f);
			m_cameraTransform = camera.transform;
			m_cameraDistance = GetCameraDistance(ZoomType.Normal);
			m_cameraHeight = GetCameraHeight(ZoomType.Normal);

			if (callback != null)
			{
				callback();
			}
		}

		private void FixedUpdate()
		{
			if (m_cameraTransform == null)
			{
				return;
			}
			m_cameraTransform.SetPositionAndRotation(
				GetCameraPosition(),
				Quaternion.Euler(GetCameraEuler()));
		}

		public IEnumerator ChangeMapCoroutine(int stageId, int dataIndex, UnityAction callback)
		{
			yield return m_world.ChangeMapOutCoroutine();
			yield return UpdateMovieCameraCoroutine(ZoomType.Out, 0.5f, false, null);

			yield return m_world.ChangeMapCoroutine(stageId, dataIndex);

			m_cameraTransform.SetPositionAndRotation(
				GetCameraPosition(),
				Quaternion.Euler(GetCameraEuler()));

			yield return m_world.ChangeMapInCoroutine();
			yield return UpdateMovieCameraCoroutine(ZoomType.Normal, 0.5f, false, null);

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

		public void OnCharaActionButtonPressed(int controllId, UnityAction callback)
		{
			m_world.OnCharaActionButtonPressed(controllId, callback);
		}

		public void SetIsEventLock(bool value)
		{
			m_world.SetIsEventLock(value);
		}

		public EnemyDataAsset.Data.ColorData GetColorData(int enemyId, int controllId)
		{
			return m_world.GetColorData(enemyId, controllId);
		}

		public void PlayReaction(ingame.IngameWorld.ReactionType type, int enemyControllId, UnityAction callback)
		{
			m_world.PlayReaction(type, enemyControllId, callback);
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

		public void UpdateMovieCamera(
			ZoomType type,
			float time,
			bool isResetEulerY,
			UnityAction callback)
		{
			StartCoroutine(UpdateMovieCameraCoroutine(type, time, isResetEulerY, callback));
		}

		private IEnumerator UpdateMovieCameraCoroutine(
			ZoomType type,
			float time,
			bool isResetEulerY,
			UnityAction callback)
		{
			Vector3 beforeEuler = m_cameraEuler;
			Vector3 afterEuler = m_cameraEuler;
			if (isResetEulerY == true)
			{
				afterEuler = new Vector3(GetCameraEulerX(), m_cameraEuler.y, 0.0f);
			}

			AnimationCurve curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

			float beforeDistance = m_cameraDistance;
			float afterDistance = GetCameraDistance(type);
			float beforeHeight = m_cameraHeight;
			float afterHeight = GetCameraHeight(type);

			float nowTime = 0.0f;
			while (nowTime < time)
			{
				nowTime += Time.deltaTime;
				if (nowTime > time)
				{
					nowTime = time;
				}

				float t = nowTime / time;
				float value = curve.Evaluate(t);
				m_cameraEuler = Vector3.Lerp(beforeEuler, afterEuler, value);
				m_cameraDistance = Mathf.Lerp(beforeDistance, afterDistance, value);
				m_cameraHeight = Mathf.Lerp(beforeHeight, afterHeight, value);
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

		private float GetCameraDistance(ZoomType type)
		{
			switch (type)
			{
				case ZoomType.Normal:
					{
						return 20.0f;
					}
				case ZoomType.In:
					{
						return 10.0f;
					}
				case ZoomType.Out:
					{
						return 30.0f;
					}
				default:
					{
						return 20.0f;
					}
			}
		}

		private float GetCameraHeight(ZoomType type)
		{
			switch (type)
			{
				case ZoomType.Normal:
					{
						return 12.0f;
					}
				case ZoomType.In:
					{
						return 8.0f;
					}
				case ZoomType.Out:
					{
						return 16.0f;
					}
				default:
					{
						return 12.0f;
					}
			}
		}

		private float GetCameraEulerX()
		{
			return 20.0f;
		}
	}
}