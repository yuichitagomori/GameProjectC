using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Common
{
	/// <summary>
	/// リスト要素管理クラス
	/// </summary>
	public class ElementList : MonoBehaviour
	{
		/// <summary>
		/// リスト要素
		/// </summary>
		[SerializeField]
		private GameObject m_elementObject;

		/// <summary>
		/// 要素数
		/// </summary>
		[SerializeField]
		private int m_elementNum;

		/// <summary>
		/// 各要素
		/// </summary>
		[SerializeField]
		private List<GameObject> m_elements;



		/// <summary>
		/// 各要素取得
		/// </summary>
		/// <returns></returns>
		public List<GameObject> GetElements()
		{
			return m_elements;
		}

		public void AddElement()
		{
			var element = CreateElement();
			m_elements.Add(element);
		}

		private GameObject CreateElement()
		{
			GameObject element = GameObject.Instantiate(m_elementObject);
			Transform elementTransform = element.transform;
			Transform objectTransform = m_elementObject.transform;
			elementTransform.SetParent(objectTransform.parent == null ? this.transform : objectTransform.parent);
			elementTransform.localPosition = objectTransform.localPosition;
			elementTransform.localScale = objectTransform.localScale;
			element.name = m_elements.Count.ToString();
			return element;
		}

#if UNITY_EDITOR
		/// <summary>
		/// エディタスクリプト用
		/// 要素作成
		/// </summary>
		public void SetupElements()
		{
			for (int i = 0; i < m_elementNum; ++i)
			{
				AddElement();
			}
			if (m_elementObject.transform.parent != null)
			{
				m_elementObject.SetActive(false);
			}
		}

		/// <summary>
		/// エディタスクリプト要
		/// 要素削除
		/// </summary>
		public void DestroyElements()
		{
			foreach (GameObject element in m_elements)
			{
				GameObject.DestroyImmediate(element);
			}
			m_elements.Clear();
			if (m_elementObject.transform.parent != null)
			{
				m_elementObject.SetActive(true);
			}
		}
#endif
	}
}