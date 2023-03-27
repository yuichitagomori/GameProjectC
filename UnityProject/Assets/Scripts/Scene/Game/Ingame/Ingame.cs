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
			Approach,
			Pull,
			QuestClear,
			Customize,
		}

		[SerializeField]
		private ingame.IngameWorld m_world;

		[SerializeField]
		private ingame.IngameApp m_app;



		private UnityAction<string, UnityAction> m_parentIngameEvent;

		private ZoomType m_cameraZoomType = ZoomType.Normal;

		private Transform m_cameraParentTransform;

		private Common.AnimatorExpansion m_cameraAnimator;

		private Vector3 m_cameraDragEuler = Vector3.zero;

		private Vector3 m_cameraEuler = Vector3.zero;



		public void Initialize(
			UnityAction<string, UnityAction> ingameEvent,
			Transform ingameCameraParentTransform,
			Common.AnimatorExpansion ingameCameraAnimator,
			UnityAction<int, ingame.StageScene, UnityAction<ingame.StageScene>> loadMapEvent,
			UnityAction<ingame.IngameWorld.SearchInData> updateCharaActionButtonEvent,
			UnityAction callback)
		{
			m_parentIngameEvent = ingameEvent;
			m_cameraParentTransform = ingameCameraParentTransform;
			m_cameraAnimator = ingameCameraAnimator;

			StartCoroutine(InitializeCoroutine(
				loadMapEvent,
				updateCharaActionButtonEvent,
				callback));
		}

		private IEnumerator InitializeCoroutine(
			UnityAction<int, ingame.StageScene, UnityAction<ingame.StageScene>> loadMapEvent,
			UnityAction<ingame.IngameWorld.SearchInData> updateCharaActionButtonEvent,
			UnityAction callback)
		{
			m_cameraAnimator.Play("NormalAngle", null);

			bool isDone = false;
			m_world.Initialize(
				loadMapEvent,
				IngameEvent,
				m_cameraAnimator.transform,
				updateCharaActionButtonEvent,
				() => { isDone = true; });
			while (!isDone) { yield return null; }
			m_app.Initialize();

			m_cameraEuler = Vector3.zero;

			if (callback != null)
			{
				callback();
			}
		}

		private void FixedUpdate()
		{
			if (m_cameraParentTransform == null)
			{
				return;
			}
			m_cameraParentTransform.SetPositionAndRotation(
				m_world.GetPlayerPosition(),
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

		public void ChangeMap(int stageId, int dataIndex, bool isReady, UnityAction callback)
		{
			StartCoroutine(ChangeMapCoroutine(stageId, dataIndex, isReady, callback));
		}

		private IEnumerator ChangeMapCoroutine(int stageId, int dataIndex, bool isReady, UnityAction callback)
		{
			if (isReady == false)
			{
				yield return m_world.ChangeMapOutCoroutine(0.5f);
				yield return UpdateMovieCameraCoroutine(ZoomType.Pull, null);
			}

			yield return m_world.LoadMapCoroutine(stageId, dataIndex);

			if (isReady == false)
			{
				yield return m_world.ChangeMapInCoroutine(0.5f);
				yield return UpdateMovieCameraCoroutine(ZoomType.Normal, null);
			}
			else
			{
				yield return m_world.ChangeMapInCoroutine(0.0f);
			}

			if (callback != null)
			{
				callback();
			}
		}

		public void OnCameraDragEvent(Vector2 dragVector)
		{
			float dragX = dragVector.y * 0.04f;
			float dragY = dragVector.x * 0.2f;
			if (m_cameraEuler.x + dragX > 10.0f)
			{
				dragX = 10.0f - m_cameraEuler.x;
			}
			else if (m_cameraEuler.x + dragX < -10.0f)
			{
				dragX = -10.0f - m_cameraEuler.x;
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

		public void CheckEnemyActionEvent(int controllId)
		{
			m_world.CheckEnemyActionEvent(controllId);
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
			UnityAction callback)
		{
			StartCoroutine(UpdateMovieCameraCoroutine(type, callback));
		}

		private IEnumerator UpdateMovieCameraCoroutine(
			ZoomType type,
			UnityAction callback)
		{
			bool isDone = false;
			switch (type)
			{
				case ZoomType.Normal:
					{
						switch (m_cameraZoomType)
						{
							case ZoomType.Approach:
								{
									m_cameraAnimator.Play("ApproachToNormalAngle", () => { isDone = true; });
									break;
								}
							case ZoomType.Pull:
								{
									m_cameraAnimator.Play("PullToNormalAngle", () => { isDone = true; });
									break;
								}
							case ZoomType.QuestClear:
								{
									m_cameraAnimator.Play("QuestClearToNormalAngle", () => { isDone = true; });
									break;
								}
							case ZoomType.Customize:
								{
									m_cameraAnimator.Play("CustomizeToNormalAngle", () => { isDone = true; });
									break;
								}
							default:
								{
									isDone = true;
									break;
								}
						}
						break;
					}
				case ZoomType.Approach:
					{
						switch (m_cameraZoomType)
						{
							case ZoomType.Normal:
								{
									m_cameraAnimator.Play("NormalToApproachAngle", () => { isDone = true; });
									break;
								}
							default:
								{
									isDone = true;
									break;
								}
						}
						break;
					}
				case ZoomType.Pull:
					{
						switch (m_cameraZoomType)
						{
							case ZoomType.Normal:
								{
									m_cameraAnimator.Play("NormalToPullAngle", () => { isDone = true; });
									break;
								}
							default:
								{
									isDone = true;
									break;
								}
						}
						break;
					}
				case ZoomType.QuestClear:
					{
						switch (m_cameraZoomType)
						{
							case ZoomType.Approach:
								{
									m_cameraAnimator.Play("ApproachToQuestClearAngle", () => { isDone = true; });
									break;
								}
							default:
								{
									isDone = true;
									break;
								}
						}
						break;
					}
				case ZoomType.Customize:
					{
						switch (m_cameraZoomType)
						{
							case ZoomType.Normal:
								{
									m_cameraAnimator.Play("NormalToCustomizeAngle", () => { isDone = true; });
									break;
								}
							default:
								{
									isDone = true;
									break;
								}
						}
						break;
					}
			}
			while (!isDone) { yield return null; }

			m_cameraZoomType = type;

			if (callback != null)
			{
				callback();
			}
		}
	}
}