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
			Delight,    // 喜ぶ
			Restraint,  // 拘束
		}

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

		private UnityAction<int, StageScene, UnityAction<StageScene>> m_changeMapEvent;

		private UnityAction<string, UnityAction> m_ingameEvent;

		private UnityAction<SearchInData> m_updateCharaActionButtonEvent;

		private StageScene m_stage;

		private List<string> m_eventParamList = new List<string>();

		private Coroutine m_setupEventParamCoroutine;

		private bool m_isEventLock = false;

		private List<SearchInData> m_searchInDataList = new List<SearchInData>();

		private UnityAction m_playLoopEffectOutEvent;



		public void Initialize(
			UnityAction<int, StageScene, UnityAction<StageScene>> changeMapEvent,
			UnityAction<string, UnityAction> ingameEvent,
			UnityAction<SearchInData> updateCharaActionButtonEvent,
			UnityAction callback)
		{
			m_changeMapEvent = changeMapEvent;
			m_ingameEvent = ingameEvent;
			m_updateCharaActionButtonEvent = updateCharaActionButtonEvent;
			
			m_player.Initialize();
			m_player.SetEnable(false);

			m_renderPlate.Initialize();

			m_effectController.Initialize();

			callback();
		}

		public IEnumerator ChangeMapCoroutine(int stageId, int dataIndex)
		{
			bool isDone = false;

			m_changeMapEvent(stageId, m_stage, (stage) =>
			{
				m_stage = stage;
				m_stage.Initialize(
					m_player.transform,
					m_enemyAssetData,
					SetupEventParam,
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

		public IEnumerator ChangeMapInCoroutine()
		{
			if (m_stage != null)
			{
				yield return m_stage.ChangeMapInCoroutine();
			}
			m_player.SetEnable(true);
		}

		public IEnumerator ChangeMapOutCoroutine()
		{
			if (m_stage != null)
			{
				yield return m_stage.ChangeMapOutCoroutine();
			}
			m_player.SetEnable(false);

			m_searchInDataList.Clear();
			UpdateCharaActionButton();
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

		public void OnCharaActionButtonPressed(int controllId, UnityAction callback)
		{
			StartCoroutine(OnCharaActionButtonPressedCoroutine(controllId, callback));
		}

		private IEnumerator OnCharaActionButtonPressedCoroutine(int controllId, UnityAction callback)
		{
			var enemy = m_stage.GetEnemy(controllId);
			if (enemy == null)
			{
				if (callback != null)
				{
					callback();
				}
				yield break;
			}

			var data = m_searchInDataList.Find(d => (d.ControllId == controllId));
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
			m_player.SetEnable(false);
			m_player.LookTarget(enemy.TransformPosition, () => { doneCount++; });
			enemy.SetEnable(false);
			enemy.LookTarget(m_player.transform.position, () => { doneCount++; });
			while (doneCount < 2) { yield return null; }

			doneCount = 0;
			m_player.OnCharaActionButtonPressed(() =>
			{
				doneCount++;
			});
			string actionEvent = "";
			enemy.OnCharaActionButtonPressed((e) =>
			{
				actionEvent = e;
				doneCount++;
			});
			while (doneCount < 2) { yield return null; }

			m_player.SetEnable(true);
			enemy.SetEnable(true);

			if (string.IsNullOrEmpty(actionEvent) == false)
			{
				// enemy.ActionEventによるアクション
				SetupEventParam(actionEvent);
			}

			if (callback != null)
			{
				callback();
			}
		}

		private void UpdateCharaActionButton()
		{
			if (m_searchInDataList.Count > 0)
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

		private void SetupEventParam(string eventParam)
		{
			Debug.Log("SetupEventParam eventParam = " + eventParam);
			m_eventParamList.Add(eventParam);
			if (m_setupEventParamCoroutine == null)
			{
				m_setupEventParamCoroutine = StartCoroutine(SetupEventParamCoroutine());
			}
		}

		private IEnumerator SetupEventParamCoroutine()
		{
			if (m_eventParamList.Count <= 0)
			{
				yield break;
			}

			// ロック解除まで待機
			while (m_isEventLock) { yield return null; }

			m_player.SetEnable(false);

			bool isDone = false;
			string eventParam = m_eventParamList[0];
			m_ingameEvent(eventParam, () => { isDone = true; });
			while (!isDone) { yield return null; }

			m_player.SetEnable(true);

			m_eventParamList.RemoveAt(0);

			// 再帰
			yield return SetupEventParamCoroutine();

			m_setupEventParamCoroutine = null;
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

		public void PlayMovieCharaReaction(ReactionType type, int enemyControllId, UnityAction callback)
		{
			StartCoroutine(PlayMovieCharaReactionCoroutine(type, enemyControllId, callback));
		}

		private IEnumerator PlayMovieCharaReactionCoroutine(ReactionType type, int enemyControllId, UnityAction callback)
		{
			int doneCount = 0;
			int doneCountMax = 0;

			doneCountMax++;
			m_player.PlayReaction((world.PlayerChara.ReactionType)type, () => { doneCount++; });

			var enemy = m_stage.GetEnemy(enemyControllId);
			switch (type)
			{
				case ReactionType.Delight:
					{
						break;
					}
				case ReactionType.Restraint:
					{
						doneCountMax++;
						var effectOutEvent = m_effectController.PlayLoop(
							world.EffectController.LoopEffectType.Restraint,
							enemy.TransformPosition);
						enemy.PlayReaction((world.Enemy.ReactionType)type, effectOutEvent, () => { doneCount++; });

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
	}
}
