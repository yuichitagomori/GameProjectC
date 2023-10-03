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
		private Image m_fade = null;

		[SerializeField]
		private outgame.GameUI m_gameUI = null;


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
			UnityAction<KeyCode[]> inputEvent)
		{
			var fadeColor = m_fade.color;
			fadeColor.a = 1.0f;
			m_fade.color = fadeColor;

			m_gameUI.Initialize(inputEvent: inputEvent);
		}

		public void Go()
		{
			m_gameUI.Go();
		}

		public void Fade(bool isFadeIn, float fadeTime, UnityAction callback)
		{
			StartCoroutine(FadeCoroutine(isFadeIn, fadeTime, callback));
		}

		private IEnumerator FadeCoroutine(bool isFadeIn, float fadeTime, UnityAction callback)
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
				float alpha = (isFadeIn) ? (nowTime / fadeTime) : 1.0f - (nowTime / fadeTime);
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
