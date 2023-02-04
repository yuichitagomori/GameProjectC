using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppliIconController : MonoBehaviour
{
	public class Data
	{
		public int m_index { get; private set; }

		public Data(int _index)
		{
			m_index = _index;
		}
	}

	[SerializeField]
	private SpriteRenderer m_appliIconSprite;

	[SerializeField]
	private AnimatorExpansion m_animator;



	private Data m_data = null;

	public void Setup(Data _data)
	{
	}

	public IEnumerator SetAnimationIn()
	{
		bool isDone = false;
		m_animator.Play("In", () => { isDone = true; });
		while (!isDone) { yield return null; }
	}

	public IEnumerator SetAnimationOut()
	{
		bool isDone = false;
		m_animator.Play("Out", () => { isDone = true; });
		while (!isDone) { yield return null; }
	}
}
