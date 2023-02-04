using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene
{
	public class Game : SceneBase
	{
		public enum GameMode
		{
			None,
			App,
		}

		[Header("Game")]

		[SerializeField]
		private game.Ingame m_ingame = null;

		[SerializeField]
		private game.Outgame m_outgame = null;

		[SerializeField]
		private game.MovieController m_movieController = null;



		private GameMode m_gameMode = GameMode.None;

		private scene.game.ingame.StageScene m_stageScene = null;



		public override void Ready(UnityAction callback)
		{
			StartCoroutine(ReadyCoroutine(callback));
		}

		private IEnumerator ReadyCoroutine(UnityAction callback)
		{
			m_movieController.Initialize(
				m_ingame,
				m_outgame);

			var searchTargetList = GeneralRoot.Instance.UserData.Data.SearchTargetList;
			searchTargetList.Clear();
			//for (int i = 0; i < 5; ++i)
			//{
			//	searchTargetList.Add(new data.UserData.LocalSaveData.SearchTargetData(
			//		enemyId: 30,
			//		controllId: i + 1));
			//}

			bool isDone = false;
			m_ingame.Initialize(
				loadStageEvent: LoadStageEvent,
				ingameEvent: IngameEvent,
				updateCharaActionButtonEvent: UpdateCharaActionButton,
				() => { isDone = true; });
			while (!isDone) { yield return null; }
			m_outgame.Initialize(
				cameraHandlerEventData: new game.outgame.Handler.EventData(
					m_ingame.OnCameraDragEvent,
					m_ingame.OnCameraEndDragEvent,
					m_ingame.OnCameraClickEvent),
				playerHandlerEventData: new game.outgame.Handler.EventData(
					m_ingame.OnPlayerDragEvent,
					m_ingame.OnPlayerEndDragEvent,
					m_ingame.OnPlayerClickEvent),
				onCharaActionButtonEvent: OnCharaActionButtonPressed);

			m_gameMode = GameMode.None;

			UpdateOutgameObject();

			callback();
		}

		public override void Go()
		{
			StartCoroutine(m_movieController.ChangeMapCoroutine(1, 0, false, true, null));
		}

		private void LoadStageEvent(int stageId, UnityAction<scene.game.ingame.StageScene> addedEvent)
		{
			StartCoroutine(LoadStageEventCoroutine(stageId, addedEvent));
		}

		private IEnumerator LoadStageEventCoroutine(int stageId, UnityAction<scene.game.ingame.StageScene> addedEvent)
		{
			bool isDone = false;
			if (m_stageScene != null)
			{
				GeneralRoot.Instance.SceneController.RemoveScene(m_stageScene, () => { isDone = true; });
				while (!isDone) { yield return null; }
			}

			GeneralRoot.Instance.SceneController.AddScene<game.ingame.StageScene>(
				sceneName: string.Format("Stage{0:000}", stageId),
				added: (s) =>
				{
					m_stageScene = s;
					addedEvent(m_stageScene);
				},
				finishEvent: null);
		}

		private void IngameEvent(string eventParam, UnityAction callback)
		{
			Debug.Log("eventParam = " + eventParam);
			string[] eventType = eventParam.Split('_');
			switch (eventType[0])
			{
				case "DropItem":
					{
						int id = int.Parse(eventType[1]);
						StartCoroutine(DropItemCoroutine(id, callback));
						break;
					}
				case "GetItem":
					{
						int id = int.Parse(eventType[1]);
						StartCoroutine(GetItemCoroutine(id, callback));
						break;
					}
				case "ChangeMap":
					{
						int mapId = int.Parse(eventType[1]);
						int dataIndex = int.Parse(eventType[2]);
						StartCoroutine(m_movieController.ChangeMapCoroutine(mapId, dataIndex, false, false, callback));
						break;
					}
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
				case "AddTarget":
					{
						int enemyId = int.Parse(eventType[1]);
						int controllId = int.Parse(eventType[2]);
						AddTarget(enemyId, controllId, callback);
						break;
					}
				case "RemoveTarget":
					{
						int enemyId = int.Parse(eventType[1]);
						int controllId = int.Parse(eventType[2]);
						RemoveTarget(enemyId, controllId, callback);
						break;
					}
				case "Movie":
					{
						int movieId = int.Parse(eventType[1]);
						int controllId = int.Parse(eventType[2]);
						m_movieController.Play(movieId, controllId, callback);
						break;
					}
			}
		}

		private IEnumerator DropItemCoroutine(int id, UnityAction callback)
		{
			yield return m_ingame.DropItemCoroutine(id);
			callback();
		}

		private IEnumerator GetItemCoroutine(int id, UnityAction callback)
		{
			yield return m_ingame.GetItemCoroutine(id);
			callback();
		}

		private void SearchIn(int controllId, UnityAction callback)
		{
			m_ingame.SearchIn(controllId);
			callback();
		}

		private void SearchOut(int controllId, UnityAction callback)
		{
			m_ingame.SearchOut(controllId);
			callback();
		}

		private void AddTarget(int enemyId, int controllId, UnityAction callback)
		{
			var searchTargetList = GeneralRoot.Instance.UserData.Data.SearchTargetList;
			searchTargetList.Add(new data.UserData.LocalSaveData.SearchTargetData(
				enemyId,
				controllId));
			UpdateOutgameSearchTarget(callback);
		}

		private void RemoveTarget(int enemyId, int controllId, UnityAction callback)
		{
			StartCoroutine(RemoveTargetCoroutine(enemyId, controllId, callback));
		}

		private IEnumerator RemoveTargetCoroutine(int enemyId, int controllId, UnityAction callback)
		{
			var searchTargetList = GeneralRoot.Instance.UserData.Data.SearchTargetList;
			var targetData = searchTargetList.Find(d => d.EnemyId == enemyId && d.ControllId == controllId);
			if (targetData == null)
			{
				if (callback != null)
				{
					callback();
				}
				yield break;
			}

			searchTargetList.Remove(targetData);
			bool isDone = false;
			UpdateOutgameSearchTarget(() => { isDone = true; });
			while (!isDone) { yield return null; }

			m_ingame.RemoveTarget(controllId);

			if (callback != null)
			{
				callback();
			}
		}

		private void UpdateCharaActionButton(game.ingame.IngameWorld.SearchInData data)
		{
			if (data != null)
			{
				game.outgame.CharaActionButtonElement.ActionType type = game.outgame.CharaActionButtonElement.ActionType.Active;
				switch (data.Type)
				{
					case game.ingame.IngameWorld.SearchInData.ActionType.Action:
						{
							type = game.outgame.CharaActionButtonElement.ActionType.Active;
							break;
						}
					case game.ingame.IngameWorld.SearchInData.ActionType.Talk:
						{
							type = game.outgame.CharaActionButtonElement.ActionType.Talk;
							break;
						}
				}
				var buttonData = new game.outgame.GameUI.CharaActionButtonData(
					data.EnemyId,
					data.ControllId,
					data.Target,
					type);
				m_outgame.UpdateCharaActionButton(buttonData);
			}
			else
			{
				m_outgame.UpdateCharaActionButton(null);
			}
		}

		private void OnCharaActionButtonPressed(int controllId)
		{
			m_ingame.OnCharaActionButtonPressed(controllId);
		}

		private void UpdateOutgameObject()
		{
			UpdateOutgameSearchTarget(null);
		}

		private void UpdateOutgameSearchTarget(UnityAction callback)
		{
			var searchTargetList = GeneralRoot.Instance.UserData.Data.SearchTargetList;
			game.outgame.GameUI.SearchTargetIconColor colorData = default;
			if (searchTargetList.Count > 0)
			{
				colorData = new game.outgame.GameUI.SearchTargetIconColor()
				{
					m_enemyId = searchTargetList[0].EnemyId,
					m_colorData = m_ingame.GetColorData(
					searchTargetList[0].EnemyId,
					searchTargetList[0].ControllId)
				};
			}
			else
			{
				colorData = new game.outgame.GameUI.SearchTargetIconColor()
				{
					m_enemyId = -1,
					m_colorData = null
				};
			}
			m_outgame.UpdateSearchTarget(colorData, callback);
		}

		private void OnAppButtonPressed()
		{
			//if (m_gameMode == GameMode.None)
			//{
			//	m_gameMode = GameMode.App;
			//}
			//else
			//{
			//	m_gameMode = GameMode.None;
			//}
			//m_ingame.UpdateMode(m_gameMode);
			//m_outgame.UpdateMode(m_gameMode);
		}

		private void OnAppIconButtonPressed(int index)
		{
			if (m_gameMode != GameMode.App)
			{
				return;
			}
			UpdateOutgameObject();
		}

		private void OnInfoButtonPressed()
		{
		}
	}
}