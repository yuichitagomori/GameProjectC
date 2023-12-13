using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game
{
	[System.Serializable]
	public class Ingame : MonoBehaviour
	{
		///// <summary>
		///// インゲーム用カメラ
		///// </summary>
		//[SerializeField]
		//private Transform m_cameraTransform;

		///// <summary>
		///// インゲーム用カメラトランスフォーム
		///// </summary>
		//[SerializeField]
		//private Transform m_cameraParentTransform;

		//[SerializeField]
		//private ingame.IngameWorld m_world;

		//[SerializeField]
		//private Transform[] m_cameraAngleTransforms;



		//private UnityAction<string, UnityAction> m_parentIngameEvent;

		//private Vector2 m_cameraBeginMovePosition = Vector2.zero;

		//private Vector3 m_cameraDragEuler = Vector3.zero;

		//private Vector3 m_cameraEuler = Vector3.zero;

		//private ingame.world.ActionTargetBase m_actionTarget;

		//private Vector3 cameraTargetPositionNow = Vector3.zero;

		private UnityAction<string, ingame.GameGenreBase, UnityAction<ingame.GameGenreBase>> m_loadGameEvent;

		private ingame.GameGenreBase m_gameGenre;

		private UnityAction<string, UnityAction> m_outgameSetupEvent;

		public void Initialize(
			UnityAction<string, ingame.GameGenreBase, UnityAction<ingame.GameGenreBase>> loadGameEvent,
			UnityAction<string, UnityAction> outgameSetupEvent)
		{
			m_loadGameEvent = loadGameEvent;
			m_outgameSetupEvent = outgameSetupEvent;
		}

		//public void Initialize(
		//	UnityAction<string, UnityAction> ingameEvent,
		//	UnityAction<int, ingame.StageScene, UnityAction<ingame.StageScene>> loadMapEvent,
		//	UnityAction<ingame.IngameWorld.SearchInData> updateMainWindow,
		//	UnityAction callback)
		//{
		//	m_parentIngameEvent = ingameEvent;

		//	StartCoroutine(InitializeCoroutine(
		//		loadMapEvent,
		//              updateMainWindow,
		//		callback));
		//}

		//private IEnumerator InitializeCoroutine(
		//	UnityAction<int, ingame.StageScene, UnityAction<ingame.StageScene>> loadMapEvent,
		//	UnityAction<ingame.IngameWorld.SearchInData> UpdateMainWindow,
		//	UnityAction callback)
		//{
		//	bool isDone = false;
		//	m_world.Initialize(
		//		loadMapEvent,
		//		IngameEvent,
		//		m_cameraTransform,  // Lookを行うためにPositionが欲しい用途で譲渡
		//              UpdateMainWindow,
		//		() => { isDone = true; });
		//	while (!isDone) { yield return null; }

		//	m_cameraEuler = Vector3.zero;

		//	if (callback != null)
		//	{
		//		callback();
		//	}
		//}

		public void Go()
		{
		}

		//private void FixedUpdate()
		//{
		//	// カメラ座標更新は、FixedUpdateで呼ばないとガタガタする
		//	if (m_cameraParentTransform == null)
		//	{
		//		return;
		//	}

		//	cameraTargetPositionNow = Vector3.Lerp(cameraTargetPositionNow, GetCameraTargetPosition(), 0.2f);
		//	m_cameraParentTransform.SetPositionAndRotation(
		//		cameraTargetPositionNow,
		//		Quaternion.Euler(GetCameraEuler()));
		//}

		//private void IngameEvent(string eventParam, UnityAction callback)
		//{
		//	Debug.Log("eventParam = " + eventParam);
		//	string[] eventType = eventParam.Split('_');
		//	switch (eventType[0])
		//	{
		//		case "SearchIn":
		//			{
		//				ingame.world.ActionTargetBase.Category category = (ingame.world.ActionTargetBase.Category)int.Parse(eventType[1]);
		//				int controllId = int.Parse(eventType[2]);
		//				SearchIn(category, controllId, callback);
		//				break;
		//			}
		//		case "SearchOut":
		//			{
		//				ingame.world.ActionTargetBase.Category category = (ingame.world.ActionTargetBase.Category)int.Parse(eventType[1]);
		//				int controllId = int.Parse(eventType[2]);
		//				SearchOut(category, controllId, callback);
		//				break;
		//			}
		//		default:
		//			{
		//				m_parentIngameEvent(eventParam, callback);
		//				break;
		//			}
		//	}
		//}

		//public void ChangeMap(int stageId, int dataIndex, bool isReady, UnityAction callback)
		//{
		//	StartCoroutine(ChangeMapCoroutine(stageId, dataIndex, isReady, callback));
		//}

		//private IEnumerator ChangeMapCoroutine(int stageId, int dataIndex, bool isReady, UnityAction callback)
		//{
		//	if (isReady == false)
		//	{
		//		yield return m_world.ChangeMapOutCoroutine(0.5f);
		//	}

		//	yield return m_world.LoadMapCoroutine(stageId, dataIndex);

		//	if (isReady == false)
		//	{
		//		yield return m_world.ChangeMapInCoroutine(0.5f);
		//	}
		//	else
		//	{
		//		yield return m_world.ChangeMapInCoroutine(0.0f);
		//	}

		//	if (callback != null)
		//	{
		//		callback();
		//	}
		//}

		//public void CameraBeginMoveEvent(Vector2 position)
		//{
		//	m_cameraBeginMovePosition = position;
		//}

		//public void CameraMoveEvent(Vector2 position)
		//{
		//	Vector2 direction = position - m_cameraBeginMovePosition;
		//	float dragX = direction.x * 0.2f;
		//	m_cameraDragEuler = new Vector3(0.0f, dragX, 0.0f);
		//}

		//public void CameraEndMoveEvent()
		//{
		//	m_cameraEuler += m_cameraDragEuler;
		//	m_cameraDragEuler = Vector3.zero;
		//}

		//public void InputEvent(KeyCode[] pressKeys)
		//{
		//}

		//public void ClickEvent(Vector2 position)
		//{
		//	var camera = m_cameraTransform.GetComponent<Camera>();
		//	var ray = camera.ViewportPointToRay(position);
		//	m_world.ClickEvent(ray);
		//}

		//public void CameraZoomEvent(float value)
		//{
		//	Vector3 minPosition = m_cameraAngleTransforms[0].localPosition;
		//	Quaternion minRotation = m_cameraAngleTransforms[0].localRotation;
		//	Vector3 maxPosition = m_cameraAngleTransforms[1].localPosition;
		//	Quaternion maxRotation = m_cameraAngleTransforms[1].localRotation;
		//	CommonMath.TransformLerp(
		//		m_cameraTransform,
		//		minPosition,
		//		minRotation,
		//		maxPosition,
		//		maxRotation,
		//		value);
		//}

		//private void SearchIn(ingame.world.ActionTargetBase.Category category, int controllId, UnityAction callback)
		//{
		//	m_world.SearchIn(category, controllId);

		//	m_actionTarget = m_world.GetActionTarget(category, controllId);

		//	if (callback != null)
		//	{
		//		callback();
		//	}
		//}

		//private void SearchOut(ingame.world.ActionTargetBase.Category category, int controllId, UnityAction callback)
		//{
		//	m_world.SearchOut(category, controllId);

		//	m_actionTarget = null;

		//	if (callback != null)
		//	{
		//		callback();
		//	}
		//}

		//public void OnCharaActionButtonPressed(ingame.world.ActionTargetBase.Category category, int controllId, UnityAction callback)
		//{
		//	m_world.OnCharaActionButtonPressed(category, controllId, callback);
		//}

		//public ingame.world.NPC GetNPC(int controllId)
		//{
		//	return (ingame.world.NPC)m_world.GetActionTarget(ingame.world.ActionTargetBase.Category.NPC, controllId);
		//}

		////public void DeleteNPC(int controllId, UnityAction callback)
		////{

		////}

		//public IEnumerator DeleteNPCCoroutine(int controllId)
		//{
		//	yield return m_world.DeleteNPCCoroutine(controllId);

		//	bool isDone = false;
		//	SearchOut(ingame.world.ActionTargetBase.Category.NPC, controllId, () => { isDone = true; });
		//	while (!isDone) { yield return null; }
		//}

		//private Vector3 GetCameraTargetPosition()
		//{
		//	Vector3 position = Vector3.zero;
		//	if (m_actionTarget != null)
		//	{
		//		//position = m_actionTarget.TransformPosition;
		//	}
		//	else
		//	{
		//		position = m_world.GetPlayerPosition();
		//	}
		//	return position;
		//}

		//private Vector3 GetCameraEuler()
		//{
		//	float eulerX = (m_cameraEuler.x + m_cameraDragEuler.x);
		//	float eulerY = (m_cameraEuler.y + m_cameraDragEuler.y) * -1.0f + 270.0f;
		//	if (eulerY > 360.0f)
		//	{
		//		eulerY -= 360.0f;
		//	}
		//	else if (eulerY < 0.0f)
		//	{
		//		eulerY += 360.0f;
		//	}
		//	Vector3 v = new Vector3(eulerX, eulerY, 0.0f);
		//	return v;
		//}


		//public void PlayMovieCharaReaction(
		//	ingame.IngameWorld.ReactionType type,
		//	ingame.world.ActionTargetBase.Category category,
		//	int controllId,
		//	UnityAction callback)
		//{
		//	m_world.PlayMovieCharaReaction(type, category, controllId, callback);
		//}

		public void OnMovieStart(string[] paramStrings, UnityAction callback)
		{
			//switch (paramStrings[0])
			//{
			//	default:
			//		{

			//			break;
			//		}
			//}
			string gameGenreName = paramStrings[0];
			m_loadGameEvent(gameGenreName, m_gameGenre, (ingame.GameGenreBase newGameGenre) =>
			{
				SetupGameGunre(newGameGenre);
				if (callback != null)
				{
					callback();
				}
			});
		}

		private void SetupGameGunre(ingame.GameGenreBase gameGenre)
		{
			m_gameGenre = gameGenre;
			m_gameGenre.SetOutgameSetupEvent(m_outgameSetupEvent);
		}

		public void ResetGame()
		{
			m_loadGameEvent(m_gameGenre.name, m_gameGenre, SetupGameGunre);
		}

		public void OnInputEvent(KeyCode[] pressKeys)
		{
			if (m_gameGenre == null)
			{
				return;
			}
			m_gameGenre.OnInputEvent(pressKeys);
		}
	}
}