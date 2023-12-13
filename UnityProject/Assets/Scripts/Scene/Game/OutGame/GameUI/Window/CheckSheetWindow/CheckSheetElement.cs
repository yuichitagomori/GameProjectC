using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.outgame.window
{
	[System.Serializable]
	public class CheckSheetElement : MonoBehaviour
	{
		[System.Serializable]
		public class Data
		{
			public enum State
			{
				GOOD,
				QUESTION,
				BAD,
			}

			[SerializeField]
			private int m_bugId;
			public int BugId => m_bugId;

			[SerializeField]
			private string m_info;
			public string Info => m_info;

			[SerializeField]
			private State m_nowCheckState;
			public State NowCheckState => m_nowCheckState;

			[SerializeField]
			private State m_answerCheckState;
			public State AnswerCheckState => m_answerCheckState;

			[SerializeField]
			private bool m_isSelected;
			public bool IsSelected => m_isSelected;

			public Data(
				int bugId,
				string info,
				State nowCheckState,
				State answerCheckState,
				bool isSelected)
			{
				m_bugId = bugId;
				m_info = info;
				m_nowCheckState = nowCheckState;
				m_answerCheckState = answerCheckState;
				m_isSelected = isSelected;
			}

			public void UpdateNowCheckState(State state)
			{
				m_nowCheckState = state;
			}

			public void UpdateIsSelect(bool value)
			{
				m_isSelected = value;
			}
		}

		[SerializeField]
		private CommonUI.TextExpansion m_infoText;

		[SerializeField]
		private CommonUI.ButtonExpansion m_goodButton;

		[SerializeField]
		private CommonUI.ButtonExpansion m_questionButton;

		[SerializeField]
		private CommonUI.ButtonExpansion m_badButton;

		[SerializeField]
		private GameObject m_goodActiveObject;

		[SerializeField]
		private GameObject m_questionActiveObject;

		[SerializeField]
		private GameObject m_badActiveObject;

		[SerializeField]
		private GameObject m_cursorObject;

		[SerializeField]
		private Color m_selectedTextColor;

		[SerializeField]
		private Color m_unSelectTextColor;



		public void Initialize(UnityAction<Data.State> buttonEvent)
		{
			m_goodButton.SetupClickEvent(() => { buttonEvent(Data.State.GOOD); });
			m_questionButton.SetupClickEvent(() => { buttonEvent(Data.State.QUESTION); });
			m_badButton.SetupClickEvent(() => { buttonEvent(Data.State.BAD); });
		}

		public void Setting(Data data)
		{
			m_infoText.text = data.Info;
			m_infoText.color = (data.IsSelected) ? m_selectedTextColor : m_unSelectTextColor;
			m_goodButton.SetupActive(data.NowCheckState == Data.State.GOOD);
			m_goodActiveObject.SetActive(data.NowCheckState == Data.State.GOOD);
			m_questionButton.SetupActive(data.NowCheckState == Data.State.QUESTION);
			m_questionActiveObject.SetActive(data.NowCheckState == Data.State.QUESTION);
			m_badButton.SetupActive(data.NowCheckState == Data.State.BAD);
			m_badActiveObject.SetActive(data.NowCheckState == Data.State.BAD);
			m_cursorObject.SetActive(data.IsSelected);
		}
	}
}
