using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene
{
	public class CommonDialog : SceneBase
	{
		/// <summary>
		/// アニメーター
		/// </summary>
		[SerializeField]
		private AnimatorExpansion m_animator;

		/// <summary>
		/// メッセージ
		/// </summary>
		[SerializeField]
		private Text m_messagetext;

		/// <summary>
		/// ボタン
		/// </summary>
		[SerializeField]
		private CommonUI.ButtonExpansion m_button;

		/// <summary>
		/// ボタンテキスト
		/// </summary>
		[SerializeField]
		private Text m_buttonText;



		/// <summary>
		/// メッセージ文言
		/// </summary>
		private string m_messageTextStr;

		/// <summary>
		/// ボタン文言
		/// </summary>
		private string m_buttonTextStr;



		/// <summary>
		/// 事前設定
		/// </summary>
		/// <param name="_messageTextStr"></param>
		/// <param name="_buttonTextStr"></param>
		public void Setting(string _messageTextStr, string _buttonTextStr)
		{
			m_messageTextStr = _messageTextStr;
			m_buttonTextStr = _buttonTextStr;
		}

		public override void Ready(UnityAction _callback)
		{
			StartCoroutine(ReadyCoroutine(() =>
			{
				if (_callback != null)
				{
					_callback();
				}
			}));
		}

		public override void Go()
		{
			StartCoroutine(GoCoroutine());
		}

		private IEnumerator ReadyCoroutine(UnityAction _callback)
		{
			m_messagetext.text = m_messageTextStr;

			m_button.SetupClickEvent(OnButtonPressed);
			m_button.interactable = false;
			m_buttonText.text = m_buttonTextStr;

			yield return null;

			if (_callback != null)
			{
				_callback();
			}
		}

		private IEnumerator GoCoroutine()
		{
			bool isDone = false;
			m_animator.Play("Open", () => { isDone = true; });
			while (!isDone) { yield return null; }

			m_button.interactable = true;
		}

		private void OnButtonPressed()
		{
			StartCoroutine(OnButtonPressedCoroutine());
		}

		private IEnumerator OnButtonPressedCoroutine()
		{
			bool isDone = false;
			m_animator.Play("Close", () => { isDone = true; });
			while (!isDone) { yield return null; }

			isDone = false;
			GeneralRoot.Instance.SceneController.RemoveScene(this, () => { isDone = true; });
			while (!isDone) { yield return null; }
		}
	}
}