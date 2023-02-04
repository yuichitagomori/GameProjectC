using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonUI
{
	/// <summary>
	/// 表示切替
	/// </summary>
	public class Switch : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_on;

		[SerializeField]
		private GameObject m_off;

		public void Setup(bool value)
		{
			m_on.SetActive(value == true);
			m_off.SetActive(value == false);
		}
	}
}
