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
			private int m_checkDataId;
			public int CheckDataId => m_checkDataId;

			[SerializeField]
			private CheckSheetElement.Data[] m_datas;
			public CheckSheetElement.Data[] Datas => m_datas;



			public Data(
				int checkDataId,
				int accuracyLevel,
				int[] occurredBugIds)
			{
				m_checkDataId = checkDataId;
				var checkSheedMaster = GeneralRoot.Master.CheckSheetData;
				var checkSheedMasterData = checkSheedMaster.Find(checkDataId);
				if (checkSheedMasterData == null)
				{
					return;
				}
				var checkSheetMasterAccuracyData = checkSheedMasterData.CheckDatas.FirstOrDefault(d => d.AccuracyLevel == accuracyLevel);
				if (checkSheetMasterAccuracyData == null)
				{
					return;
				}

				List<CheckSheetElement.Data> dataList = new List<CheckSheetElement.Data>();
				var checkSheetBugMaster = GeneralRoot.Master.CheckSheetBugData;
				for (int i = 0; i < checkSheetMasterAccuracyData.BudIds.Length; ++i)
				{
					int bugId = checkSheetMasterAccuracyData.BudIds[i];
					var checkSheetBugMasterData = checkSheetBugMaster.Find(bugId);
					if (checkSheetBugMasterData == null)
					{
						continue;
					}
					dataList.Add(new CheckSheetElement.Data(
						bugId,
						checkSheetBugMasterData.Info,
						CheckSheetElement.Data.State.QUESTION,
						(occurredBugIds.Contains(bugId) ? CheckSheetElement.Data.State.BAD : CheckSheetElement.Data.State.GOOD),
						false));
				}
				m_datas = dataList.ToArray();
			}
		}

		[SerializeField]
		private Common.ElementList m_documentElementList;

		[SerializeField]
		private ScrollRect m_scroll;

		[SerializeField]
		private Data m_checkSheetData;



		private int m_selectIndex;



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
			UnityAction<int, int> updateSelectIndexEvent = (beforeIndex, afterIndex) =>
			{
				if (beforeIndex == afterIndex)
				{
					return;
				}

				m_checkSheetData.Datas[beforeIndex].UpdateIsSelect(false);
				m_checkSheetData.Datas[afterIndex].UpdateIsSelect(true);

				m_selectIndex = afterIndex;
				SetupElements();
				SetupScrollPosition();
			};
			for (int i = 0; i < k_useKeys.Length; ++i)
			{
				var key = k_useKeys[i];
				if (key == KeyCode.W ||
					key == KeyCode.S ||
					key == KeyCode.A ||
					key == KeyCode.D)
				{
					input.UpdateEvent(system.InputSystem.Type.Up, key, null);
					if (key == KeyCode.W)
					{
						input.UpdateEvent(system.InputSystem.Type.Down, key, () =>
						{
							int index = m_selectIndex;
							index--;
							if (index < 0)
							{
								index = 0;
							}
							updateSelectIndexEvent(m_selectIndex, index);
						});
					}
					else if (key == KeyCode.S)
					{
						input.UpdateEvent(system.InputSystem.Type.Down, key, () =>
						{
							int index = m_selectIndex;
							index++;
							int indexMax = m_checkSheetData.Datas.Length - 1;
							if (index > indexMax)
							{
								index = indexMax;
							}
							updateSelectIndexEvent(m_selectIndex, index);
						});
					}
					else if (key == KeyCode.A)
					{
						input.UpdateEvent(system.InputSystem.Type.Down, key, () =>
						{
							var state = m_checkSheetData.Datas[m_selectIndex].NowCheckState;
							if (state == CheckSheetElement.Data.State.BAD)
							{
								state = CheckSheetElement.Data.State.QUESTION;
							}
							else if (state == CheckSheetElement.Data.State.QUESTION)
							{
								state = CheckSheetElement.Data.State.GOOD;
							}
							if (state != m_checkSheetData.Datas[m_selectIndex].NowCheckState)
							{
								m_checkSheetData.Datas[m_selectIndex].UpdateNowCheckState(state);
								SetupElements();
							}
						});
					}
					else if (key == KeyCode.D)
					{
						input.UpdateEvent(system.InputSystem.Type.Down, key, () =>
						{
							var state = m_checkSheetData.Datas[m_selectIndex].NowCheckState;
							if (state == CheckSheetElement.Data.State.GOOD)
							{
								state = CheckSheetElement.Data.State.QUESTION;
							}
							else if (state == CheckSheetElement.Data.State.QUESTION)
							{
								state = CheckSheetElement.Data.State.BAD;
							}
							if (state != m_checkSheetData.Datas[m_selectIndex].NowCheckState)
							{
								m_checkSheetData.Datas[m_selectIndex].UpdateNowCheckState(state);
								SetupElements();
							}
						});
					}
					else
					{
						input.UpdateEvent(system.InputSystem.Type.Down, key, null);
					}
				}
			}
		}

		public void Setting(
			int checkSheedId,
			int accuracyLevel,
			int[] occurredBugIds)
		{
			m_checkSheetData = new Data(checkSheedId, accuracyLevel, occurredBugIds);
			m_selectIndex = 0;
			for (int i = 0; i < m_checkSheetData.Datas.Length; ++i)
			{
				m_checkSheetData.Datas[i].UpdateIsSelect(i == m_selectIndex);
			}

			var documentElements = m_documentElementList.GetElements();
			for (int i = 0; i < documentElements.Count; ++i)
			{
				int index = i;
				var element = documentElements[index].GetComponent<CheckSheetElement>();
				element.Initialize((checkState) =>
				{
					m_checkSheetData.Datas[index].UpdateNowCheckState(checkState);
					SetupElements();
				});
			}

			SetupElements();
		}

		private void SetupElements()
		{
			var documentElements = m_documentElementList.GetElements();
			for (int i = 0; i < documentElements.Count; ++i)
			{
				documentElements[i].SetActive(m_checkSheetData.Datas.Length > i);
				if (m_checkSheetData.Datas.Length > i)
				{
					var element = documentElements[i].GetComponent<CheckSheetElement>();
					element.Setting(m_checkSheetData.Datas[i]);
				}
			}
		}

		private void SetupScrollPosition()
		{
			StartCoroutine(SetupScrollPositionCoroutine());
		}

		private IEnumerator SetupScrollPositionCoroutine()
		{
			int viewCount = 5;
			int turnCount = 3;
			int elementCount = m_checkSheetData.Datas.Length;
			float afterValue = 1.0f;

			if (m_selectIndex < turnCount)
			{
				afterValue = 1.0f;
			}
			else if (m_selectIndex >= elementCount - 1 - (viewCount - turnCount))
			{
				afterValue = 0.0f;
			}
			else
			{
				afterValue = 1.0f - (1.0f / (elementCount - (viewCount))) * (m_selectIndex - (turnCount - 1));
			}

			float beforeValue = m_scroll.verticalNormalizedPosition;
			if (beforeValue == afterValue)
			{
				yield break;
			}

			float timeNow = 0.0f;
			float timeMax = 0.2f;
			while (timeNow < timeMax)
			{
				float time = (timeNow / timeMax);
				float value = (afterValue - beforeValue) * time + beforeValue;
				m_scroll.verticalNormalizedPosition = value;

				timeNow += Time.deltaTime;
				yield return null;
			}
			m_scroll.verticalNormalizedPosition = afterValue;
		}
	}
}