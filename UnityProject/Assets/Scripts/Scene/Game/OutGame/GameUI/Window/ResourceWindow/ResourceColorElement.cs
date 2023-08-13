using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace scene.game.outgame.window
{
	[System.Serializable]
	public class ResourceColorElement : MonoBehaviour
	{
		public class Data
		{
			private Color m_color1;
			public Color Color1 => m_color1;

			private Color m_color2;
			public Color Color2 => m_color2;

			public Data(Color color1, Color color2)
			{
				m_color1 = color1;
				m_color2 = color2;
			}
		}

		[System.Serializable]
		public class ColorView
		{
			[SerializeField]
			private Image m_image;

			[SerializeField]
			private CommonUI.TextExpansion m_text;

			[SerializeField]
			private Shadow m_textShadow;

			public void Setting(Color color)
			{
				m_image.color = color;
				float ColorTotal1 = color.r + color.g + color.b;
				if (ColorTotal1 > 1.5f)
				{
					// 明るい色なので、黒テキストに（影は白）
					m_text.color = Color.black;
					m_textShadow.effectColor = Color.white;
				}
				else
				{
					// 暗い色なので、白テキストに（影は黒）
					m_text.color = Color.white;
					m_textShadow.effectColor = Color.black;
				}
			}
		}

		[SerializeField]
		private ColorView m_colorView1;

		[SerializeField]
		private ColorView m_colorView2;



		public void Initialize()
		{
		}

		public void Setting(Data data)
		{
			m_colorView1.Setting(data.Color1);
			m_colorView2.Setting(data.Color2);
		}
	}
}
