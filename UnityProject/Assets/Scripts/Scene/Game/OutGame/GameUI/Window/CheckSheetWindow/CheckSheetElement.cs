using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.outgame.window.checksheet
{
	[System.Serializable]
	public class CheckSheetElement : MonoBehaviour
	{
		[System.Serializable]
		public class Data
		{
			[SerializeField]
			private int m_bugId;
			public int BugId => m_bugId;

			[SerializeField]
			private int m_infoTextId;
			public int InfoTextId => m_infoTextId;

			[SerializeField]
			private bool m_isSelected;
			public bool IsSelected => m_isSelected;

			public Data(
				int bugId,
				int infoTextId,
				bool isSelected)
			{
				m_bugId = bugId;
				m_infoTextId = infoTextId;
				m_isSelected = isSelected;
			}

			public void UpdateIsSelect(bool value)
			{
				m_isSelected = value;
			}
		}

		[SerializeField]
		private CommonUI.TextExpansion m_infoText;

		[SerializeField]
		private GameObject m_cursorObject;

		[SerializeField]
		private Color m_selectedTextColor;

		[SerializeField]
		private Color m_unSelectTextColor;



		public void Initialize()
		{
		}

		public void Setting(Data data)
		{
			m_infoText.text = CommonUI.LocalizeText.GetString(data.InfoTextId);
			m_infoText.color = (data.IsSelected) ? m_selectedTextColor : m_unSelectTextColor;
			m_cursorObject.SetActive(data.IsSelected);
		}
	}
}
