using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game
{
	[System.Serializable]
	public class Ingame : MonoBehaviour
	{
		[SerializeField]
		private Material m_backgroundMaterial;

		[SerializeField]
		private Common.AnimatorExpansion m_aiCharaAnimation;







		private UnityAction<string, ingame.GameGenreBase, UnityAction<ingame.GameGenreBase>> m_loadGameEvent;

		private ingame.GameGenreBase m_gameGenre;

		private UnityAction<string, UnityAction> m_outgameSetupEvent;

		private float m_backgroundScrollSpeed;

		private float m_backgroundScrollValue;



		public void Initialize(
			UnityAction<string, ingame.GameGenreBase, UnityAction<ingame.GameGenreBase>> loadGameEvent,
			UnityAction<string, UnityAction> outgameSetupEvent)
		{
			m_loadGameEvent = loadGameEvent;
			m_outgameSetupEvent = outgameSetupEvent;

			m_backgroundMaterial.SetFloat("_Scroll", 0.0f);
			m_backgroundMaterial.SetFloat("_Distortion", 0.0f);
			m_backgroundScrollSpeed = 4.0f;
			m_backgroundScrollValue = 0.0f;
			StartCoroutine(UpdateBackgroundScroll());
		}

		public void Go()
		{
			m_aiCharaAnimation.PlayLoop("Wait");
		}

		public void OnMovieStart(string[] paramStrings, UnityAction callback)
		{
			switch (paramStrings[0])
			{
				case "BackgroundFast":
					{
						float time = float.Parse(paramStrings[1]);
						SetupBackground(true, time, callback);
						break;
					}
				case "BackgroundSlow":
					{
						float time = float.Parse(paramStrings[1]);
						SetupBackground(false, time, callback);
						break;
					}
				case "StartGame":
					{
						string sceneName = paramStrings[1];
						StartGame(sceneName, ingame.GameGenreBase.State.None, callback);
						break;
					}
				default:
					{
						break;
					}
			}
		}

		private void StartGame(string sceneName, ingame.GameGenreBase.State state, UnityAction callback)
		{
			m_loadGameEvent(sceneName, m_gameGenre, (ingame.GameGenreBase newGameGenre) =>
			{
				m_gameGenre = newGameGenre;
				m_gameGenre.Setting(
					state: state,
					changeGameEvent: (sceneName) =>
					{
						StartGame(sceneName, ingame.GameGenreBase.State.Game, null);
					},
					outgameSetupEvent: m_outgameSetupEvent);
				if (callback != null)
				{
					callback();
				}
			});
		}

		public void ResetGame()
		{
			StartGame(m_gameGenre.name, ingame.GameGenreBase.State.None, null);
		}

		public void OnInputEvent(KeyCode[] pressKeys)
		{
			if (m_gameGenre == null)
			{
				return;
			}
			m_gameGenre.OnInputEvent(pressKeys);
		}
		private void SetupBackground(bool isFast, float time, UnityAction callback)
		{
			StartCoroutine(SetupBackgroundCoroutine(isFast, time, callback));
		}

		private IEnumerator SetupBackgroundCoroutine(bool isFast, float time, UnityAction callback)
		{
			float nowTime = 0.0f;
			while (nowTime < time)
			{
				nowTime += Time.deltaTime;
				if (nowTime > time)
				{
					nowTime = time;
				}
				m_backgroundScrollSpeed = (isFast) ?
					3.0f * (nowTime / time) + 1.0f :
					3.0f * (1.0f - (nowTime / time)) + 1.0f;
				float distortionValue = (isFast) ?
					0.5f * (1.0f - (nowTime / time)) :
					0.5f * (nowTime / time);
				m_backgroundMaterial.SetFloat("_Distortion", distortionValue);

				yield return null;
			}

			if (callback != null)
			{
				callback();
			}

		}

		private IEnumerator UpdateBackgroundScroll()
		{
			while (true)
			{
				m_backgroundScrollValue += Time.deltaTime * m_backgroundScrollSpeed;
				m_backgroundScrollValue = m_backgroundScrollValue % 1.0f;
				m_backgroundMaterial.SetFloat("_Scroll", m_backgroundScrollValue);

				yield return null;
			}
		}
	}
}