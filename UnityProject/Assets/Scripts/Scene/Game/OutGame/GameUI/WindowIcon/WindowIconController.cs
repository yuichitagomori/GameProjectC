using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.outgame
{
	public class WindowIconController : MonoBehaviour
	{
		[System.Serializable]
		public class SpriteData
		{
			[SerializeField]
			private window.WindowBase.Type m_type;
			public window.WindowBase.Type Type => m_type;

			[SerializeField]
			private Sprite m_iconSprite;
			public Sprite IconSprite => m_iconSprite;
		}

		[SerializeField]
		private Sprite m_frameEnableSprite;
		public Sprite FrameEnableSprite => m_frameEnableSprite;

		[SerializeField]
		private Sprite m_frameDisableSprite;
		public Sprite FrameDisableSprite => m_frameDisableSprite;

		[SerializeField]
		private Color m_iconEnableColor;
		public Color IconEnableColor => m_iconEnableColor;

		[SerializeField]
		private Color m_iconDisableColor;
		public Color IconDisableColor => m_iconDisableColor;

		[SerializeField]
		private Common.ElementList m_elementList;

		[SerializeField]
		private SpriteData[] m_spriteDatas;



		private UnityAction<int> m_buttonCallback;



		public void Initialize(UnityAction<int> buttonCallback)
		{
			m_buttonCallback = buttonCallback;

			var elements = m_elementList.GetElements();
			for (int i = 0; i < elements.Count; ++i)
			{
				elements[i].SetActive(false);
				var element = elements[i].GetComponent<WindowIconElement>();
				element.Initialize();
			}
		}

		public void Setting(window.WindowBase.Type[] types, int selectedIndex)
		{
			List<WindowIconElement.Data> dataList = new List<WindowIconElement.Data>();
			for (int i = 0; i < types.Length; ++i)
			{
				var data = GetData(i, types[i], (selectedIndex == i), (index) =>
				{
					m_buttonCallback(index);
				});
				if (data == null)
				{
					continue;
				}

				dataList.Add(data);
			}

			var elements = m_elementList.GetElements();
			for (int i = 0; i < elements.Count; ++i)
			{
				if (dataList.Count <= i || dataList.Count < 2)
				{
					elements[i].SetActive(false);
					continue;
				}

				elements[i].SetActive(true);
				var element = elements[i].GetComponent<WindowIconElement>();
				element.Setting(dataList[i]);
			}
		}

		private WindowIconElement.Data GetData(
			int index,
			window.WindowBase.Type type,
			bool isSelected,
			UnityAction<int> callback)
		{
			var spriteData = m_spriteDatas.FirstOrDefault(d => d.Type == type);
			if (spriteData == null)
			{
				return null;
			}

			return new WindowIconElement.Data(
				index,
				(isSelected) ? m_frameEnableSprite : m_frameDisableSprite,
				spriteData.IconSprite,
				(isSelected) ? m_iconEnableColor : m_iconDisableColor,
				callback);
		}
	}
}
