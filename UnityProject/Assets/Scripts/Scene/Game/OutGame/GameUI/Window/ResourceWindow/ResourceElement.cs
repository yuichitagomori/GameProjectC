using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace scene.game.outgame.window
{
	[System.Serializable]
	public class ResourceElement : MonoBehaviour
	{
		public class Data
		{
			private string m_titleString;
			public string TitleString => m_titleString;

			private string m_infoString;
			public string InfoString => m_infoString;

			private ResourceColorElement.Data[] m_colorDatas;
			public ResourceColorElement.Data[] ColorDatas => m_colorDatas;


			public Data(
				string titleString,
				string infoString,
				int npcId,
				int[] colorIds)
			{
				m_titleString = titleString;
				m_infoString = infoString;
				UpdateColorDatas(npcId, colorIds);
			}

			public void UpdateInfo(string infoString)
			{
				m_infoString = infoString;
			}

			public void UpdateColorDatas(int npcId, int[] colorIds)
			{
				var colorResource = GeneralRoot.Resource.ColorResource;
				var colorResourceData = colorResource.Find(npcId);
				if (colorResourceData == null)
				{
					return;
				}

				List<ResourceColorElement.Data> colorDataList = new List<ResourceColorElement.Data>();
				for (int i = 0; i < colorIds.Length; ++i)
				{
					var color = colorResourceData.Find(colorIds[i]);
					if (color == null)
					{
						continue;
					}
					colorDataList.Add(new ResourceColorElement.Data(
						color.Colors1,
						color.Colors2));
				}
				m_colorDatas = colorDataList.ToArray();
			}
		}

		[SerializeField]
		private Common.AnimatorExpansion m_animator;

		[SerializeField]
		private CommonUI.TextExpansion m_titleText;

		[SerializeField]
		private CommonUI.TextExpansion m_infoText;

		[SerializeField]
		private Common.ElementList m_colorListElementList;




		public void Initialize()
		{
			var elements = m_colorListElementList.GetElements();
			for (int i = 0; i < elements.Count; ++i)
			{
				var element = elements[i];
				element.SetActive(false);
				var colorElement = element.GetComponent<ResourceColorElement>();
				colorElement.Initialize();
			}
		}

		public void Setting(Data data)
		{
			m_titleText.text = data.TitleString;
			m_infoText.text = data.InfoString;

			var elements = m_colorListElementList.GetElements();
			for (int i = 0; i < elements.Count; ++i)
			{
				var element = elements[i];
				if (i < data.ColorDatas.Length)
				{
					element.SetActive(true);
					var colorElement = element.GetComponent<ResourceColorElement>();
					colorElement.Setting(data.ColorDatas[i]);
				}
				else
				{
					element.SetActive(false);
				}
			}
		}
	}
}
