using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene
{
	public class Game : SceneBase
	{
		[Header("Game")]

		/// <summary>
		/// インゲーム用カメラ
		/// </summary>
		[SerializeField]
		private Camera m_ingameCamera;

		/// <summary>
		/// インゲーム用カメラ
		/// </summary>
		[SerializeField]
		private Camera m_ingame2Camera;

		/// <summary>
		/// インゲーム用カメラトランスフォーム
		/// </summary>
		[SerializeField]
		private Transform m_ingame2CameraTransform;

		/// <summary>
		/// インゲーム用カメラアニメーション
		/// </summary>
		[SerializeField]
		private AnimatorExpansion m_ingame2CameraAnimator;

		/// <summary>
		/// アウトゲーム用カメラ
		/// </summary>
		[SerializeField]
		private Camera m_outgameCamera;

		/// <summary>
		/// インゲーム管理
		/// </summary>
		[SerializeField]
		private game.Ingame m_ingame = null;

		/// <summary>
		/// アウトゲーム管理
		/// </summary>
		[SerializeField]
		private game.Outgame m_outgame = null;

		/// <summary>
		/// ムービー管理
		/// </summary>
		[SerializeField]
		private game.MovieController m_movieController = null;



		public override void Ready(UnityAction callback)
		{
			StartCoroutine(ReadyCoroutine(callback));
		}

		private IEnumerator ReadyCoroutine(UnityAction callback)
		{
			m_ingame2Camera.transform.SetPositionAndRotation(
				Vector3.zero,
				Quaternion.identity);

			m_movieController.Initialize(
				m_outgame.SetVisible,
				m_ingame.PlayMovieCamera,
				m_ingame.PlayMovieCharaReaction,
				m_outgame.PlayMovieQuestClearIn);

			var searchTargetList = GeneralRoot.User.LocalSaveData.SearchTargetList;
			searchTargetList.Clear();
			//for (int i = 0; i < 5; ++i)
			//{
			//	searchTargetList.Add(new data.UserData.LocalSaveData.SearchTargetData(
			//		enemyId: 30,
			//		controllId: i + 1));
			//}

			bool isDone = false;
			m_ingame.Initialize(
				ingameEvent: IngameEvent,
				ingameCameraParentTransform: m_ingame2CameraTransform,
				ingameCameraAnimator: m_ingame2CameraAnimator,
				loadMapEvent: LoadMapEvent,
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
				m_ingame2Camera,
				onCharaActionButtonEvent: OnCharaActionButtonPressed);

			UpdateOutgameObject();

			yield return ChangeMapCoroutine(1, 0, true, null);

			callback();
		}

		public override void Go()
		{
			m_outgame.Fade(false, 1.0f);
		}

		private void LoadMapEvent(
			int stageId,
			game.ingame.StageScene beforeStage,
			UnityAction<game.ingame.StageScene> addedEvent)
		{
			StartCoroutine(LoadMapEventCoroutine(stageId, beforeStage, addedEvent));
		}

		private IEnumerator LoadMapEventCoroutine(
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
			string[] eventType = eventParam.Split('_');
			switch (eventType[0])
			{
				//case "DropItem":
				//	{
				//		int id = int.Parse(eventType[1]);
				//		StartCoroutine(DropItemCoroutine(id, callback));
				//		break;
				//	}
				//case "GetItem":
				//	{
				//		int id = int.Parse(eventType[1]);
				//		StartCoroutine(GetItemCoroutine(id, callback));
				//		break;
				//	}
				case "ChangeMap":
					{
						int stageId = int.Parse(eventType[1]);
						int dataIndex = int.Parse(eventType[2]);
						StartCoroutine(ChangeMapCoroutine(stageId, dataIndex, false, callback));
						break;
					}
				case "OpenDialog":
					{
						string dialogName = eventType[1];
						int id = int.Parse(eventType[2]);
						OpenDialog(dialogName, id, callback);
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

		//private IEnumerator DropItemCoroutine(int id, UnityAction callback)
		//{
		//	yield return m_ingame.DropItemCoroutine(id);
		//	callback();
		//}

		//private IEnumerator GetItemCoroutine(int id, UnityAction callback)
		//{
		//	yield return m_ingame.GetItemCoroutine(id);
		//	callback();
		//}

		private IEnumerator ChangeMapCoroutine(int stageId, int dataIndex, bool isReady, UnityAction callback)
		{
			if (isReady == false)
			{
				m_outgame.SetVisible(game.Outgame.Target.None);
			}

			bool isDone = false;
			m_ingame.ChangeMap(stageId, dataIndex, isReady, () => { isDone = true; });
			while (!isDone) { yield return null; }

			m_outgame.SetVisible(game.Outgame.Target.Game);

			if (callback != null)
			{
				callback();
			}
		}

		private void OpenDialog(string dialogName, int id, UnityAction callback)
		{
			switch (dialogName)
			{
				case "QuestList":
					{
						StartCoroutine(OpenQuestListDialog(id, callback));
						break;
					}
				case "Shop":
					{
						StartCoroutine(OpenShopDialog(id, callback));
						break;
					}
				case "Customize":
					{
						StartCoroutine(OpenCustomizeDialog(callback));
						break;
					}
				default:
					{
						callback();
						break;
					}
			}
		}

		private IEnumerator OpenQuestListDialog(int questListId, UnityAction callback)
		{
			m_outgame.SetVisible(game.Outgame.Target.None);

			bool isDone = false;
			m_ingame.PlayMovieCamera(game.Ingame.ZoomType.Pull, () => { isDone = true; });
			while (!isDone) { yield return null; }

			// クエストデータ
			var quests = new List<dialog.QuestListDialog.Data.Quest>();
			for (int i = 0; i < 10; ++i)
			{
				var questState = (dialog.QuestListDialog.Data.Quest.State)(i % 3);
				quests.Add(new dialog.QuestListDialog.Data.Quest(
					title: string.Format("QUEST{0}", (i + 1)),
					info: string.Format("クエストじょうほう{0}", (i + i)),
					difficultyRank: (i + 1),
					questState: questState,
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
			m_ingame.PlayMovieCamera(game.Ingame.ZoomType.Normal, () => { isDone = true; });
			while (!isDone) { yield return null; }

			Debug.Log("OpenQuestListDialog = " + game.Outgame.Target.Game);
			m_outgame.SetVisible(game.Outgame.Target.Game);

			if (receiveQuestIndex >= 0)
			{
				var searchTargetList = GeneralRoot.User.LocalSaveData.SearchTargetList;
				searchTargetList.Add(new data.UserData.LocalSave.SearchTargetData(
					30,
					7));

				isDone = false;
				UpdateOutgameSearchTarget(() => { isDone = true; });
				while (!isDone) { yield return null; }
			}

			callback();
		}

		private IEnumerator OpenShopDialog(int shopId, UnityAction callback)
		{
			bool isDone = false;
			m_sceneController.AddScene<dialog.ShopDialog>(
				added: (s) =>
				{
					s.Setting(null, () => { isDone = true; });
				});
			while (!isDone) { yield return null; }

			callback();
		}

		private IEnumerator OpenCustomizeDialog(UnityAction callback)
		{
			bool isDone = false;
			m_sceneController.AddScene<dialog.CustomizeDialog>(
				added: (s) =>
				{
					s.Setting(null, () => { isDone = true; });
				});
			while (!isDone) { yield return null; }

			callback();
		}

		private void RemoveTarget(int enemyId, int controllId, UnityAction callback)
		{
			StartCoroutine(RemoveTargetCoroutine(enemyId, controllId, callback));
		}

		private IEnumerator RemoveTargetCoroutine(int enemyId, int controllId, UnityAction callback)
		{
			var searchTargetList = GeneralRoot.User.LocalSaveData.SearchTargetList;
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
			m_outgame.SetVisible(game.Outgame.Target.None);

			bool isDone = false;
			m_ingame.OnCharaActionButtonPressed(controllId, () => { isDone = true; });
			while (!isDone) { yield return null; }

			m_outgame.SetVisible(game.Outgame.Target.Game);

			m_ingame.CheckEnemyActionEvent(controllId);
		}

		private void UpdateOutgameObject()
		{
			UpdateOutgameSearchTarget(null);
		}

		private void UpdateOutgameSearchTarget(UnityAction callback)
		{
			var searchTargetList = GeneralRoot.User.LocalSaveData.SearchTargetList;
			game.outgame.GameUI.SearchTargetIconColor iconColorData = default;
			if (searchTargetList.Count > 0)
			{
				var enemyColorResource = GeneralRoot.Resource.EnemyColorResource;
				data.resource.EnemyColorResource.Data.ColorData colorData = null;
				int enemyId = searchTargetList[0].EnemyId;
				var enemyData = enemyColorResource.Find(enemyId);
				if (enemyData != null)
				{
					int controllId = searchTargetList[0].ControllId;
					colorData = enemyData.Find(controllId);
				}
				iconColorData = new game.outgame.GameUI.SearchTargetIconColor()
				{
					m_enemyId = enemyId,
					m_colorData = colorData
				};
			}
			else
			{
				iconColorData = new game.outgame.GameUI.SearchTargetIconColor()
				{
					m_enemyId = -1,
					m_colorData = null
				};
			}
			m_outgame.UpdateSearchTarget(iconColorData, callback);
		}
	}
}