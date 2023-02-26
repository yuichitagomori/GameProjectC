using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene.dialog
{
	public class CommonDialog : SceneBase
	{
		public class Data
		{
			public enum Type
			{
				Notification,   // 通知（閉じるボタンのみ）
				Confirmation,   // 確認（任意テキストボタンと閉じるボタン）
				YesNo,          // ２択（２つの任意テキストボタン）
			}

			private string m_title = "";

			public string Title => m_title;

			private string m_message = "";

			public string Message => m_message;

			private Type m_dialogType = Type.Notification;

			public Type DialogType => m_dialogType;

			private string[] m_buttonNames;

			public string[] ButtonNames => m_buttonNames;

			private UnityAction[] m_buttonActions;

			public UnityAction[] ButtonActions => m_buttonActions;



			private Data(
				string title,
				string message,
				Type dialogType,
				string[] buttonNames,
				UnityAction[] buttonActions)
			{
				m_title = title;
				m_message = message;
				m_dialogType = dialogType;
				m_buttonNames = buttonNames;
				m_buttonActions = buttonActions;
			}

			public static Data CreateNotificationData(
				string title,
				string message)
			{
				return new Data(
					title,
					message,
					Type.Notification,
					null,
					null);
			}

			public static Data CreateConfirmationData(
				string title,
				string message,
				string buttonName,
				UnityAction buttonAction)
			{
				return new Data(
					title,
					message,
					Type.Confirmation,
					new string[] { buttonName },
					new UnityAction[] { buttonAction });
			}

			public static Data CreateYesNoData(
				string title,
				string message,
				string[] buttonNames,
				UnityAction[] buttonActions)
			{
				return new Data(
					title,
					message,
					Type.YesNo,
					buttonNames,
					buttonActions);
			}
		}

		/// <summary>
		/// アニメーター
		/// </summary>
		[SerializeField]
		private AnimatorExpansion m_animator;

		/// <summary>
		/// 閉じるボタンオブジェクト
		/// </summary>
		[SerializeField]
		private GameObject m_closeButtonObject;

		/// <summary>
		/// 閉じるボタン
		/// </summary>
		[SerializeField]
		private CommonUI.ButtonExpansion m_closeButton;

		/// <summary>
		/// タイトル
		/// </summary>
		[SerializeField]
		private Text m_titleText;

		/// <summary>
		/// メッセージ
		/// </summary>
		[SerializeField]
		private Text m_messageText;

		/// <summary>
		/// 閉じるボタンオブジェクト
		/// </summary>
		[SerializeField]
		private GameObject[] m_buttonObjects;

		/// <summary>
		/// ボタン
		/// </summary>
		[SerializeField]
		private CommonUI.ButtonExpansion[] m_buttons;

		/// <summary>
		/// ボタンテキスト
		/// </summary>
		[SerializeField]
		private Text[] m_buttonTexts;



		private Data m_data;

		private UnityAction m_finishCallback;

		/// <summary>
		/// 事前設定
		/// </summary>
		/// <param name="data"></param>
		/// <param name="finishCallback"></param>
		public void Setting(Data data, UnityAction finishCallback)
		{
			m_data = data;
			m_finishCallback = finishCallback;
		}

		public override void Ready(UnityAction callback)
		{
			StartCoroutine(ReadyCoroutine(callback));
		}

		private IEnumerator ReadyCoroutine(UnityAction callback)
		{
			m_titleText.text = m_data.Title;
			m_messageText.text = m_data.Message;

			if (m_data.DialogType == Data.Type.YesNo)
			{
				m_closeButtonObject.SetActive(false);
			}
			else
			{
				m_closeButtonObject.SetActive(true);
				m_closeButton.SetupClickEvent(() =>
				{
					m_sceneController.RemoveScene(this, null);
				});
				m_closeButton.interactable = false;
			}

			if (m_data.DialogType != Data.Type.Notification)
			{
				for (int i = 0; i < m_buttonObjects.Length; ++i)
				{
					int index = i;
					if (m_data.ButtonNames.Length <= index)
					{
						m_buttonObjects[index].SetActive(false);
						continue;
					}

					m_buttonObjects[index].SetActive(true);
					m_buttons[index].SetupClickEvent(() =>
					{
						OnButtonPressed(index);
						m_sceneController.RemoveScene(this, null);
					});
					m_buttonTexts[index].text = m_data.ButtonNames[index];
					m_buttons[index].interactable = false;
				}
			}
			else
			{
				for (int i = 0; i < m_buttonObjects.Length; ++i)
				{
					m_buttonObjects[i].SetActive(false);
				}
			}

			bool isDone = false;
			m_animator.Play("In", () => { isDone = true; });
			while (!isDone) { yield return null; }

			if (m_closeButtonObject.activeInHierarchy == true)
			{
				m_closeButton.interactable = true;
			}

			for (int i = 0; i < m_buttonObjects.Length; ++i)
			{
				if (m_buttonObjects[i].activeInHierarchy == true)
				{
					m_buttons[i].interactable = true;
				}
			}

			if (callback != null)
			{
				callback();
			}
		}

		public override void Go()
		{
		}

		public override void Finish(UnityAction callback)
		{
			StartCoroutine(FinishCoroutine(callback));
		}

		private IEnumerator FinishCoroutine(UnityAction callback)
		{
			bool isDone = false;
			m_animator.Play("Out", () => { isDone = true; });
			while (!isDone) { yield return null; }

			if (m_finishCallback != null)
			{
				m_finishCallback();
			}

			if (callback != null)
			{
				callback();
			}
		}

		private void OnButtonPressed(int index)
		{
			if (m_data.ButtonActions[index] != null)
			{
				m_data.ButtonActions[index]();
			}
			m_sceneController.RemoveScene(this, null);
		}
	}
}