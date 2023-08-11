using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene
{
	public class Game : SceneBase
	{
		public class NpcReleaseData
		{
			public class Data
			{
				private int m_npcId;
				public int NPCId => m_npcId;

				private List<int> m_colorIdList = new List<int>();
				public List<int> ColorIdList => m_colorIdList;

				public Data(int npcId, int colorId)
				{
					m_npcId = npcId;
					m_colorIdList.Add(colorId);
				}
			}

			private List<Data> m_dataList = new List<Data>();

			public bool IsRelease(game.ingame.world.NPC npc)
			{
				var data = Find(npc.NPCId);
				if (data == null)
				{
					return false;
				}
				if (data.ColorIdList.Contains(npc.ColorId) == false)
				{
					return false;
				}
				return true;
			}

			public Data Find(int npcId)
			{
				return m_dataList.Find(d => d.NPCId == npcId);
			}

			public void Add(int npcId, int colorId)
			{
				m_dataList.Add(new Data(npcId, colorId));
			}
		}

		[Header("Game")]

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



		private NpcReleaseData m_npcReleaseData = new NpcReleaseData();



		public override void Ready(UnityAction callback)
		{
			StartCoroutine(ReadyCoroutine(callback));
		}

		private IEnumerator ReadyCoroutine(UnityAction callback)
		{
			m_movieController.Initialize(OnMovieStart);

			bool isDone = false;
			m_ingame.Initialize(
				ingameEvent: IngameEvent,
				loadMapEvent: LoadMapEvent,
				updateCharaActionButtonEvent: UpdateCharaActionButton,
				() => { isDone = true; });
			m_outgame.Initialize(
				charaActionButtonEvent: OnCharaActionButtonPressed,
				cameraBeginMoveEvent: m_ingame.CameraBeginMoveEvent,
				cameraMoveEvent: m_ingame.CameraMoveEvent,
				cameraEndMoveEvent: m_ingame.CameraEndMoveEvent,
				charaBeginMoveEvent: m_ingame.CharaBeginMoveEvent,
				charaMoveEvent: m_ingame.CharaMoveEvent,
				charaEndMoveEvent: m_ingame.CharaEndMoveEvent,
				cameraZoomEvent: m_ingame.CameraZoomEvent);

			while (!isDone) { yield return null; }

			callback();
		}

		public override void Go()
		{
			StartCoroutine(GoCoroutine());
		}

		private IEnumerator GoCoroutine()
		{
			yield return ChangeMapCoroutine(1, 0, true, null);

			bool isDone = false;
			m_outgame.Fade(false, 1.0f, () => { isDone = true; });
			while (!isDone) { yield return null; }

			m_ingame.Go();
			m_outgame.Go();

			isDone = false;
			m_movieController.Play(1, () => { isDone = true; });
			while (!isDone) { yield return null; }
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
				case "AddTarget":
					{
						//int subCharaId = int.Parse(eventType[1]);
						//int controllId = int.Parse(eventType[2]);
						//StartCoroutine(AddTargetCoroutine(subCharaId, controllId, callback));
						break;
					}
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
						int id = (eventType.Length > 2) ? int.Parse(eventType[2]) : -1;
						OpenDialog(dialogName, id, callback);
						break;
					}
			}
		}

		private IEnumerator ChangeMapCoroutine(int stageId, int dataIndex, bool isReady, UnityAction callback)
		{
			if (isReady == false)
			{
				//m_outgame.SetVisible(game.Outgame.Target.None);
			}

			bool isDone = false;
			m_ingame.ChangeMap(stageId, dataIndex, isReady, () => { isDone = true; });
			while (!isDone) { yield return null; }

			//m_outgame.SetVisible(game.Outgame.Target.Game);

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
						//StartCoroutine(OpenCommonDialog("テストタイトル", "テストメッセージ", new string[] { "テスト１", "テスト２" }, callback));
						//StartCoroutine(OpenQuestListDialog(id, callback));
						break;
					}
				case "Shop":
					{
						//StartCoroutine(OpenShopDialog(id, callback));
						break;
					}
				case "Customize":
					{
						//StartCoroutine(OpenCustomizeDialog(callback));
						break;
					}
				default:
					{
						callback();
						break;
					}
			}
		}

		private IEnumerator OpenCommonDialog(string title, string message, string[] buttonNames, UnityAction callback)
		{
			bool isDone = false;
			var data = dialog.CommonDialog.Data.CreateYesNoData(title, message, buttonNames, new UnityAction[] { null, null });
			m_sceneController.AddScene<dialog.CommonDialog>(
				"CommonDialog",
				added: (s) =>
				{
					s.Setting(data, () => { isDone = true; });
				});
			while (!isDone) { yield return null; }

			callback();
		}

		private void UpdateCharaActionButton(game.ingame.IngameWorld.SearchInData data)
		{
			if (data != null)
			{
				var buttonData = new game.outgame.window.MainWindow.CharaActionButtonData(
					data.Category,
					data.ControllId,
					game.outgame.window.CharaActionButtonElement.ActionType.Active);
				m_outgame.UpdateCharaActionButton(buttonData);
			}
			else
			{
				m_outgame.UpdateCharaActionButton(null);
			}
		}

		private void OnCharaActionButtonPressed(game.ingame.world.ActionTargetBase.Category category, int controllId)
		{
			StartCoroutine(OnCharaActionButtonPressedCoroutine(category, controllId));
		}

		private IEnumerator OnCharaActionButtonPressedCoroutine(game.ingame.world.ActionTargetBase.Category category, int controllId)
		{
			bool isDone = false;
			m_ingame.OnCharaActionButtonPressed(category, controllId, () => { isDone = true; });
			while (!isDone) { yield return null; }

			if (category == game.ingame.world.ActionTargetBase.Category.NPC)
			{
				var npc = m_ingame.GetNPC(controllId);
				//if (m_npcReleaseData.IsRelease(npc) == true)
				{
					yield return m_ingame.DeleteNPCCoroutine(controllId);
				}
			}
		}

		private void OnMovieStart(string param, UnityAction callback)
		{
			string[] paramStrings = param.Split(',');
			switch (paramStrings[0])
			{
				case "WaitTime":
					{
						float time = float.Parse(paramStrings[1]);
						StartCoroutine(WaitTimeCoroutine(time, callback));
						break;
					}
				case "Data":
					{
						paramStrings = paramStrings.Skip(1).ToArray();
						UpdateData(paramStrings);
						if (callback != null)
						{
							callback();
						}
						break;
					}
				case "Ingame":
					{
						paramStrings = paramStrings.Skip(1).ToArray();
						m_ingame.OnMovieStart(paramStrings, callback);
						break;
					}
				case "Outgame":
					{
						paramStrings = paramStrings.Skip(1).ToArray();
						m_outgame.OnMovieStart(paramStrings, callback);
						break;
					}
				default:
					{
						if (callback != null)
						{
							callback();
						}
						break;
					}
			}
		}

		private IEnumerator WaitTimeCoroutine(float time, UnityAction callback)
		{
			yield return new WaitForSeconds(time);
			if (callback != null)
			{
				callback();
			}
		}

		private void UpdateData(string[] paramStrings)
		{
			switch (paramStrings[0])
			{
				case "AddReleaseData":
					{
						int npcId = int.Parse(paramStrings[1]);
						int colorId = int.Parse(paramStrings[2]);
						var data = m_npcReleaseData.Find(npcId);
						if (data != null)
						{
							data.ColorIdList.Add(colorId);
						}
						else
						{
							m_npcReleaseData.Add(npcId, colorId);
						}
						break;
					}
			}
		}
	}
}