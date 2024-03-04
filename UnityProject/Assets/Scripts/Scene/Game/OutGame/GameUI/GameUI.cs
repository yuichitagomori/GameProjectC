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
		private window.WindowController m_windowController;



		public void Initialize(
			UnityAction<int> commonWindowPlayMovieEvent,
			UnityAction feedbackButtonEvent,
			UnityAction powerButtonEvent,
			UnityAction recreateButtonEvent,
			UnityAction releaseButtonEvent,
			UnityAction canselButtonEvent,
			UnityAction<KeyCode[]> mainWindowInputEvent)
		{
			m_windowController.Initialize(
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
			m_windowController.Go();
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
	}
}
