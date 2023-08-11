using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.outgame.window
{
    [System.Serializable]
    public class WindowBase : MonoBehaviour
    {
        [SerializeField]
		private Transform m_windowTransform = null;

        [SerializeField]
        protected Common.AnimatorExpansion m_windowAnime = null;

		[SerializeField]
		private WindowHoldArea m_holdArea;



		protected bool m_isActiveWindow = false;
		public bool IsActiveWindow => m_isActiveWindow;

		protected bool m_isSelectWindow = false;

		protected void Initialize(UnityAction holdCallback)
		{
			m_windowAnime.Play("Default");

			m_holdArea.Initialize(m_windowTransform, holdCallback);
		}

		public virtual void Go()
		{
		}

		public IEnumerator AddWindow()
		{
			m_isActiveWindow = true;
			bool isDone = false;
			m_windowAnime.Play("In", () => { isDone = true; });
			while (!isDone) { yield return null; }
		}

		public IEnumerator RemoveWindow()
		{
			m_isActiveWindow = false;
			bool isDone = false;
			m_windowAnime.Play("Out", () => { isDone = true; });
			while (!isDone) { yield return null; }
		}

		public void SetSelect(bool value, int windowCount)
		{
			if (m_isSelectWindow == false && value == true)
			{
				m_windowAnime.Play("Enable", null);
			}
			else if (m_isSelectWindow == true && value == false)
			{
				m_windowAnime.Play("Disable", null);
			}
			m_isSelectWindow = value;
			if (m_isSelectWindow == true)
			{
				m_windowTransform.SetSiblingIndex(windowCount - 1);
			}
		}

		public void Move(Vector2 v)
		{
			m_windowTransform.localPosition += new Vector3(v.x, v.y, 0.0f);
		}
	}
}
