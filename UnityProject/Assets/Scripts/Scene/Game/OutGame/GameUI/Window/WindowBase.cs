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
			Camera,

			Common,
			Feedback,
			Image,
			Result,
		}

		[SerializeField]
		protected Type m_type;
		public Type WindowType => m_type;

		[SerializeField]
		private int m_priority;
		public int Priority => m_priority;

		[SerializeField]
		protected Transform m_windowTransform;

		[SerializeField]
		protected Vector2 m_windowSize;

        [SerializeField]
        protected Common.AnimatorExpansion m_windowAnime;



		private UnityAction<Transform> m_setTopSiblingEvent;



		public void Initialize(UnityAction<Transform> setTopSiblingEvent)
		{
			m_setTopSiblingEvent = setTopSiblingEvent;
			PlayAnimation("Default");
		}

		public virtual void Go() { }

		public virtual void OnMovieStart(string[] paramStrings, UnityAction callback) { }

		public virtual void SetupInputKeyEvent() { }

		public IEnumerator AddWindow()
		{
			GeneralRoot.Sound.PlaySE((int)system.SoundSystem.SEType.WINDOW_OPEN);
			bool isDone = false;
			PlayAnimation("In", () => { isDone = true; });
			while (!isDone) { yield return null; }
		}

		public IEnumerator RemoveWindow()
		{
			bool isDone = false;
			PlayAnimation("Out", () => { isDone = true; });
			while (!isDone) { yield return null; }
		}

		private void PlayAnimation(string name, UnityAction callback = null)
		{
			if (m_windowAnime != null)
			{
				m_windowAnime.Play(name, callback);
			}
			else
			{
				StartCoroutine(PlayAnimationCoroutine(name, callback));
			}
		}

		private IEnumerator PlayAnimationCoroutine(string name, UnityAction callback)
		{
			var animatorTransform = m_windowTransform.GetChild(0).GetComponent<RectTransform>();
			switch (name)
			{
				case "Default":
					{
						animatorTransform.sizeDelta = new Vector2(0, 0);
						break;
					}
				case "In":
					{
						animatorTransform.sizeDelta = new Vector2(0, 0);

						UnityAction<float> update = (t) =>
						{
							animatorTransform.sizeDelta = new Vector2(m_windowSize.x * t, 0.0f);
						};
						yield return CommonMath.EaseInOut(0.1f, update, null);
						animatorTransform.sizeDelta = new Vector2(m_windowSize.x, 0.0f);

						update = (t) =>
						{
							animatorTransform.sizeDelta = new Vector2(m_windowSize.x, m_windowSize.y * t);
						};
						yield return CommonMath.EaseInOut(0.1f, update, null);
						animatorTransform.sizeDelta = new Vector2(m_windowSize.x, m_windowSize.y);
						break;
					}
				case "Out":
					{
						animatorTransform.sizeDelta = m_windowSize;

						UnityAction<float> update = (t) =>
						{
							animatorTransform.sizeDelta = new Vector2(m_windowSize.x, m_windowSize.y * (1 - t));
						};
						yield return CommonMath.EaseInOut(0.1f, update, null);
						animatorTransform.sizeDelta = new Vector2(m_windowSize.x, 0.0f);

						update = (t) =>
						{
							animatorTransform.sizeDelta = new Vector2(m_windowSize.x * (1 - t), 0.0f);
						};
						yield return CommonMath.EaseInOut(0.1f, update, null);
						animatorTransform.sizeDelta = new Vector2(0.0f, 0.0f);

						break;
					}
			}

			if (callback != null)
			{
				callback();
			}
		}

		protected void SetTopSibling()
		{
			m_setTopSiblingEvent(m_windowTransform);
		}
	}
}
