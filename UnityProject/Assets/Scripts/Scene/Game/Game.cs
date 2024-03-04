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
		public enum GenreType
		{
			Action,
			Puzzle,
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



		public override void Ready(UnityAction callback)
		{
			StartCoroutine(ReadyCoroutine(callback));
		}

		private IEnumerator ReadyCoroutine(UnityAction callback)
		{
			var local = GeneralRoot.User.LocalSaveData;
			var gameGunreMaster = GeneralRoot.Master.GameGunreData;
			var gameGunreMasterData = gameGunreMaster.Find(local.ChallengeGameGunreId);
			if (gameGunreMasterData == null)
			{
				yield break;
			}
			UpdateBudId(true);

			m_movieController.Initialize(OnMovieStart);

			m_ingame.Initialize(
				loadGameEvent: LoadGameEvent,
				playMovieEvent: OnMovieStart);
			m_outgame.Initialize(
				commonWindowPlayMovieEvent: (int movieId) =>
				{
					m_movieController.Play(movieId, null);
				},
				feedbackButtonEvent: () =>
				{
					m_movieController.Play((int)data.master.MovieListData.MovieType.Feedback, null);
				},
				powerButtonEvent: () =>
				{
					m_movieController.Play((int)data.master.MovieListData.MovieType.Recreate, () =>
					{
						var temporary = GeneralRoot.User.LocalTemporaryData;
						temporary.ClearSceneNameList.Clear();

						m_ingame.ResetGame(null);
					});
				},
				recreateButtonEvent: () =>
				{
					m_movieController.Play((int)data.master.MovieListData.MovieType.Recreate, () =>
					{
						var temporary = GeneralRoot.User.LocalTemporaryData;
						temporary.ClearSceneNameList.Clear();

						UpdateBudId(false);
						m_ingame.ResetGame(null);
					});
				},
				releaseButtonEvent: () =>
				{
					m_movieController.Play((int)data.master.MovieListData.MovieType.Release, null);
				},
				canselButtonEvent: () =>
				{
					m_movieController.Play((int)data.master.MovieListData.MovieType.Cansel, null);
				},
				mainWindowInputEvent: m_ingame.OnInputEvent);

			//GeneralRoot.Sound.PlayBGM(100);

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
			m_movieController.Play((int)data.master.MovieListData.MovieType.Title, () => { isDone = true; });
			while (!isDone) { yield return null; }

			var local = GeneralRoot.User.LocalSaveData;
			var gameGunreMaster = GeneralRoot.Master.GameGunreData;
			var gameGunreMasterData = gameGunreMaster.Find(local.ChallengeGameGunreId);
			if (gameGunreMasterData == null)
			{
				yield break;
			}
			isDone = false;
			m_movieController.Play(gameGunreMasterData.MovieDataId, () => { isDone = true; });
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

			m_sceneController.AddScene(
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
				case "EnableInput":
					{
						GeneralRoot.Instance.SetForeMostRayCast(false);
						if (callback != null)
						{
							callback();
						}
						break;
					}
				case "DisableInput":
					{
						GeneralRoot.Instance.SetForeMostRayCast(true);
						if (callback != null)
						{
							callback();
						}
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

		private void UpdateBudId(bool isInitialize)
		{
			var local = GeneralRoot.User.LocalSaveData;
			var gameGunreMaster = GeneralRoot.Master.GameGunreData;
			var gameGunreMasterData = gameGunreMaster.Find(local.ChallengeGameGunreId);
			if (gameGunreMasterData == null)
			{
				return;
			}

			var temporary = GeneralRoot.User.LocalTemporaryData;
			if (isInitialize == true)
			{
				temporary.UpdateTryCount(0);
			}
			else
			{
				temporary.UpdateTryCount(temporary.TryCount + 1);
			}

			bool isBug = false;
			if (temporary.TryCount < gameGunreMasterData.TryCountMin)
			{
				isBug = true;
			}
			else if (
				temporary.TryCount < gameGunreMasterData.TryCountMax &&
				UnityEngine.Random.Range(0, 2) == 0)
			{
				isBug = true;
			}

			if (isBug)
			{
				var bugIds = gameGunreMasterData.CheckSheetBugIds;
				int index = UnityEngine.Random.Range(0, bugIds.Length);
				temporary.UpdateOccurredBugId(bugIds[index]);
				switch (bugIds[index])
				{
					case (int)data.master.CheckSheetBugData.BugType.Animation:
						{
							temporary.UpdateOccurredBugOptionId(UnityEngine.Random.Range(0, 2));
							break;
						}
					default:
						{
							temporary.UpdateOccurredBugOptionId(0);
							break;
						}
				}
			}
			else
			{
				temporary.UpdateOccurredBugId(-1);
				temporary.UpdateOccurredBugOptionId(-1);
			}
			temporary.UpdateOccurredBugId(51);
			temporary.UpdateOccurredBugOptionId(1);
		}
	}
}