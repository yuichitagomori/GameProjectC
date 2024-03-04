using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.outgame.window
{
    [System.Serializable]
    public class FeedbackWindow : WindowBase
	{

		[SerializeField]
		private feedback.FeedbackElement[] m_buttonMenus;



		private int m_selectIndex;



		public void Setting(
			UnityAction powerButtonEvent,
			UnityAction recreateButtonEvent,
			UnityAction releaseButtonEvent,
			UnityAction canselButtonEvent)
		{
			m_buttonMenus[0].Button.SetupClickEvent(powerButtonEvent);
			m_buttonMenus[1].Button.SetupClickEvent(recreateButtonEvent);
			m_buttonMenus[2].Button.SetupClickEvent(releaseButtonEvent);
			m_buttonMenus[3].Button.SetupClickEvent(canselButtonEvent);

			m_selectIndex = 0;
			UpdateInfoMenuCursols();
		}

		public override void Go()
		{
		}

		public override void SetupInputKeyEvent()
		{
			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.UpArrow, () =>
			{
				if (m_selectIndex > 0)
				{
					m_selectIndex--;
					UpdateInfoMenuCursols();
				}
			});
			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.DownArrow, () =>
			{
				if (m_selectIndex < 3)
				{
					m_selectIndex++;
					UpdateInfoMenuCursols();
				}
			});

			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Down, KeyCode.Space, () =>
			{
				m_buttonMenus[m_selectIndex].Button.OnDown();
			});
			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.Space, () =>
			{
				m_buttonMenus[m_selectIndex].Button.OnUp();
				m_buttonMenus[m_selectIndex].Button.OnClick();
			});
			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Down, KeyCode.C, () =>
			{
				if (m_selectIndex != 3)
				{
					m_selectIndex = 3;
					UpdateInfoMenuCursols();
				}
				m_buttonMenus[m_selectIndex].Button.OnDown();
			});
			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.C, () =>
			{
				m_buttonMenus[m_selectIndex].Button.OnUp();
				m_buttonMenus[m_selectIndex].Button.OnClick();
			});
		}

		private void UpdateInfoMenuCursols()
		{
			for (int i = 0; i < m_buttonMenus.Length; ++i)
			{
				m_buttonMenus[i].Setting(i == m_selectIndex);
			}
		}
	}
}
