using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene.dialog
{
	[System.Serializable]
	public class CustomizeBoard
	{
		/// <summary>
		/// データクラス
		/// </summary>
		public class Data
		{
			/// <summary>
			/// ユーザーのカスタマイズパーツリスト
			/// </summary>
			private board.CustomizeBoardPartsView.Data[] m_partsViewDatas;
			public board.CustomizeBoardPartsView.Data[] PartsViewDatas => m_partsViewDatas;

			public Data(board.CustomizeBoardPartsView.Data[] partsViewDatas)
			{
				m_partsViewDatas = partsViewDatas;
			}
		}



		/// <summary>
		/// 右移動ボタン
		/// </summary>
		[SerializeField]
		private CommonUI.ButtonExpansion m_moveRightButton;

		/// <summary>
		/// 左移動ボタン
		/// </summary>
		[SerializeField]
		private CommonUI.ButtonExpansion m_moveLeftButton;

		/// <summary>
		/// 上移動ボタン
		/// </summary>
		[SerializeField]
		private CommonUI.ButtonExpansion m_moveUpButton;

		/// <summary>
		/// 下移動ボタン
		/// </summary>
		[SerializeField]
		private CommonUI.ButtonExpansion m_moveDownButton;

		/// <summary>
		/// 回転ボタン
		/// </summary>
		[SerializeField]
		private CommonUI.ButtonExpansion m_rotateButton;

		/// <summary>
		/// 決定ボタン
		/// </summary>
		[SerializeField]
		private CommonUI.ButtonExpansion m_decisionButton;

		/// <summary>
		/// パーツ表示要素
		/// </summary>
		[SerializeField]
		private Common.ElementList m_partsViewElement;


		public void Initialize(
			UnityAction moveRightEvent,
			UnityAction moveLeftEvent,
			UnityAction moveUpEvent,
			UnityAction moveDownEvent,
			UnityAction rotateEvent,
			UnityAction decisionEvent)
		{
			m_moveRightButton.SetupClickEvent(moveRightEvent);
			m_moveLeftButton.SetupClickEvent(moveLeftEvent);
			m_moveUpButton.SetupClickEvent(moveUpEvent);
			m_moveDownButton.SetupClickEvent(moveDownEvent);
			m_rotateButton.SetupClickEvent(rotateEvent);
			m_decisionButton.SetupClickEvent(decisionEvent);
		}

		public void UpdateView(Data data)
		{
			List<Grid> useGridAreaList = new List<Grid>();
			var elementList = m_partsViewElement.GetElements();
			for (int i = 0; i < elementList.Count; ++i)
			{
				if (i >= data.PartsViewDatas.Length)
				{
					elementList[i].SetActive(false);
					continue;
				}

				elementList[i].SetActive(true);
				var partsView = elementList[i].GetComponent<board.CustomizeBoardPartsView>();
				partsView.UpdateView(data.PartsViewDatas[i]);

				if (data.PartsViewDatas[i].StateType == board.CustomizeBoardPartsView.Data.Type.Seted)
				{
					useGridAreaList.AddRange(data.PartsViewDatas[i].GetUseAreaGrids());
				}
			}

			var selectParts = data.PartsViewDatas.FirstOrDefault(d => d.StateType == board.CustomizeBoardPartsView.Data.Type.NotSet);
			if (selectParts != null)
			{
				var useGridArea = selectParts.GetUseAreaGrids();
				string json = JsonUtility.ToJson(useGridArea[0]);
				m_moveRightButton.SetupActive(!useGridArea.Any(d => d.x >= 6));
				m_moveLeftButton.SetupActive(!useGridArea.Any(d => d.x <= 1));
				m_moveUpButton.SetupActive(!useGridArea.Any(d => d.y <= 1));
				m_moveDownButton.SetupActive(!useGridArea.Any(d => d.y >= 6));
				m_rotateButton.SetupActive(true);

				bool isDecision = true;
				for (int i = 0; i < useGridArea.Length; ++i)
				{
					if (useGridAreaList.Contains(useGridArea[i]) == true)
					{
						isDecision = false;
						break;
					}
				}
				m_decisionButton.SetupActive(isDecision);
			}
			else
			{
				m_moveRightButton.SetupActive(false);
				m_moveLeftButton.SetupActive(false);
				m_moveUpButton.SetupActive(false);
				m_moveDownButton.SetupActive(false);
				m_rotateButton.SetupActive(false);
				m_decisionButton.SetupActive(false);
			}
		}
	}
}