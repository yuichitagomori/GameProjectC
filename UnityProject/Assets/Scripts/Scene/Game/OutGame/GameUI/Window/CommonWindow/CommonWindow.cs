using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace scene.game.outgame.window
{
    [System.Serializable]
    public class CommonWindow : WindowBase
    {
		[SerializeField]
		private CommonUI.TextExpansion m_messageText;

		[SerializeField]
		private CommonUI.ButtonExpansion m_yesButton;

		[SerializeField]
		private CommonUI.ButtonExpansion m_noButton;



		public void Setting(
			string messageText,
			UnityAction yesButtonEvent,
			UnityAction noButtonEvent)
		{
			m_messageText.text = messageText;
			m_yesButton.SetupClickEvent(yesButtonEvent);
			m_noButton.SetupClickEvent(noButtonEvent);
		}

		public override void Go()
		{
		}

		public override void SetupInputKeyEvent()
		{
			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Down, KeyCode.Space, () =>
			{
				m_yesButton.OnDown();
			});
			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.Space, () =>
			{
				m_yesButton.OnUp();
				m_yesButton.OnClick();
			});

			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Down, KeyCode.C, () =>
			{
				m_noButton.OnDown();
			});
			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.C, () =>
			{
				m_noButton.OnUp();
				m_noButton.OnClick();
			});
		}
	}
}