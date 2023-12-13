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
		public enum Target
		{
			None,
			Game,
			Movie,
		}

		[SerializeField]
		private Image m_fade;

		[SerializeField]
		private outgame.GameUI m_gameUI;

		[SerializeField]
		private Material m_backgroundMaterial;



		private float m_backgroundScrollSpeed;

		private float m_backgroundScrollValue;



		//public void Initialize(
		//	UnityAction<ingame.world.ActionTargetBase.Category, int> charaActionButtonEvent,
		//	UnityAction<Vector2> cameraBeginMoveEvent,
		//	UnityAction<Vector2> cameraMoveEvent,
		//	UnityAction cameraEndMoveEvent,
		//	UnityAction<KeyCode[]> inputEvent)
		//{
		//	var fadeColor = m_fade.color;
		//	fadeColor.a = 1.0f;
		//	m_fade.color = fadeColor;

		//	m_gameUI.Initialize(
		//		charaActionButtonEvent: charaActionButtonEvent,
		//		cameraBeginMoveEvent: cameraBeginMoveEvent,
		//		cameraMoveEvent: cameraMoveEvent,
		//		cameraEndMoveEvent: cameraEndMoveEvent,
		//		inputEvent: inputEvent,
		//		clickEvent: clickEvent,
		//		cameraZoomEvent: cameraZoomEvent);
		//}

		public void Initialize(
			UnityAction uploadButtonEvent,
			UnityAction<int> commonWindowPlayMovieEvent,
			UnityAction mainWindowPowerButtonEvent,
			UnityAction<KeyCode[]> mainWindowInputEvent)
		{
			var fadeColor = m_fade.color;
			fadeColor.a = 1.0f;
			m_fade.color = fadeColor;

			m_backgroundMaterial.SetFloat("_Scroll", 0.0f);
			m_backgroundMaterial.SetFloat("_Distortion", 0.0f);
			m_backgroundScrollSpeed = 4.0f;
			m_backgroundScrollValue = 0.0f;
			StartCoroutine(UpdateBackgroundScroll());

			m_gameUI.Initialize(
				uploadButtonEvent: uploadButtonEvent,
				commonWindowPlayMovieEvent: commonWindowPlayMovieEvent,
				mainWindowPowerButtonEvent: mainWindowPowerButtonEvent,
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
					0.5f * (1.0f - (nowTime / time)):
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
				case "MenuVisible":
					{
						float time = float.Parse(paramStrings[1]);
						m_gameUI.SetMenuVisible(true, time, callback);
						break;
					}
				case "MenuInvisible":
					{
						float time = float.Parse(paramStrings[1]);
						m_gameUI.SetMenuVisible(false, time, callback);
						break;
					}
				default:
					{
						m_gameUI.OnMovieStart(paramStrings, callback);
						break;
					}
			}
		}

		public void SetupEvent(string param, UnityAction callback)
		{
			m_gameUI.SetupEvent(param, callback);
		}
    }
}
