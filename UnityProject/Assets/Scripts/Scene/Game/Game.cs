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
				changeMapEvent: ChangeMapEvent,
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

		private void ChangeMapEvent(
			int stageId,
			game.ingame.StageScene beforeStage,
			UnityAction<game.ingame.StageScene> addedEvent)
		{
			StartCoroutine(ChangeMapEventCoroutine(stageId, beforeStage, addedEvent));
		}

		private IEnumerator ChangeMapEventCoroutine(
			int stageId,
			game.ingame.StageScene beforeStage,
			UnityAction<game.ingame.StageScene> addedEvent)
		{
			bool isDone = false;
			if (beforeStage != null)
			{
				m_sceneController.RemoveScene(beforeStage, () => { isDone = true; });
				while (!isDone) { yield return null; }
			}

			m_sceneController.AddScene<game.ingame.StageScene>(
				sceneName: string.Format("Stage{0:000}", stageId),
				added: addedEvent);
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
						StartCoroutine(ChangeMapCoroutine(mapId, dataIndex, callback));
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
				case "Shop":
					{
						int shopId = int.Parse(eventType[1]);
						Shop(shopId, callback);
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

		private IEnumerator ChangeMapCoroutine(int mapId, int dataIndex, UnityAction callback)
		{
			m_outgame.SetVisible(game.Outgame.Target.None);

			yield return m_movieController.ChangeMapCoroutine(mapId, dataIndex, false, false, callback);

			m_outgame.SetVisible(game.Outgame.Target.Game);

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
			//var searchTargetList = GeneralRoot.Instance.UserData.Data.SearchTargetList;
			//searchTargetList.Add(new data.UserData.LocalSaveData.SearchTargetData(
			//	enemyId,
			//	controllId));
			//UpdateOutgameSearchTarget(callback);
			StartCoroutine(Test(enemyId, controllId, callback));
		}

		private IEnumerator Test(int enemyId, int controllId, UnityAction callback)
		{
			m_outgame.SetVisible(game.Outgame.Target.None);

			bool isDone = false;
			m_ingame.UpdateMovieCamera(game.Ingame.ZoomType.Out, 0.5f, false, () => { isDone = true; });
			while (!isDone) { yield return null; }

			// クエストデータ
			var quests = new List<dialog.QuestListDialog.Data.Quest>();
			for (int i = 0; i < 10; ++i)
			{
				quests.Add(new dialog.QuestListDialog.Data.Quest(
					title: string.Format("QUEST{0}", (i + 1)),
					info: string.Format("クエストじょうほう{0}", (i + i)),
					difficultyRank: (i + 1),
					questState: dialog.QuestListDialog.Data.Quest.State.Unprogressed,
					rewards: null));
			}
			int receiveQuestIndex = -1;
			var data = new dialog.QuestListDialog.Data(
				title: "QUEST LIST",
				quests: quests.ToArray(),
				receiveEvent: (index) => { receiveQuestIndex = index; });

			isDone = false;
			m_sceneController.AddScene<dialog.QuestListDialog>(
				added: (s) =>
				{
					s.Setting(data, () => { isDone = true; });
				});
			while (!isDone) { yield return null; }

			isDone = false;
			m_ingame.UpdateMovieCamera(game.Ingame.ZoomType.Normal, 0.25f, false, () => { isDone = true; });
			while (!isDone) { yield return null; }

			m_outgame.SetVisible(game.Outgame.Target.Game);

			if (receiveQuestIndex >= 0)
			{
				var searchTargetList = GeneralRoot.Instance.UserData.Data.SearchTargetList;
				searchTargetList.Add(new data.UserData.LocalSaveData.SearchTargetData(
					enemyId,
					controllId));

				isDone = false;
				UpdateOutgameSearchTarget(() => { isDone = true; });
				while (!isDone) { yield return null; }
			}

			callback();
		}

		private void RemoveTarget(int enemyId, int controllId, UnityAction callback)
		{
			StartCoroutine(RemoveTargetCoroutine(enemyId, controllId, callback));
		}

		private void Shop(int shopId, UnityAction callback)
		{
			m_sceneController.AddScene<dialog.ShopDialog>(
				added: (s) =>
				{
				});
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
			StartCoroutine(OnCharaActionButtonPressedCoroutine(controllId));
		}

		private IEnumerator OnCharaActionButtonPressedCoroutine(int controllId)
		{
			m_ingame.SetIsEventLock(true);
			m_outgame.SetVisible(game.Outgame.Target.None);

			bool isDone = false;
			m_ingame.OnCharaActionButtonPressed(controllId, () => { isDone = true; });
			while (!isDone) { yield return null; }

			m_outgame.SetVisible(game.Outgame.Target.Game);
			m_ingame.SetIsEventLock(false);
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