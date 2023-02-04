using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scene.game.ingame.app
{
	[System.Serializable]
	public class AppIcon : MonoBehaviour
	{
		[SerializeField]
		private SpriteRenderer m_icon;

		[SerializeField]
		private GameObject m_iconBadge;



		private bool m_isActive = false;

		public void Initialize()
		{
			m_isActive = false;
			UpdateMode(m_isActive);
		}

		public void UpdateMode(bool value)
		{
			m_isActive = value;
			if (m_isActive == true)
			{
				m_icon.color = new Color(0.5f, 1.0f, 0.0f);
			}
			else
			{
				m_icon.color = new Color(0.5f, 0.5f, 0.5f);
			}
			m_iconBadge.SetActive(m_isActive);
		}
	}
}
