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
			QuestClear
		}

		[SerializeField]
		private ingame.IngameWorld m_world = null;

		[SerializeField]
		private ingame.IngameApp m_app = null;

		[SerializeField]
		private Transform[] m_movieCameraAngleTransforms = null;



		private UnityAction<string, UnityAction> m_parentIngameEvent;

		private Transform m_cameraTransform;

		private Vector3 m_cameraDragEuler = Vector3.zero;

		private Vector3 m_cameraEuler = Vector3.zero;

		private float m_cameraDistance = 0.0f;

		private Vector3 m_cameraPositionOffset = Vector3.zero;



		public void Initialize(
			UnityAction<int, ingame.StageScene, UnityAction<ingame.StageScene>> changeMapEvent,
			UnityAction<string, UnityAction> ingameEvent,
			UnityAction<ingame.IngameWorld.SearchInData> updateCharaActionButtonEvent,
			UnityAction callback)
		{
			m_parentIngameEvent = ingameEvent;
			StartCoroutine(InitializeCoroutine(changeMapEvent, updateCharaActionButtonEvent, callback));
		}

		private IEnumerator InitializeCoroutine(
			UnityAction<int, ingame.StageScene, UnityAction<ingame.StageScene>> changeMapEvent,
			UnityAction<ingame.IngameWorld.SearchInData> updateCharaActionButtonEvent,
			UnityAction callback)
		{
			bool isDone = false;

			m_world.Initialize(
				changeMapEvent,
				IngameEvent,
				updateCharaActionButtonEvent,
				() => { isDone = true; });
			while (!isDone) { yield return null; }
			m_app.Initialize();

			var camera = GeneralRoot.Instance.GetIngame2Camera();
			m_cameraEuler = new Vector3(GetCameraEulerX(), 0.0f, 0.0f);
			m_cameraTransform = camera.transform;
			m_cameraDistance = GetCameraDistance(ZoomType.Normal);
			m_cameraPositionOffset = GetCameraPositionOffset(ZoomType.Normal);

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

		private void IngameEvent(string eventParam, UnityAction callback)
		{
			string[] eventType = eventParam.Split('_');
			switch (eventType[0])
			{
				case "SearchIn":
					{
						int controllId = int.Parse(eventType[1]);
						SearchIn(controllId, callback);
						break;
					}
				case "SearchOut":
					{
						int controllId = int.Parse(eventType[1]);
						SearchOut(controllId, callback);
						break;
					}
				default:
					{
						m_parentIngameEvent(eventParam, callback);
						break;
					}
			}
		}

		public void ChangeMap(int stageId, int dataIndex, UnityAction callback)
		{
			StartCoroutine(ChangeMapCoroutine(stageId, dataIndex, callback));
		}

		private IEnumerator ChangeMapCoroutine(int stageId, int dataIndex, UnityAction callback)
		{
			yield return m_world.ChangeMapOutCoroutine();
			yield return UpdateMovieCameraCoroutine(ZoomType.Out, 0.5f, null);

			yield return m_world.ChangeMapCoroutine(stageId, dataIndex);

			yield return m_world.ChangeMapInCoroutine();
			yield return UpdateMovieCameraCoroutine(ZoomType.Normal, 0.5f, null);

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

		private void SearchIn(int controllId, UnityAction callback)
		{
			m_world.SearchIn(controllId);
			if (callback != null)
			{
				callback();
			}
		}

		private void SearchOut(int controllId, UnityAction callback)
		{
			m_world.SearchOut(controllId);
			if (callback != null)
			{
				callback();
			}
		}

		public void RemoveTarget(int controllId)
		{
			m_world.RemoveTarget(controllId);
		}

		public void OnCharaActionButtonPressed(int controllId, UnityAction callback)
		{
			m_world.OnCharaActionButtonPressed(controllId, callback);
		}

		public EnemyDataAsset.Data.ColorData GetColorData(int enemyId, int controllId)
		{
			return m_world.GetColorData(enemyId, controllId);
		}

		private Vector3 GetCameraPosition()
		{
			float eulerY = m_cameraEuler.y + m_cameraDragEuler.y;
			float rad = eulerY * Mathf.Deg2Rad;
			float x = Mathf.Cos(rad) * m_cameraDistance;
			float z = Mathf.Sin(rad) * m_cameraDistance;
			Vector3 v =
				new Vector3(x, 0.0f, z) +
				m_cameraPositionOffset +
				m_world.GetPlayerPosition();
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


		public void PlayMovieCharaReaction(
			ingame.IngameWorld.ReactionType type,
			int enemyControllId,
			UnityAction callback)
		{
			m_world.PlayMovieCharaReaction(type, enemyControllId, callback);
		}

		public void PlayMovieCamera(
			ZoomType type,
			float time,
			UnityAction callback)
		{
			StartCoroutine(UpdateMovieCameraCoroutine(type, time, callback));
		}

		private IEnumerator UpdateMovieCameraCoroutine(
			ZoomType type,
			float time,
			UnityAction callback)
		{
			AnimationCurve curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

			Vector3 beforeEuler = m_cameraEuler;
			float eulerX = GetCameraEulerX();
			float eulerY = m_cameraEuler.y;
			Vector3 afterEuler = new Vector3(eulerX, eulerY, 0.0f);
			float beforeDistance = m_cameraDistance;
			float afterDistance = GetCameraDistance(type);
			Vector3 beforePositionOffset = m_cameraPositionOffset;
			Vector3 afterPositionOffset = GetCameraPositionOffset(type);

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
				m_cameraPositionOffset = Vector3.Lerp(beforePositionOffset, afterPositionOffset, value);

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
				case ZoomType.QuestClear:
					{
						return 10.0f;
					}
				default:
					{
						return 20.0f;
					}
			}
		}

		private Vector3 GetCameraPositionOffset(ZoomType type)
		{
			switch (type)
			{
				case ZoomType.Normal:
					{
						return new Vector3(0.0f, 12.0f, 0.0f);
					}
				case ZoomType.In:
					{
						return new Vector3(0.0f, 6.0f, 0.0f);
					}
				case ZoomType.Out:
					{
						return new Vector3(0.0f, 18.0f, 0.0f);
					}
				case ZoomType.QuestClear:
					{
						Vector3 euler = new Vector3(0.0f, m_cameraTransform.eulerAngles.y, 0.0f);
						Vector3 addOffset = Quaternion.Euler(euler) * new Vector3(2.0f, 0.0f, 0.0f);
						return new Vector3(0.0f, 6.0f, 0.0f) + addOffset;
					}
				default:
					{
						return new Vector3(0.0f, 12.0f, 0.0f);
					}
			}
		}

		private float GetCameraEulerX()
		{
			return 20.0f;
		}
	}
}