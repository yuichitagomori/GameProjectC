using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.outgame.window.feedback
{
	[System.Serializable]
	public class FeedbackElement : MonoBehaviour
	{
		[SerializeField]
		private CommonUI.ButtonExpansion m_button;
		public CommonUI.ButtonExpansion Button => m_button;

		[SerializeField]
		private CommonUI.TextExpansion m_infoText;

		[SerializeField]
		private GameObject m_cursorObject;

		[SerializeField]
		private Color m_selectedTextColor;

		[SerializeField]
		private Color m_unSelectTextColor;



		public void Initialize(int infoTextId)
		{
			m_infoText.text = CommonUI.LocalizeText.GetString(infoTextId);
		}

		public void Setting(bool isSelectedCursor)
		{
			m_button.SetupActive(isSelectedCursor);
			m_infoText.color = (isSelectedCursor) ? m_selectedTextColor : m_unSelectTextColor;
			m_cursorObject.SetActive(isSelectedCursor);
		}
	}
}
