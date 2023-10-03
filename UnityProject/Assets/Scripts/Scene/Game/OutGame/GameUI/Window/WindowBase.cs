using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene.game.outgame.window
{
    [System.Serializable]
    public abstract class WindowBase : MonoBehaviour
    {
		public enum Type
		{
			None = -1,
			Main,
			Message,
			DateTime,
			CheckSheet,
		}

		[SerializeField]
		protected Type m_type;
		public Type WindowType => m_type;

		[SerializeField]
		private Sprite m_frameEnableSprite;

		[SerializeField]
		private Sprite m_frameDisableSprite;

		[SerializeField]
		private Image m_frameImage;

		[SerializeField]
		protected Transform m_windowTransform;

		[SerializeField]
		private Vector2 m_windowSize;

        [SerializeField]
        protected Common.AnimatorExpansion m_windowAnime;

		[SerializeField]
		private WindowHoldArea m_holdArea;



		protected bool m_isActiveWindow = false;
		public bool IsActiveWindow => m_isActiveWindow;

		protected bool m_isSelectWindow = false;



		protected void Initialize(
			RectTransform windowArea,
			UnityAction holdCallback)
		{
			m_windowAnime.Play("Default");

			if (m_holdArea != null)
			{
				m_holdArea.Initialize(m_windowTransform, m_windowSize, windowArea, holdCallback);
			}
		}

		public virtual void Go()
		{
		}

		public abstract void SetupEvent(string[] paramStrings, UnityAction callback);

		protected abstract void SetupInputKeyEvent();

		public IEnumerator AddWindow()
		{
			m_frameImage.sprite = m_frameDisableSprite;
			m_holdArea.SetupImage(false);

			bool isDone = false;
			m_windowAnime.Play("In", () => { isDone = true; });
			while (!isDone) { yield return null; }
			m_isActiveWindow = true;
		}

		public IEnumerator RemoveWindow()
		{
			m_frameImage.sprite = m_frameDisableSprite;
			m_holdArea.SetupImage(false);

			m_isActiveWindow = false;
			bool isDone = false;
			m_windowAnime.Play("Out", () => { isDone = true; });
			while (!isDone) { yield return null; }
		}

		public void SetSelect(bool value, int windowCount)
		{
			if (m_isSelectWindow == false && value == true)
			{
				m_frameImage.sprite = m_frameEnableSprite;
				m_holdArea.SetupImage(true);

				m_windowAnime.Play("Enable", null);
			}
			else if (m_isSelectWindow == true && value == false)
			{
				m_frameImage.sprite = m_frameDisableSprite;
				m_holdArea.SetupImage(false);

				m_windowAnime.Play("Disable", null);
			}
			m_isSelectWindow = value;
			if (m_isSelectWindow == true)
			{
				m_windowTransform.SetSiblingIndex(windowCount - 1);
				SetupInputKeyEvent();
			}
		}

		public void OnMove(Vector2 moveV)
		{
			m_holdArea.OnMove(moveV);
		}
	}
}
