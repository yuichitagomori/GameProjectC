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
		public class Data
		{
			private checksheet.CheckSheetElement.Data[] m_datas;
			public checksheet.CheckSheetElement.Data[] Datas => m_datas;



			public Data(int[] checkSheetBugDataIds)
			{
				var checkSheetBugMaster = GeneralRoot.Master.CheckSheetBugData;
				List<checksheet.CheckSheetElement.Data> dataList = new List<checksheet.CheckSheetElement.Data>();
				for (int i = 0; i < checkSheetBugDataIds.Length; ++i)
				{
					int bugId = checkSheetBugDataIds[i];
					var checkSheetBugMasterData = checkSheetBugMaster.Find(bugId);
					if (checkSheetBugMasterData == null)
					{
						continue;
					}
					dataList.Add(new checksheet.CheckSheetElement.Data(
						bugId,
						checkSheetBugMasterData.InfoTextId,
						false));
				}
				m_datas = dataList.ToArray();
			}
		}

		[SerializeField]
		private Common.ElementList m_documentElementList;

		[SerializeField]
		private ScrollRect m_scroll;




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
					GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, key, null);
					if (key == KeyCode.W)
					{
						GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Down, key, () =>
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
						GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Down, key, () =>
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
					else
					{
						GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Down, key, null);
					}
				}
			}
		}

		public void Setting()
		{
			var local = GeneralRoot.User.LocalSaveData;
			var gameGunreMaster = GeneralRoot.Master.GameGunreData;
			var gameGunreMasterData = gameGunreMaster.Find(local.ChallengeGameGunreId);
			if (gameGunreMasterData == null)
			{
				return;
			}

			m_checkSheetData = new Data(gameGunreMasterData.CheckSheetBugIds);
			m_selectIndex = 0;
			for (int i = 0; i < m_checkSheetData.Datas.Length; ++i)
			{
				m_checkSheetData.Datas[i].UpdateIsSelect(i == m_selectIndex);
			}

			var documentElements = m_documentElementList.GetElements();
			for (int i = 0; i < documentElements.Count; ++i)
			{
				int index = i;
				var element = documentElements[index].GetComponent<checksheet.CheckSheetElement>();
				element.Initialize();
			}

			SetupElements();
		}

		private void SetupElements()
		{
			var documentElements = m_documentElementList.GetElements();
			for (int i = 0; i < documentElements.Count; ++i)
			{
				documentElements[i].SetActive(m_checkSheetData.Datas.Length > i);
				if (m_checkSheetData.Datas.Length <= i)
				{
					continue;
				}
				var element = documentElements[i].GetComponent<checksheet.CheckSheetElement>();
				element.Setting(m_checkSheetData.Datas[i]);
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