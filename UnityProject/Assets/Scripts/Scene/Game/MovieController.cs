using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene.game
{
	public class MovieController : MonoBehaviour
	{
		[SerializeField]
		private Image m_fade = null;



		private UnityAction<Outgame.Target> m_outgameSetVisibleEvent;
		private UnityAction<int, int, UnityAction> m_ingameChangeMapEvent;
		private UnityAction<Ingame.ZoomType, float, UnityAction> m_ingamePlayMovieCameraEvent;
		private UnityAction<ingame.IngameWorld.ReactionType, int, UnityAction> m_ingamePlayMovieCharaReactionEvent;
		private UnityAction<int, UnityAction> m_outgamePlayMovieQuestClearInEvent;



		public void Initialize(
			UnityAction<Outgame.Target> outgameSetVisibleEvent,
			UnityAction<int, int, UnityAction> ingameChangeMapEvent,
			UnityAction<Ingame.ZoomType, float, UnityAction> ingamePlayMovieCameraEvent,
			UnityAction<ingame.IngameWorld.ReactionType, int, UnityAction> ingamePlayMovieCharaReactionEvent,
			UnityAction<int, UnityAction> outgamePlayMovieQuestClearInEvent)
		{
			m_outgameSetVisibleEvent = outgameSetVisibleEvent;
			m_ingameChangeMapEvent = ingameChangeMapEvent;
			m_ingamePlayMovieCameraEvent = ingamePlayMovieCameraEvent;
			m_ingamePlayMovieCharaReactionEvent = ingamePlayMovieCharaReactionEvent;
			m_outgamePlayMovieQuestClearInEvent = outgamePlayMovieQuestClearInEvent;

			var fadeColor = m_fade.color;
			fadeColor.a = 1.0f;
			m_fade.color = fadeColor;
		}
		public IEnumerator ChangeMapCoroutine(
			int stageId,
			int dataIndex,
			bool isFadeIn,
			bool isFadeOut,
			UnityAction callback)
		{
			if (isFadeIn == true)
			{
				yield return FadeInCoroutine(1.0f);
			}

			bool isDone = false;
			m_ingameChangeMapEvent(stageId, dataIndex, () => { isDone = true; });
			while (!isDone) { yield return null; }

			if (isFadeOut == true)
			{
				yield return FadeOutCoroutine(1.0f);
			}
			if (callback != null)
			{
				callback();
			}
		}

		public void Play(int movieId, int enemyControllId, UnityAction callback)
		{
			StartCoroutine(PlayCoroutine(movieId, enemyControllId, callback));
		}

		private IEnumerator PlayCoroutine(int movieId, int enemyControllId, UnityAction callback)
		{
			m_outgameSetVisibleEvent(Outgame.Target.None);

			int doneCount = 0;
			m_ingamePlayMovieCameraEvent(Ingame.ZoomType.In, 1.0f, () => { doneCount++; });
			while (doneCount < 1) { yield return null; };

			doneCount = 0;
			m_ingamePlayMovieCharaReactionEvent(ingame.IngameWorld.ReactionType.Restraint, enemyControllId, () => { doneCount++; });
			while (doneCount < 1) { yield return null; };

			m_outgameSetVisibleEvent(Outgame.Target.Movie);

			doneCount = 0;
			m_ingamePlayMovieCameraEvent(Ingame.ZoomType.QuestClear, 0.5f, () => { doneCount++; });
			m_ingamePlayMovieCharaReactionEvent(ingame.IngameWorld.ReactionType.Delight, enemyControllId, () => { doneCount++; });
			m_outgamePlayMovieQuestClearInEvent(0, () => { doneCount++; });
			while (doneCount < 3) { yield return null; };

			doneCount = 0;
			m_ingamePlayMovieCameraEvent(Ingame.ZoomType.Normal, 1.0f, () => { doneCount++; });
			while (doneCount < 1) { yield return null; };

			m_outgameSetVisibleEvent(Outgame.Target.Game);

			if (callback != null)
			{
				callback();
			}
		}

		private IEnumerator FadeInCoroutine(float fadeTime)
		{
			float nowTime = 0.0f;
			while (nowTime < fadeTime)
			{
				nowTime += Time.deltaTime;
				if (nowTime > fadeTime)
				{
					nowTime = fadeTime;
				}

				var fadeColor = m_fade.color;
				fadeColor.a = (nowTime / fadeTime);
				m_fade.color = fadeColor;

				yield return null;
			}
		}

		private IEnumerator FadeOutCoroutine(float fadeTime)
		{
			float nowTime = 0.0f;
			while (nowTime < fadeTime)
			{
				nowTime += Time.deltaTime;
				if (nowTime > fadeTime)
				{
					nowTime = fadeTime;
				}
				var fadeColor = m_fade.color;
				fadeColor.a = 1.0f - (nowTime / fadeTime);
				m_fade.color = fadeColor;

				yield return null;
			}
		}
	}
}
