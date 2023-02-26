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

		private game.Ingame m_ingame = null;
		private game.Outgame m_outgame = null;

		public void Initialize(
			Ingame ingame,
			Outgame outgame)
		{
			m_ingame = ingame;
			m_outgame = outgame;

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
			yield return m_ingame.ChangeMapCoroutine(stageId, dataIndex, null);
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
			//float fadeTime = 0.5f;
			//yield return FadeInCoroutine(fadeTime);
			//m_outgame.ChangeMovieUI();
			//yield return FadeOutCoroutine(fadeTime);

			bool isDone = false;
			m_ingame.UpdateMovieCamera(Ingame.ZoomType.In, 1.0f, true, () => { isDone = true; });
			while (!isDone) { yield return null; };

			//yield return FadeInCoroutine(fadeTime);
			//m_outgame.ChangeGameUI();
			//yield return FadeOutCoroutine(fadeTime);

			isDone = false;
			m_ingame.PlayReaction(ingame.IngameWorld.ReactionType.Restraint, enemyControllId, () => { isDone = true; });
			while (!isDone) { yield return null; };

			isDone = false;
			m_ingame.UpdateMovieCamera(Ingame.ZoomType.Normal, 1.0f, true, () => { isDone = true; });
			while (!isDone) { yield return null; };

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
