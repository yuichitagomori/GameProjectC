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
		private CanvasGroup m_canvasGroup;

		[SerializeField]
		private window.WindowController m_windowController;

		[SerializeField]
		private WindowIconController m_windowIconController;




		public void Initialize(
			UnityAction<KeyCode[]> inputEvent)
		{
			m_windowController.Initialize(
				inputEvent: inputEvent,
				updateWindowIcon: m_windowIconController.Setting);
			m_windowIconController.Initialize(m_windowController.SelectWindow);
		}

		public void Go()
		{
			m_windowController.Go();
		}


		public void SetVisible(bool value)
		{
			if (value == true)
			{
				m_canvasGroup.alpha = 1.0f;
			}
			else
			{
				m_canvasGroup.alpha = 0.0f;
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
