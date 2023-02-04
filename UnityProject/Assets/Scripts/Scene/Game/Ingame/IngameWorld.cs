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
		public class SearchInData
		{
			public enum ActionType
			{
				Action,
				Talk
			}
			private int m_enemyId;
			public int EnemyId => m_enemyId;

			private int m_controllId;
			public int ControllId => m_controllId;

			private Transform m_target;
			public Transform Target => m_target;

			private ActionType m_type;
			public ActionType Type => m_type;

			public SearchInData(int enemyId, int controllId, Transform target, ActionType type)
			{
				m_enemyId = enemyId;
				m_controllId = controllId;
				m_target = target;
				m_type = type;
			}
		}

		[SerializeField]
		private world.PlayerChara m_player = null;

		[SerializeField]
		private world.RenderPlate m_renderPlate = null;

		[SerializeField]
		private world.EffectController m_effectController = null;

		[SerializeField]
		private EnemyDataAsset m_enemyAssetData = null;



		private Game.GameMode m_gameMode = Game.GameMode.None;

		//private data.UserData.LocalSaveData.AppMode m_appModeList = data.UserData.LocalSaveData.AppMode.None;

		//private int m_worldDataIndex = -1;
		private UnityAction<int, UnityAction<ingame.StageScene>> m_loadStageEvent = null;

		private UnityAction<string, UnityAction> m_ingameEvent = null;

		private UnityAction<SearchInData> m_updateCharaActionButtonEvent = null;

		private ingame.StageScene m_stage = null;

		private List<string> m_eventParamList = new List<string>();

		private List<SearchInData> m_searchInDataList = new List<SearchInData>();



		public void Initialize(
			UnityAction<int, UnityAction<scene.game.ingame.StageScene>> loadStageEvent,
			UnityAction<string, UnityAction> ingameEvent,
			UnityAction<SearchInData> updateCharaActionButtonEvent,
			UnityAction callback)
		{
			m_loadStageEvent = loadStageEvent;
			m_ingameEvent = ingameEvent;
			m_updateCharaActionButtonEvent = updateCharaActionButtonEvent;
			
			m_player.Initialize();
			m_player.Enable = false;

			m_renderPlate.Initialize();

			m_effectController.Initialize(null);

			StartCoroutine(UpdateCoroutine());

			callback();
		}

		public IEnumerator LoadStage(int stageId, int dataIndex)
		{
			bool isDone = false;

			m_loadStageEvent(stageId, (stage) =>
			{
				m_stage = stage;
				m_stage.Initialize(
					m_player.transform,
					m_enemyAssetData,
					SetEventParam,
					SetEventParam);
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

		public IEnumerator ChangeMapInCoroutine()
		{
			if (m_stage != null)
			{
				yield return m_stage.ChangeMapInCoroutine();
			}
			m_player.Enable = true;
		}

		public IEnumerator ChangeMapOutCoroutine()
		{
			if (m_stage != null)
			{
				yield return m_stage.ChangeMapOutCoroutine();
			}
			m_player.Enable = false;
		}

		public void SearchIn(int controllId)
		{
			var enemy = m_stage.GetEnemy(controllId);
			enemy.SearchIn();

			var transform = enemy.GetHeadTransform();
			var data = m_searchInDataList.Find(d => (d.ControllId == controllId));
			if (data == null)
			{
				SearchInData.ActionType type = (enemy.IsActionEvent() == false) ?
					SearchInData.ActionType.Action :
					SearchInData.ActionType.Talk;
				m_searchInDataList.Add(new SearchInData(
					enemy.EnemyId,
					controllId,
					transform,
					type));
				UpdateCharaActionButton();
			}
		}

		public void SearchOut(int controllId)
		{
			var enemy = m_stage.GetEnemy(controllId);
			enemy.SearchOut();

			var data = m_searchInDataList.Find(d => (d.ControllId == controllId));
			if (data != null)
			{
				m_searchInDataList.Remove(data);
				UpdateCharaActionButton();
			}
		}

		public void RemoveTarget(int controllId)
		{
		}

		public void UpdateSearch()
		{
			m_effectController.Play(world.EffectController.Type.Search, m_player.transform.position, null);
		}

		public void OnCharaActionButtonPressed(int controllId)
		{
			StartCoroutine(OnCharaActionButtonPressedCoroutine(controllId));
		}

		private IEnumerator OnCharaActionButtonPressedCoroutine(int controllId)
		{
			var enemy = m_stage.GetEnemy(controllId);
			if (enemy == null)
			{
				yield break;
			}

			//m_effectController.Play(world.EffectController.Type.Freeze, enemy.transform.position, null);

			var data = m_searchInDataList.Find(d => (d.ControllId == controllId));
			if (data == null)
			{
				yield break;
			}

			m_searchInDataList.Remove(data);
			UpdateCharaActionButton();

			string actionEvent = "";
			bool isDone = false;
			enemy.OnCharaActionButtonPressed((e) =>
			{
				actionEvent = e;
				isDone = true;
			});
			while (!isDone) { yield return null; }

			if (string.IsNullOrEmpty(actionEvent) == true)
			{
				yield break;
			}

			// enemy.ActionEventによるアクション
			SetEventParam(actionEvent);
		}

		private void UpdateCharaActionButton()
		{
			m_searchInDataList.Sort((a, b) =>
			{
				float disA = (a.Target.position - m_player.transform.position).magnitude;
				float disB = (a.Target.position - m_player.transform.position).magnitude;
				if (disA > disB)
				{
					return 1;
				}
				else
				{
					return -1;
				}
			});
			if (m_searchInDataList.Count > 0)
			{
				m_updateCharaActionButtonEvent(m_searchInDataList[0]);
			}
			else
			{
				m_updateCharaActionButtonEvent(null);
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

		public void OnPlayerDragEvent(Vector2 _dragVector)
		{
			if (m_gameMode != Game.GameMode.None)
			{
				return;
			}

			m_player.DragEvent(_dragVector);
		}

		public void OnPlayerEndDragEvent()
		{
			if (m_gameMode != Game.GameMode.None)
			{
				return;
			}

			m_player.EndDragEvent();
		}

		private IEnumerator UpdateCoroutine()
		{
			while (true)
			{
				yield return EventActionCoroutine();

				yield return null;
			}
		}

		private void SetEventParam(string eventParam)
		{
			m_eventParamList.Add(eventParam);
		}

		private IEnumerator EventActionCoroutine()
		{
			if (m_eventParamList.Count <= 0)
			{
				yield break;
			}
			string eventParam = m_eventParamList[0];

			m_player.Enable = false;

			bool isDone = false;
			m_ingameEvent(eventParam, () => { isDone = true; });
			while (!isDone) { yield return null; }

			m_player.Enable = true;
			m_eventParamList.RemoveAt(0);

			yield return null;
		}

		public EnemyDataAsset.Data.ColorData GetColorData(int enemyId, int controllId)
		{
			var enemyData = m_enemyAssetData.List.Find(d => d.EnemyId == enemyId);
			if (enemyData == null)
			{
				return null;
			}

			var colorData = enemyData.ColorDatas.Where(d => d.ColorId == controllId).First();
			return colorData;
		}

		public void PlayPlayerReaction(UnityAction callback)
		{
			m_player.PlayReaction(callback);
		}

		public void PlayEnemyReaction(int controllId, UnityAction callback)
		{
			var enemy = m_stage.GetEnemy(controllId);
			enemy.PlayReaction(callback);
		}

		public Vector3 GetPlayerPosition()
		{
			return m_player.transform.position;
		}
	}
}
