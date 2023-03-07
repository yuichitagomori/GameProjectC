using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonUI
{
	/// <summary>
	/// 表示切替
	/// </summary>
	public class SwitchSprite : MonoBehaviour
	{
		[SerializeField]
		private UnityEngine.UI.Image m_targetImage;

		[SerializeField]
		private Sprite m_on;

		[SerializeField]
		private Sprite m_off;

		public void Setup(bool value)
		{
			m_targetImage.sprite = (value == true) ? m_on : m_off;
		}
	}
}
