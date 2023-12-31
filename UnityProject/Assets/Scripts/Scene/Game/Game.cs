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
				public class Color
				{
					private int m_colorId;
					public int ColorId => m_colorId;

					private int m_count;
					public int Count => m_count;

					private bool m_isEnemy;
					public bool IsEnemy => m_isEnemy;

					public Color(int colorId, int count, bool isEnemy)
					{
						m_colorId = colorId;
						m_count = count;
						m_isEnemy = isEnemy;
					}

					public void AddCount(int count)
					{
						m_count += count;
					}

					public void UpdateIsEnemy(bool isEnemy)
					{
						m_isEnemy = isEnemy;
					}
				}

				private int m_npcId;
				public int NPCId => m_npcId;

				private List<Color> m_colorList = new List<Color>();
				

				public Data(int npcId, int colorId, int count, bool isEnemy)
				{
					m_npcId = npcId;
					m_colorList.Add(new Color(colorId, count, isEnemy));
				}

				public void Add(int colorId, int count, bool isEnemy)
				{
					var color = m_colorList.Find(d => d.ColorId == colorId);
					if (color != null)
					{
						color.AddCount(count);
						color.UpdateIsEnemy(isEnemy);
					}
					else
					{
						m_colorList.Add(new Color(colorId, count, isEnemy));
					}
				}

				public bool IsEnemy(int colorId)
				{
					var color = m_colorList.Find(d => d.ColorId == colorId);
					if (color == null)
					{
						return false;
					}
					if (color.Count <= 0)
					{
						return false;
					}
					return color.IsEnemy;
				}

				public int GetEnemyTotalCount()
				{
					int totalCount = 0;
					for (int i = 0; i < m_colorList.Count; ++i)
					{
						if (m_colorList[i].IsEnemy == false)
						{
							continue;
						}
						totalCount += m_colorList[i].Count;
					}
					return totalCount;
				}
			}

			private List<Data> m_dataList = new List<Data>();



			public void Add(int npcId, int colorId, int count, bool isEnemy)
			{
				var data = m_dataList.Find(d => d.NPCId == npcId);
				if (data != null)
				{
					data.Add(colorId, count, isEnemy);
				}
				else
				{
					m_dataList.Add(new Data(npcId, colorId, count, isEnemy));
				}
			}

			public bool IsEnemy(game.ingame.world.NPC npc)
			{
				var data = m_dataList.Find(d => d.NPCId == npc.NPCId);
				if (data == null)
				{
					return false;
				}
				return data.IsEnemy(npc.ColorId);
			}

			public int GetEnemyTotalCount()
            {
                int totalCount = 0;
                for (int i = 0; i < m_dataList.Count; ++i)
                {
					totalCount += m_dataList[i].GetEnemyTotalCount();
				}
                return totalCount;
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

			//bool isDone = false;
			m_ingame.Initialize(
				loadGameEvent: LoadGameEvent,
				outgameSetupEvent: m_outgame.SetupEvent);
			m_outgame.Initialize(
				uploadButtonEvent: () =>
				{
					m_movieController.Play(3, null);
				},
				commonWindowPlayMovieEvent: (int movieId) =>
				{
					m_movieController.Play(movieId, null);
				},
				mainWindowPowerButtonEvent: m_ingame.ResetGame,
				mainWindowInputEvent: m_ingame.OnInputEvent);

			//while (!isDone) { yield return null; }

			yield return null;

			callback();
		}

		public override void Go()
		{
			StartCoroutine(GoCoroutine());
		}

		private IEnumerator GoCoroutine()
		{
			m_ingame.Go();
			m_outgame.Go();

			bool isDone = false;
			m_movieController.Play(1, () => { isDone = true; });
			while (!isDone) { yield return null; }

			isDone = false;
			m_movieController.Play(2, () => { isDone = true; });
			while (!isDone) { yield return null; }
		}

		private void LoadGameEvent(
			string gameGenreName,
			game.ingame.GameGenreBase beforeGameGenre,
			UnityAction<game.ingame.GameGenreBase> addedEvent)
		{
			StartCoroutine(LoadGameEventCoroutine(gameGenreName, beforeGameGenre, addedEvent));
		}

		private IEnumerator LoadGameEventCoroutine(
			string gameGenreName,
			game.ingame.GameGenreBase beforeGameGenre,
			UnityAction<game.ingame.GameGenreBase> addedEvent)
		{
			bool isDone = false;
			if (beforeGameGenre != null)
			{
				m_sceneController.RemoveScene(beforeGameGenre, () => { isDone = true; });
				while (!isDone) { yield return null; }
			}

			m_sceneController.AddScene<game.ingame.GameGenreBase>(
				sceneName: gameGenreName,
				added: addedEvent);
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
						Debug.LogError("Game.cs OnMovieStart ErrorCommand");
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
                        int count = int.Parse(paramStrings[3]);
                        bool isEnemy = bool.Parse(paramStrings[4]);
						m_npcReleaseData.Add(npcId, colorId, count, isEnemy);
						break;
					}
				default:
					{
						Debug.LogError("Game.cs UpdateData ErrorCommand");
						break;
					}
			}
		}
	}
}