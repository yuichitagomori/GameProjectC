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

		[SerializeField]
		private CommonUI.ButtonExpansion m_uploadButton;



		private bool m_isMenuVisible = true;



		public void Initialize(
			UnityAction uploadButtonEvent,
			UnityAction<int> commonWindowPlayMovieEvent,
			UnityAction mainWindowPowerButtonEvent,
			UnityAction<KeyCode[]> mainWindowInputEvent)
		{
			m_windowController.Initialize(
				commonWindowPlayMovieEvent: commonWindowPlayMovieEvent,
				mainWindowPowerButtonEvent: mainWindowPowerButtonEvent,
				mainWindowInputEvent: mainWindowInputEvent,
				updateWindowIcon: m_windowIconController.Setting);
			m_windowIconController.Initialize((index) => { m_windowController.SelectWindow(index, null); });
			m_uploadButton.SetupClickEvent(uploadButtonEvent);

			var input = GeneralRoot.Instance.Input;
			input.UpdateEvent(system.InputSystem.Type.Down, KeyCode.Z, () =>
			{
				m_uploadButton.OnDown();
			});
			input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.Z, () =>
			{
				m_uploadButton.OnUp();
				if (m_isMenuVisible == true)
				{
					m_uploadButton.OnClick();
				}
			});
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
