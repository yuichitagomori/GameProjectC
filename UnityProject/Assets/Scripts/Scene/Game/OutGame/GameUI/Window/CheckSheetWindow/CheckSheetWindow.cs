using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene.game.outgame.window
{
    [System.Serializable]
    public class CheckSheetWindow : WindowBase
    {
		[System.Serializable]
		public class Data
		{
			[SerializeField]
			private int m_id;
			public int Id => m_id;

			[SerializeField]
			private CheckSheetElement.Data[] m_datas;
			public CheckSheetElement.Data[] Datas => m_datas;
		}

		[SerializeField]
		private Data[] m_checkSheetDatas;

		[SerializeField]
		private Common.ElementList m_documentElementList;



		private Data m_checkData;
		private CheckSheetElement.Data.State[] m_answerStates;
		private int m_selectIndex;



		public new void Initialize(RectTransform area, UnityAction holdCallback)
		{
			base.Initialize(area, holdCallback);

			var documentElements = m_documentElementList.GetElements();
			for (int i = 0; i < documentElements.Count; ++i)
			{
				int index = i;
				var element = documentElements[index].GetComponent<CheckSheetElement>();
				element.Initialize((checkState) =>
				{
					m_answerStates[index] = checkState;
					SetupElements();
				});
			}
		}

		public override void SetupEvent(string[] paramStrings, UnityAction callback)
		{
		}
		protected override void SetupInputKeyEvent()
		{
			if (GeneralRoot.Instance.IsPCPlatform() == false)
			{
				return;
			}

			var input = GeneralRoot.Instance.Input;
			input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.W, () =>
			{
				int index = m_selectIndex;
				index--;
				if (index < 0)
				{
					index = 0;
				}
				if (index != m_selectIndex)
				{
					m_selectIndex = index;
					SetupElements();
				}
			});
			input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.S, () =>
			{
				int index = m_selectIndex;
				index++;
				if (index >= m_checkData.Datas.Length)
				{
					index = m_checkData.Datas.Length - 1;
				}
				if (index != m_selectIndex)
				{
					m_selectIndex = index;
					SetupElements();
				}
			});
			input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.A, () =>
			{
				var state = m_answerStates[m_selectIndex];
				if (state == CheckSheetElement.Data.State.BAD)
				{
					state = CheckSheetElement.Data.State.QUESTION;
				}
				else if (state == CheckSheetElement.Data.State.QUESTION)
				{
					state = CheckSheetElement.Data.State.GOOD;
				}
				if (state != m_answerStates[m_selectIndex])
				{
					m_answerStates[m_selectIndex] = state;
					SetupElements();
				}
			});
			input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.D, () =>
			{
				var state = m_answerStates[m_selectIndex];
				if (state == CheckSheetElement.Data.State.GOOD)
				{
					state = CheckSheetElement.Data.State.QUESTION;
				}
				else if (state == CheckSheetElement.Data.State.QUESTION)
				{
					state = CheckSheetElement.Data.State.BAD;
				}
				if (state != m_answerStates[m_selectIndex])
				{
					m_answerStates[m_selectIndex] = state;
					SetupElements();
				}
			});
		}

		public void Setting(int dataId)
		{
			m_checkData = m_checkSheetDatas.FirstOrDefault(d => d.Id == dataId);
			if (m_checkData == null)
			{
				return;
			}

			m_answerStates = new CheckSheetElement.Data.State[m_checkData.Datas.Length];
			for (int i = 0; i < m_answerStates.Length; ++i)
			{
				m_answerStates[i] = CheckSheetElement.Data.State.QUESTION;
			}
			m_selectIndex = 0;

			SetupElements();
		}

		private void SetupElements()
		{
			var documentElements = m_documentElementList.GetElements();
			for (int i = 0; i < documentElements.Count; ++i)
			{
				documentElements[i].SetActive(m_checkData.Datas.Length > i);
				if (m_checkData.Datas.Length > i)
				{
					var element = documentElements[i].GetComponent<CheckSheetElement>();
					element.Setting(new CheckSheetElement.Data(
						m_checkData.Datas[i].Info,
						m_answerStates[i],
						m_selectIndex == i));
				}
			}
		}
	}
}