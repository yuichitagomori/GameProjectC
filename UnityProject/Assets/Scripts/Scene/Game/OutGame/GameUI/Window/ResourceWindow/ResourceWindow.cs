using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.outgame.window
{
    [System.Serializable]
    public class ResourceWindow : WindowBase
    {
		public class ElementData
		{
			private int m_npcId;
			public int NPCId => m_npcId;

			private List<int> m_colorIdList = new List<int>();
			public List<int> ColorIdList => m_colorIdList;

			private int m_npcCount;
			public int NPCCount => m_npcCount;

			private ResourceElement.Data m_data;
			public ResourceElement.Data Data => m_data;

			public ElementData(int npcId, int colorId, int npcCount)
			{
				m_npcId = npcId;
				ColorIdList.Add(colorId);
				m_npcCount += npcCount;
				m_data = new ResourceElement.Data(GetTitleString(), GetInfoString(), npcId, ColorIdList.ToArray());
			}

			public void AddColorId(int colorId)
			{
				if (ColorIdList.Contains(colorId) == true)
				{
					return;
				}
				ColorIdList.Add(colorId);
				m_data.UpdateColorDatas(m_npcId, ColorIdList.ToArray());
			}

			//public void AddColorIds(int[] colorIds)
			//{
			//	ColorIdList.AddRange(colorIds);
			//}

			public void AddNPCCount(int npcCount)
			{
				m_npcCount += npcCount;
				m_data.UpdateInfo(GetInfoString());
			}

			private string GetTitleString()
			{
				string titleString = "";
				switch (m_npcId)
				{
					case 33:
						{
							titleString = "ヘビ型";
							break;
						}
				}
				return titleString;
			}

			private string GetInfoString()
			{
				string infoString = "";
				switch (m_npcId)
				{
					case 33:
						{
							infoString = string.Format("ヘビのキャラクター\n移動速度は遅い\n配置数：{0}", m_npcCount);
							break;
						}
				}
				return infoString;
			}
		}

		[SerializeField]
		private Common.ElementList m_elementList;



		private List<ElementData> m_dataList = new List<ElementData>();

		public new void Initialize(UnityAction holdCallback)
		{
			base.Initialize(holdCallback);

			var elements = m_elementList.GetElements();
			for (int i = 0; i < elements.Count; ++i)
			{
				elements[i].SetActive(false);
				var element = elements[i].GetComponent<ResourceElement>();
				element.Initialize();
			}
		}

		public void AddResource(int npcId, int colorId, int npcCount)
		{
			var findData = m_dataList.Find(d => d.NPCId == npcId);
			if (findData != null)
			{
				findData.AddColorId(colorId);
				findData.AddNPCCount(npcCount);
			}
			else
			{
				m_dataList.Add(new ElementData(npcId, colorId, npcCount));
			}
			Setting();
		}

		private void Setting()
		{
			var elements = m_elementList.GetElements();
			for (int i = 0; i < elements.Count; ++i)
			{
				if (i < m_dataList.Count)
				{
					elements[i].SetActive(true);
					var element = elements[i].GetComponent<ResourceElement>();
					element.Setting(m_dataList[i].Data);
				}
				else
				{
					elements[i].SetActive(false);
				}
			}
		}
	}
}