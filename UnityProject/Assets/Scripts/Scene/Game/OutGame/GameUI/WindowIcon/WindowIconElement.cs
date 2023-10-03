using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene.game.outgame
{
	[System.Serializable]
	public class WindowIconElement : MonoBehaviour
	{
		public class Data
		{
			private int m_index;
			public int Index => m_index;

			private Sprite m_frameSprite;
			public Sprite FrameSprite => m_frameSprite;

			private Sprite m_iconSprite;
			public Sprite IconSprite => m_iconSprite;

			private Color m_iconColor;
			public Color IconColor => m_iconColor;

			private UnityAction<int> m_callback;
			public UnityAction<int> Callback => m_callback;


			public Data(
				int index,
				Sprite frameSprite,
				Sprite iconSprite,
				Color iconColor,
				UnityAction<int> callback)
			{
				m_index = index;
				m_frameSprite = frameSprite;
				m_iconSprite = iconSprite;
				m_iconColor = iconColor;
				m_callback = callback;
			}
		}

		[SerializeField]
		private CommonUI.ButtonExpansion m_button;

		[SerializeField]
		private Image m_frameImage;

		[SerializeField]
		private Image[] m_iconImages;






		public void Initialize()
		{
		}

		public void Setting(Data data)
		{
			m_button.SetupClickEvent(() => { data.Callback(data.Index); });
			m_frameImage.sprite = data.FrameSprite;
			for (int i = 0; i < m_iconImages.Length; ++i)
			{
				m_iconImages[i].sprite = data.IconSprite;
				m_iconImages[i].color = data.IconColor;
			}
		}
	}
}
