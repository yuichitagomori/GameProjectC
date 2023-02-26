using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace scene.dialog
{
    public class QuestListElement : MonoBehaviour
    {
		public class Data
		{
			private string m_title = "";

			public string Title => m_title;

			private int m_difficultyRank;
			
			public int DifficultyRank => m_difficultyRank;

			private QuestListDialog.Data.Quest.State m_questState;
			
			public QuestListDialog.Data.Quest.State QuestState => m_questState;

			private UnityAction<int> m_onSelectEvent;

			public UnityAction<int> OnSelectEvent => m_onSelectEvent;

			private int m_index;

			public int Index => m_index;

			public Data(
				string title,
				int difficultyRank,
				QuestListDialog.Data.Quest.State questState,
				UnityAction<int> onSelectEvent,
				int index)
			{
				m_title = title;
				m_difficultyRank = difficultyRank;
				m_questState = questState;
				m_onSelectEvent = onSelectEvent;
				m_index = index;
			}
		}

        [SerializeField]
        private CommonUI.ButtonExpansion m_button;

		[SerializeField]
		private Text m_titleText;

		[SerializeField]
		private Common.ElementList m_difficultyRankElementList;

		[SerializeField]
		private Image m_buttonFrame;

		[SerializeField]
		private Sprite m_unselectSprite;

		[SerializeField]
		private Sprite m_selectSprite;



		private Data m_data;

        public void Setup(Data data)
		{
			m_data = data;

			m_titleText.text = m_data.Title;
			var elements = m_difficultyRankElementList.GetElements();
			for (int i = 0; i < elements.Count; ++i)
			{
				elements[i].SetActive(i < m_data.DifficultyRank);
			}
			m_button.SetupClickEvent(OnSelected);
		}

		public void UpdateSelect(bool isSelected)
		{
			if (isSelected == true)
			{
				m_buttonFrame.sprite = m_selectSprite;
			}
			else
			{
				m_buttonFrame.sprite = m_unselectSprite;
			}
		}

		private void OnSelected()
		{
			if (m_data.OnSelectEvent != null)
			{
				m_data.OnSelectEvent(m_data.Index);
			}
		}
	}
}