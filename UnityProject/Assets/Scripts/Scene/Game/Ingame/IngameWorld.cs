using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace scene.game.ingame
{
	[System.Serializable]
	public class IngameWorld : MonoBehaviour
	{
		public enum ReactionType
		{
			DelightIn,  // 喜ぶ
			DelightOut,  // 喜ぶ
			Restraint,  // 拘束
		}

		public class SearchInData
		{
			private world.ActionTargetBase.Category m_category;
			public world.ActionTargetBase.Category Category => m_category;

			private int m_controllId;
			public int ControllId => m_controllId;

			private world.ActionTargetBase m_target;
			public world.ActionTargetBase Target => m_target;

			public SearchInData(world.ActionTargetBase.Category category, int controllId, world.ActionTargetBase target)
			{
				m_category = category;
				m_controllId = controllId;
				m_target = target;
			}
		}

		[SerializeField]
		private world.PlayerChara m_player = null;

		[SerializeField]
		private world.EffectController m_effectController = null;



		private UnityAction<int, StageScene, UnityAction<StageScene>> m_loadMapEvent;

		private UnityAction<string, UnityAction> m_ingameEvent;

		private Transform m_ingameCameraTransform;

		private UnityAction<SearchInData> m_updateMainWindowEvent;

		private StageScene m_stage;

		private List<string> m_eventParamList = new List<string>();

		//private Coroutine m_setupEventParamCoroutine;

		private bool m_isEventLock = false;

		private List<SearchInData> m_searchInDataList = new List<SearchInData>();

		private UnityAction m_playLoopEffectOutEvent;



		public void Initialize(
			UnityAction<int, StageScene, UnityAction<StageScene>> loadMapEvent,
			UnityAction<string, UnityAction> ingameEvent,
			Transform ingameCameraTransform,
			UnityAction<SearchInData> UpdateMainWindow,
			UnityAction callback)
		{
			m_loadMapEvent = loadMapEvent;
			m_ingameEvent = ingameEvent;
			m_ingameCameraTransform = ingameCameraTransform;
            m_updateMainWindowEvent = UpdateMainWindow;
			
			m_player.Initialize(m_ingameCameraTransform);
			m_player.SetEnable(false);
			m_player.UpdateParam();

			m_effectController.Initialize();

			StartCoroutine(SetupEventParamCoroutine());

			callback();
		}

		public IEnumerator LoadMapCoroutine(int stageId, int dataIndex)
		{
			bool isDone = false;

			m_loadMapEvent(stageId, m_stage, (stage) =>
			{
				m_stage = stage;
				m_stage.Initialize(
					m_ingameCameraTransform,
					SetupEventParam);
				isDone = true;
			});
			while (!isDone) { yield return null; }

			var playerPositionData = m_stage.GetPositionData(dataIndex);
			m_player.transform.SetPositionAndRotation(
				playerPositionData.m_position,
				Quaternion.Euler(playerPositionData.m_euler));
		}

		public IEnumerator DropItemCoroutine(int id)
		{
			//var worldData = m_worldDataList[m_worldDataIndex];

			//AnimationCurve curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

			//Vector3 nowPos = m_cameraTransform.position;
			//var item = m_stageWorld.GetItem(id);
			//Vector3 nextPos = item.transform.position;// + m_cameraModeOffsetDatas[0].m_position;
			//float time = 1.0f;
			//float nowTime = 0.0f;
			//while (nowTime < time)
			//{
			//	nowTime += Time.deltaTime;

			//	float t = nowTime / time;
			//	Vector3 setPos = nowPos + (nextPos - nowPos) * curve.Evaluate(t);
			//	m_cameraTransform.position = setPos;

			//	yield return null;
			//}
			//m_cameraTransform.position = nextPos;

			//yield return item.SetActiveColoutine(true);

			//nowPos = m_cameraTransform.position;
			//nextPos = m_player.transform.position;// + m_cameraModeOffsetDatas[0].m_position;
			//time = 1.0f;
			//nowTime = 0.0f;
			//while (nowTime < time)
			//{
			//	nowTime += Time.deltaTime;

			//	float t = nowTime / time;
			//	Vector3 setPos = nowPos + (nextPos - nowPos) * curve.Evaluate(t);
			//	m_cameraTransform.position = setPos;

			//	yield return null;
			//}
			//m_cameraTransform.position = nextPos;

			yield return null;
		}

		public IEnumerator GetItemCoroutine(int id)
		{
			var item = m_stage.GetItem(id);
			yield return item.SetActiveColoutine(false);
		}

		public IEnumerator ChangeMapInCoroutine(float time)
		{
			if (m_stage != null)
			{
				yield return m_stage.ChangeMapInCoroutine(time);
			}
			m_player.SetEnable(true);
		}

		public IEnumerator ChangeMapOutCoroutine(float time)
		{
			if (m_stage != null)
			{
				yield return m_stage.ChangeMapOutCoroutine(time);
			}
			m_player.SetEnable(false);

			m_searchInDataList.Clear();
			UpdateCharaActionButton();
		}

		public void SearchIn(world.ActionTargetBase.Category category, int controllId)
		{
			var actionTarget = GetActionTarget(category, controllId);
			if (actionTarget == null)
			{
				return;
			}

			actionTarget.SearchIn(m_player.transform.position);

			var data = m_searchInDataList.Find(d => (d.ControllId == controllId));
			if (data != null)
			{
				return;
			}

			m_searchInDataList.Add(new SearchInData(
				category,
				controllId,
				actionTarget));
			UpdateCharaActionButton();
		}

		public void SearchOut(world.ActionTargetBase.Category category, int controllId)
		{
			var actionTarget = GetActionTarget(category, controllId);
			if (actionTarget == null)
			{
				return;
			}

			actionTarget.SearchOut();

			var data = m_searchInDataList.Find(d => (d.ControllId == controllId));
			if (data == null)
			{
				return;
			}

			m_searchInDataList.Remove(data);
			UpdateCharaActionButton();
		}

		public void OnCharaActionButtonPressed(world.ActionTargetBase.Category category, int controllId, UnityAction callback)
		{
			StartCoroutine(OnCharaActionButtonPressedCoroutine(category, controllId, callback));
		}

		private IEnumerator OnCharaActionButtonPressedCoroutine(world.ActionTargetBase.Category category, int controllId, UnityAction callback)
		{
			world.ActionTargetBase actionTarget = GetActionTarget(category, controllId);
			if (actionTarget == null)
			{
				if (callback != null)
				{
					callback();
				}
				yield break;
			}

			var data = m_searchInDataList.Find(d => (d.Category == category && d.ControllId == controllId));
			if (data == null)
			{
				if (callback != null)
				{
					callback();
				}
				yield break;
			}

			m_searchInDataList.Remove(data);
			UpdateCharaActionButton();

			int doneCount = 0;
			int doneCountMax = 1;
			m_player.SetEnable(false);
			m_player.LookTarget(actionTarget.TransformPosition, () => { doneCount++; });
			while (doneCount < doneCountMax) { yield return null; }

			doneCount = 0;
			m_player.OnCharaActionButtonPressed(() =>
			{
				doneCount++;
			});
			actionTarget.OnCharaActionButtonPressed(() =>
			{
				doneCount++;
			});
			while (doneCount < 2) { yield return null; }

			m_player.SetEnable(true);
			actionTarget.SetEnable(true);

			if (callback != null)
			{
				callback();
			}
		}

		public world.ActionTargetBase GetActionTarget(world.ActionTargetBase.Category category, int controllId)
		{
			world.ActionTargetBase actionTarget = null;
			switch (category)
			{
				case world.ActionTargetBase.Category.NPC:
					{
						actionTarget = m_stage.GetNPC(controllId);
						break;
					}
			}
			return actionTarget;
		}

		public IEnumerator DeleteNPCCoroutine(int controllId)
		{
			var npc = GetActionTarget(world.ActionTargetBase.Category.NPC, controllId);
			bool isDone = false;
			m_effectController.Play(world.EffectController.PlayEffectType.Delete, npc.TransformPosition, ()=> { isDone = true; });
			m_stage.DeleteNPC(controllId);
			while (!isDone) { yield return null; }
		}

		private void UpdateCharaActionButton()
		{
			if (m_searchInDataList.Count > 0)
			{
                m_updateMainWindowEvent(m_searchInDataList[0]);
			}
			else
			{
                m_updateMainWindowEvent(null);
			}
		}

		//private IEnumerator UpdateCameraCoroutine(UnityAction callback)
		//{
		//	AnimationCurve curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

		//	Vector3 nowPos = m_cameraTransform.position;
		//	Vector3 nextPos = GetCameraPosition();
		//	Vector3 nowEuler = m_cameraTransform.eulerAngles;
		//	Vector3 nextEuler = GetCameraEuler();
		//	Vector3 nowEuler2 = CommonMath.GetEulerAngles(nowEuler);
		//	if (Mathf.Abs(nowEuler.y - nextEuler.y) > Mathf.Abs(nowEuler2.y - nextEuler.y))
		//	{
		//		nowEuler = nowEuler2;
		//	}

		//	float time = 0.2f;
		//	float nowTime = 0.0f;
		//	while (nowTime < time)
		//	{
		//		float t = nowTime / time;

		//		Vector3 setPos = nowPos + (nextPos - nowPos) * curve.Evaluate(t);
		//		Vector3 setEuler = nowEuler + (nextEuler - nowEuler) * curve.Evaluate(t);
		//		m_cameraTransform.SetPositionAndRotation(setPos, Quaternion.Euler(setEuler));

		//		nowTime += Time.deltaTime;
		//		yield return null;
		//	}
		//	m_cameraTransform.SetPositionAndRotation(nextPos, Quaternion.Euler(nextEuler));

		//	if (callback != null)
		//	{
		//		callback();
		//	}
		//}


		//public void InfoButtonEvent(bool _value)
		//{
		//	StartCoroutine(InfoButtonEventCoroutine(_value));
		//}

		//private IEnumerator InfoButtonEventCoroutine(bool _value)
		//{
		//	AnimationCurve curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

		//	if (_value == true)
		//	{
		//		m_mode = Mode.Search;

		//		Vector3 nowPos = m_cameraTransform.position;
		//		Vector3 nextPos = m_chara.Transform.position + new Vector3(-8, 8, -8);
		//		Vector3 nowEuler = m_cameraTransform.eulerAngles;
		//		Vector3 nextEuler = new Vector3(30, 30, 0);
		//		float time = 0.2f;
		//		float nowTime = 0.0f;
		//		while (nowTime < time)
		//		{
		//			float t = nowTime / time;

		//			Vector3 setPos = nowPos + (nextPos - nowPos) * curve.Evaluate(t);
		//			m_cameraTransform.position = setPos;
		//			Vector3 setEuler = nowEuler + (nextEuler - nowEuler) * curve.Evaluate(t);
		//			m_cameraTransform.eulerAngles = setEuler;

		//			nowTime += Time.deltaTime;
		//			yield return null;
		//		}
		//		m_cameraTransform.position = nextPos;
		//		m_cameraTransform.eulerAngles = nextEuler;

		//		m_renderPlate.Transform.position = m_chara.Transform.position + new Vector3(-6, 4, -2);
		//		m_renderPlate.Transform.eulerAngles = Vector3.zero;
		//		yield return m_renderPlate.SetActiveColoutine(true);
		//	}
		//	else
		//	{
		//		m_mode = Mode.None;

		//		yield return m_renderPlate.SetActiveColoutine(false);

		//		Vector3 nowPos = m_cameraTransform.position;
		//		Vector3 nextPos = m_chara.Transform.position + new Vector3(-12.0f, 12.0f, -12.0f);
		//		Vector3 nowEuler = m_cameraTransform.eulerAngles;
		//		Vector3 nextEuler = new Vector3(30, 45, 0);
		//		float time = 0.1f;
		//		float nowTime = 0.0f;
		//		while (nowTime < time)
		//		{
		//			float t = nowTime / time;

		//			Vector3 setPos = nowPos + (nextPos - nowPos) * curve.Evaluate(t);
		//			m_cameraTransform.position = setPos;
		//			Vector3 setEuler = nowEuler + (nextEuler - nowEuler) * curve.Evaluate(t);
		//			m_cameraTransform.eulerAngles = setEuler;

		//			nowTime += Time.deltaTime;
		//			yield return null;
		//		}
		//		m_cameraTransform.position = nextPos;
		//		m_cameraTransform.eulerAngles = nextEuler;
		//	}
		//}

		//private IEnumerator ChangeModeCoroutine(Game.Mode _mode)
		//{
		//	AnimationCurve curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

		//	switch (_mode)
		//	{
		//		case Game.Mode.None:
		//			{
		//				if (m_mode == Game.Mode.None)
		//				{
		//					break;
		//				}

		//				m_mode = Game.Mode.None;

		//				m_player.Enable = true;
		//				m_search.Enable = false;
		//				m_worldDataList[m_worldDataIndex].m_mapCollider.SetActive(true);

		//				yield return m_search.AnimationCoroutine(world.PlayerSearch.AnimType.Out);

		//				Vector3 nowPos = m_cameraTransform.position;
		//				Vector3 nextPos = m_player.transform.position + m_cameraModeOffsetDatas[(int)m_mode].m_position;
		//				float time = 0.2f;
		//				float nowTime = 0.0f;
		//				while (nowTime < time)
		//				{
		//					float t = nowTime / time;

		//					Vector3 setPos = nowPos + (nextPos - nowPos) * curve.Evaluate(t);
		//					m_cameraTransform.position = setPos;

		//					nowTime += Time.deltaTime;
		//					yield return null;
		//				}
		//				m_cameraTransform.position = nextPos;

		//				break;
		//			}
		//		case Game.Mode.Search:
		//			{
		//				if (m_mode == Game.Mode.Search)
		//				{
		//					break;
		//				}

		//				m_mode = Game.Mode.Search;

		//				m_player.Enable = false;
		//				m_search.Enable = true;
		//				m_worldDataList[m_worldDataIndex].m_mapCollider.SetActive(false);

		//				m_search.transform.position = m_player.transform.position;
		//				yield return m_search.AnimationCoroutine(world.PlayerSearch.AnimType.In);
		//				yield return m_search.AnimationCoroutine(world.PlayerSearch.AnimType.LOOP1);

		//				Vector3 nowPos = m_cameraTransform.position;
		//				Vector3 nextPos = m_search.transform.position + m_cameraModeOffsetDatas[(int)m_mode].m_position;
		//				float time = 0.1f;
		//				float nowTime = 0.0f;
		//				while (nowTime < time)
		//				{
		//					float t = nowTime / time;

		//					Vector3 setPos = nowPos + (nextPos - nowPos) * curve.Evaluate(t);
		//					m_cameraTransform.position = setPos;

		//					nowTime += Time.deltaTime;
		//					yield return null;
		//				}
		//				m_cameraTransform.position = nextPos;

		//				break;
		//			}
		//	}


		//}

		public void OnPlayerDragEvent(Vector2 direction)
		{
			m_player.DragEvent(direction);
		}

		public void OnPlayerEndDragEvent()
		{
			m_player.EndDragEvent();
		}

		private void SetupEventParam(string eventParam)
		{
			Debug.Log("eventParam = " + eventParam);
			m_eventParamList.Add(eventParam);
		}

		private IEnumerator SetupEventParamCoroutine()
		{
			while (true)
			{
				if (m_eventParamList.Count <= 0)
				{
					yield return null;
					continue;
				}

				// ロック解除まで待機
				while (m_isEventLock) { yield return null; }

				m_player.SetEnable(false);

				bool isDone = false;
				string eventParam = m_eventParamList[0];
				m_ingameEvent(eventParam, () => { isDone = true; });
				while (!isDone) { yield return null; }

				m_player.SetEnable(true);
				string[] eventType = eventParam.Split('_');
				switch (eventType[0])
				{
					case "OpenDialog":
						{
							if (eventType[1] == "Customize")
							{
								// カスタマイズにより能力が変動しているので、更新
								m_player.UpdateParam();
							}
							break;
						}
				}

				m_eventParamList.RemoveAt(0);

				yield return null;
			}
		}

		public void PlayMovieCharaReaction(
			ReactionType type,
			world.ActionTargetBase.Category category,
			int controllId,
			UnityAction callback)
		{
			StartCoroutine(PlayMovieCharaReactionCoroutine(type, category, controllId, callback));
		}

		private IEnumerator PlayMovieCharaReactionCoroutine(
			ReactionType type,
			world.ActionTargetBase.Category category,
			int controllId,
			UnityAction callback)
		{
			int doneCount = 0;
			int doneCountMax = 0;

			doneCountMax++;
			m_player.PlayReaction(type, () => { doneCount++; });

			world.ActionTargetBase actionTarget = GetActionTarget(category, controllId);
			switch (type)
			{
				case ReactionType.DelightIn:
					{
						break;
					}
				case ReactionType.DelightOut:
					{
						break;
					}
				case ReactionType.Restraint:
					{
						doneCountMax++;
						var effectOutEvent = m_effectController.PlayLoop(
							world.EffectController.LoopEffectType.Restraint,
							actionTarget.TransformPosition);
						actionTarget.PlayReaction(type, effectOutEvent, () => { doneCount++; });

						break;
					}
				default:
					{
						break;
					}
			}

			while (doneCount < doneCountMax) { yield return null; }

			if (callback != null)
			{
				callback();
			}
		}

		public Vector3 GetPlayerPosition()
		{
			if (m_player.transform == null)
			{
				return Vector3.zero;
			}
			return m_player.transform.position;
		}

		public world.ActionTargetBase GetActionTarget()
		{
			if (m_searchInDataList.Count <= 0)
			{
				return null;
			}

			return m_searchInDataList[0].Target;
		}

		public void OnMovieStart(string[] paramStrings, UnityAction callback)
		{
			switch (paramStrings[0])
			{
				default:
					{
						m_stage.OnMovieStart(paramStrings, callback);
						break;
					}
			}
		}
	}
}
