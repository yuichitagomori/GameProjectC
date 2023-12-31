using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.actiongame
{
	[System.Serializable]
	public class SwitchObject : ObjectBase
	{
		[SerializeField]
		private Material m_material;

		[SerializeField]
		private Texture m_emissionTextureOn;

		[SerializeField]
		private Texture m_emissionTextureOff;



		private bool m_value;

		private UnityAction<bool> m_switchEvent;



		public void Initialize(bool value, UnityAction<bool> switchEvent)
		{
			m_value = value;
			m_switchEvent = switchEvent;

			m_material.SetTexture("_Emission", (m_value ? m_emissionTextureOn : m_emissionTextureOff));

			base.Initialize(OnEvent);
		}

		private void OnEvent(string param)
		{
			string[] actionStrings = param.Split('_');
			switch (actionStrings[0])
			{
				case "Switch":
					{
						m_value = !m_value;
						m_material.SetTexture("_Emission", (m_value ? m_emissionTextureOn : m_emissionTextureOff));
						m_switchEvent(m_value);
						break;
					}
			}
		}
	}
}
