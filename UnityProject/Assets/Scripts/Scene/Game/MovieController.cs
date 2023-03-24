using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene.game
{
	public class MovieController : MonoBehaviour
	{
		private UnityAction<Outgame.Target> m_outgameSetVisibleEvent;
		private UnityAction<Ingame.ZoomType, UnityAction> m_ingamePlayMovieCameraEvent;
		private UnityAction<ingame.IngameWorld.ReactionType, int, UnityAction> m_ingamePlayMovieCharaReactionEvent;
		private UnityAction<int, UnityAction> m_outgamePlayMovieQuestClearInEvent;



		public void Initialize(
			UnityAction<Outgame.Target> outgameSetVisibleEvent,
			UnityAction<Ingame.ZoomType, UnityAction> ingamePlayMovieCameraEvent,
			UnityAction<ingame.IngameWorld.ReactionType, int, UnityAction> ingamePlayMovieCharaReactionEvent,
			UnityAction<int, UnityAction> outgamePlayMovieQuestClearInEvent)
		{
			m_outgameSetVisibleEvent = outgameSetVisibleEvent;
			m_ingamePlayMovieCameraEvent = ingamePlayMovieCameraEvent;
			m_ingamePlayMovieCharaReactionEvent = ingamePlayMovieCharaReactionEvent;
			m_outgamePlayMovieQuestClearInEvent = outgamePlayMovieQuestClearInEvent;
		}

		public void Play(int movieId, int enemyControllId, UnityAction callback)
		{
			StartCoroutine(PlayCoroutine(movieId, enemyControllId, callback));
		}

		private IEnumerator PlayCoroutine(int movieId, int enemyControllId, UnityAction callback)
		{
			m_outgameSetVisibleEvent(Outgame.Target.None);

			int doneCount = 0;
			m_ingamePlayMovieCameraEvent(Ingame.ZoomType.Approach, () => { doneCount++; });
			while (doneCount < 1) { yield return null; };

			doneCount = 0;
			m_ingamePlayMovieCharaReactionEvent(ingame.IngameWorld.ReactionType.Restraint, enemyControllId, () => { doneCount++; });
			while (doneCount < 1) { yield return null; };

			m_outgameSetVisibleEvent(Outgame.Target.Movie);

			doneCount = 0;
			m_ingamePlayMovieCameraEvent(Ingame.ZoomType.QuestClear, () => { doneCount++; });
			while (doneCount < 1) { yield return null; };

			doneCount = 0;
			m_ingamePlayMovieCharaReactionEvent(ingame.IngameWorld.ReactionType.DelightIn, enemyControllId, () => { doneCount++; });
			m_outgamePlayMovieQuestClearInEvent(0, () => { doneCount++; });
			while (doneCount < 2) { yield return null; };

			doneCount = 0;
			m_ingamePlayMovieCameraEvent(Ingame.ZoomType.Normal, () => { doneCount++; });
			while (doneCount < 1) { yield return null; };

			m_outgameSetVisibleEvent(Outgame.Target.Game);

			if (callback != null)
			{
				callback();
			}
		}
	}
}
