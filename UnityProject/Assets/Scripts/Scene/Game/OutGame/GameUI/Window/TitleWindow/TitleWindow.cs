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
		private UnityAction m_onButtonPressEvent;


		public override void SetupEvent(string[] paramStrings, UnityAction callback)
		{
		}

		protected override void SetupInputKeyEvent()
		{
			if (GeneralRoot.Instance.IsPCPlatform() == false)
			{
				return;
			}

			for (int i = 0; i < k_useKeys.Length; ++i)
			{
				var key = k_useKeys[i];
				if (key == KeyCode.Space)
				{
					GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Down, key, null);
					GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, key, m_onButtonPressEvent);
				}
				else
				{
					GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Down, key, null);
					GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, key, null);
				}
			}
		}

		public void Setting(UnityAction onButtonPressEvent)
		{
			m_onButtonPressEvent = onButtonPressEvent;
		}
	}
}