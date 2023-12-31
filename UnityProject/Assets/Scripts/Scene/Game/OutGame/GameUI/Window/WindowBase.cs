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
			Title,
			Main,
			Message,
			DateTime,
			CheckSheet,
			Chara,
			Common,
		}

		protected KeyCode[] k_useKeys = new KeyCode[]
			{
				KeyCode.W,
				KeyCode.S,
				KeyCode.A,
				KeyCode.D,
				KeyCode.Space,
				KeyCode.X,
				KeyCode.C
			};

		[SerializeField]
		protected Type m_type;
		public Type WindowType => m_type;

		[SerializeField]
		private Sprite m_windowFrameEnableSprite;

		[SerializeField]
		private Sprite m_windowFrameDisableSprite;

		[SerializeField]
		private Image m_windowFrameImage;

		[SerializeField]
		private Sprite m_iconViewFrameEnableSprite;

		[SerializeField]
		private Sprite m_iconViewFrameDisableSprite;

		[SerializeField]
		private Image m_iconViewFrameImage;

		[SerializeField]
		private Image m_iconViewIconImage;

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



		public void Initialize(
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
			m_windowFrameImage.sprite = m_windowFrameDisableSprite;
			if (m_iconViewFrameImage != null)
			{
				m_iconViewFrameImage.sprite = m_iconViewFrameDisableSprite;
			}
			if (m_iconViewIconImage != null)
			{
				m_iconViewIconImage.color = Color.white;
			}

			bool isDone = false;
			m_windowAnime.Play("In", () => { isDone = true; });
			while (!isDone) { yield return null; }
			m_isActiveWindow = true;
		}

		public IEnumerator RemoveWindow()
		{
			m_windowFrameImage.sprite = m_windowFrameDisableSprite;
			if (m_iconViewFrameImage != null)
			{
				m_iconViewFrameImage.sprite = m_iconViewFrameDisableSprite;
			}
			if (m_iconViewIconImage != null)
			{
				m_iconViewIconImage.color = Color.white;
			}

			m_isActiveWindow = false;
			bool isDone = false;
			m_windowAnime.Play("Out", () => { isDone = true; });
			while (!isDone) { yield return null; }
		}

		public void SetSelect(bool value, int windowCount, UnityAction callback)
		{
			StartCoroutine(SetSelectCoroutine(value, windowCount, callback));
		}

		private IEnumerator SetSelectCoroutine(bool value, int windowCount, UnityAction callback)
		{
			bool isDone = false;
			if (m_isSelectWindow == false && value == true)
			{
				m_windowFrameImage.sprite = m_windowFrameEnableSprite;
				if (m_iconViewFrameImage != null)
				{
					m_iconViewFrameImage.sprite = m_iconViewFrameEnableSprite;
				}
				if (m_iconViewIconImage != null)
				{
					m_iconViewIconImage.color = new Color(0.0f, 0.75f, 1.0f);
				}
				m_windowAnime.Play("Enable", () => { isDone = true; });
			}
			else if (m_isSelectWindow == true && value == false)
			{
				m_windowFrameImage.sprite = m_windowFrameDisableSprite;
				if (m_iconViewFrameImage != null)
				{
					m_iconViewFrameImage.sprite = m_iconViewFrameDisableSprite;
				}
				if (m_iconViewIconImage != null)
				{
					m_iconViewIconImage.color = Color.white;
				}
				m_windowAnime.Play("Disable", () => { isDone = true; });
			}
			else
			{
				isDone = true;
			}
			while (!isDone) { yield return null; }

			m_isSelectWindow = value;
			if (m_isSelectWindow == true)
			{
				m_windowTransform.SetSiblingIndex(windowCount - 1);
				SetupInputKeyEvent();
			}

			if (callback != null)
			{
				callback();
			}
		}

		public void OnMove(Vector2 moveV)
		{
			if (m_holdArea != null)
			{
				m_holdArea.OnMove(moveV);
			}
		}
	}
}
