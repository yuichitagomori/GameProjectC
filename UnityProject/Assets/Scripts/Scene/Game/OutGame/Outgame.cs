using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

namespace scene.game
{
	[System.Serializable]
	public class Outgame : MonoBehaviour
	{
		[SerializeField]
		private Image m_fade;

		[SerializeField]
		private outgame.GameUI m_gameUI;



		public void Initialize(
			UnityAction<int> commonWindowPlayMovieEvent,
			UnityAction feedbackButtonEvent,
			UnityAction powerButtonEvent,
			UnityAction recreateButtonEvent,
			UnityAction releaseButtonEvent,
			UnityAction canselButtonEvent,
			UnityAction<KeyCode[]> mainWindowInputEvent)
		{
			var fadeColor = m_fade.color;
			fadeColor.a = 1.0f;
			m_fade.color = fadeColor;

			m_gameUI.Initialize(
				commonWindowPlayMovieEvent: commonWindowPlayMovieEvent,
				feedbackButtonEvent: feedbackButtonEvent,
				powerButtonEvent: powerButtonEvent,
				recreateButtonEvent: recreateButtonEvent,
				releaseButtonEvent: releaseButtonEvent,
				canselButtonEvent: canselButtonEvent,
				mainWindowInputEvent: mainWindowInputEvent);
		}

		public void Go()
		{
			m_gameUI.Go();
		}

		private void SetupFade(bool isIn, float time, UnityAction callback)
		{
			StartCoroutine(SetupFadeCoroutine(isIn, time, callback));
		}

		private IEnumerator SetupFadeCoroutine(bool isIn, float time, UnityAction callback)
		{
			float nowTime = 0.0f;
			while (nowTime < time)
			{
				nowTime += Time.deltaTime;
				if (nowTime > time)
				{
					nowTime = time;
				}

				var fadeColor = m_fade.color;
				float alpha = (isIn) ? 1.0f - (nowTime / time) : (nowTime / time);
				fadeColor.a = alpha;
				m_fade.color = fadeColor;

				yield return null;
			}

			if (callback != null)
			{
				callback();
			}
		}

		

		public void OnMovieStart(string[] paramStrings, UnityAction callback)
		{
			switch (paramStrings[0])
			{
				case "FadeIn":
					{
						float time = float.Parse(paramStrings[1]);
						SetupFade(true, time, callback);
						break;
					}
				case "FadeOut":
					{
						float time = float.Parse(paramStrings[1]);
						SetupFade(false, time, callback);
						break;
					}
				default:
					{
						m_gameUI.OnMovieStart(paramStrings, callback);
						break;
					}
			}
		}
    }
}
