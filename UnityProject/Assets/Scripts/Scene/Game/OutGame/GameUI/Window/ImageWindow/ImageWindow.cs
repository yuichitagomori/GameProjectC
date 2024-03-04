using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace scene.game.outgame.window
{
    [System.Serializable]
    public class ImageWindow : WindowBase
    {
		[System.Serializable]
		public class Data
		{
			[SerializeField]
			private int id;
			public int ID => id;

			[SerializeField]
			private Sprite imageSprite;
			public Sprite ImageSprite => imageSprite;
		}

		[SerializeField]
		private Image m_image;

		[SerializeField]
		private Data[] m_datas;



		public void Setting(int id)
		{
			var sprite = m_datas.FirstOrDefault(d => d.ID == id).ImageSprite;
			m_image.sprite = sprite;
			m_image.SetNativeSize();
			m_windowSize = sprite.rect.size;
		}

		public override void Go()
		{
		}
	}
}