using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene.dialog
{
	/// <summary>
	/// パーツ表示管理
	/// </summary>
	public class CustomizePartsView : MonoBehaviour
	{
		/// <summary>
		/// データクラス
		/// </summary>
		public class Data
		{
			/// <summary>
			/// パーツ画像
			/// </summary>
			private Sprite m_partsSprite;
			public Sprite PartsSprite => m_partsSprite;

			/// <summary>
			/// 選択状態かどうか
			/// </summary>
			private bool m_isSet;
			public bool IsSet => m_isSet;

			public Data(
				Sprite partsSprite,
				bool isSet)
			{
				m_partsSprite = partsSprite;
				m_isSet = isSet;
			}
		}

		/// <summary>
		/// パーツアイコン
		/// </summary>
		[SerializeField]
		private Image m_partsImage;

		/// <summary>
		/// セット済みアイコン
		/// </summary>
		[SerializeField]
		private GameObject m_setIconObject;



		public void Initialize()
		{

		}

		public void UpdateView(Data data)
		{
			m_partsImage.sprite = data.PartsSprite;
			m_partsImage.SetNativeSize();
			m_setIconObject.SetActive(data.IsSet);
		}
	}
}