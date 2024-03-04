using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene.game.outgame.window
{
    [System.Serializable]
    public class TitleWindow : WindowBase
    {
		[SerializeField]
		private CommonUI.LocalizeText m_guideText;



		private UnityAction m_onButtonPressEvent;



		public void Setting(UnityAction onButtonPressEvent)
		{
			m_onButtonPressEvent = onButtonPressEvent;
			m_guideText.Text.text = "";
		}

		public override void Go()
		{
			m_guideText.PlayProgression(0.05f, null);
		}

		public override void SetupInputKeyEvent()
		{
			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.Space, () =>
			{
				if (m_onButtonPressEvent != null)
				{
					m_onButtonPressEvent();
				}
			});
		}
	}
}