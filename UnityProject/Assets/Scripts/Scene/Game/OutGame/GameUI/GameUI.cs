using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

namespace scene.game.outgame
{
	[System.Serializable]
	public class GameUI : MonoBehaviour
	{

		[SerializeField]
		private CanvasGroup m_menuCanvasGroup;

		[SerializeField]
		private window.WindowController m_windowController;

		[SerializeField]
		private WindowIconController m_windowIconController;



		private bool m_isMenuVisible = true;



		public void Initialize(
			UnityAction<int> commonWindowPlayMovieEvent,
			UnityAction mainWindowPowerButtonEvent,
			UnityAction mainWindowRecreateButtonEvent,
			UnityAction mainWindowReleaseButtonEvent,
			UnityAction<KeyCode[]> mainWindowInputEvent)
		{
			m_windowController.Initialize(
				commonWindowPlayMovieEvent: commonWindowPlayMovieEvent,
				mainWindowPowerButtonEvent: mainWindowPowerButtonEvent,
				mainWindowRecreateButtonEvent: mainWindowRecreateButtonEvent,
				mainWindowReleaseButtonEvent: mainWindowReleaseButtonEvent,
				mainWindowInputEvent: mainWindowInputEvent,
				updateWindowIcon: m_windowIconController.Setting);
			m_windowIconController.Initialize((index) => { m_windowController.SelectWindow(index, null); });
		}

		public void Go()
		{
			m_windowController.Go();
		}


		public void SetMenuVisible(bool value, float time, UnityAction callback)
		{
			m_isMenuVisible = value;
			StartCoroutine(SetMenuVisibleCoroutine(value, time, callback));
		}

		private IEnumerator SetMenuVisibleCoroutine(bool value, float time, UnityAction callback)
		{
			float nowTime = 0.0f;
			while (nowTime < time)
			{
				if (value == true)
				{
					m_menuCanvasGroup.alpha = nowTime / time;
				}
				else
				{
					m_menuCanvasGroup.alpha = 1.0f - (nowTime / time);
				}

				nowTime += Time.deltaTime;

				yield return null;
			}

			if (value == true)
			{
				m_menuCanvasGroup.alpha = 1.0f;
			}
			else
			{
				m_menuCanvasGroup.alpha = 0.0f;
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
						m_windowController.OnMovieStart(paramStrings, callback);
						break;
					}
			}
		}

		public void SetupEvent(string param, UnityAction callback)
		{
			m_windowController.SetupEvent(param, callback);
		}
	}
}
