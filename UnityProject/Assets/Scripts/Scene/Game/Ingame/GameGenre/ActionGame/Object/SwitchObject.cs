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



		private bool m_value = false;
		public bool Value => m_value;



		public new void Initialize(UnityAction<string[]> onEvent)
		{
			base.Initialize(onEvent);
			SetupMaterial();
		}

		public void Switch(bool value)
		{
			if (m_value == value)
			{
				return;
			}
			m_value = value;
			SetupMaterial();
		}

		private void SetupMaterial()
		{
			m_material.SetTexture("_Emission", (m_value ? m_emissionTextureOn : m_emissionTextureOff));
		}
	}
}
