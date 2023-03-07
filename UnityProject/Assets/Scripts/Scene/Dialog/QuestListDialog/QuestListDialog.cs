using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene.dialog
{
	public class QuestListDialog : SceneBase
	{
		public class Data
		{
			public class Quest
			{
				public enum State
				{
					Unprogressed,
					Inprogress,
					Clear
				}

				public class Reward
				{
					private int m_itemCategory;
					public int ItemCategory => m_itemCategory;

					private int m_itemId;
					public int ItemId => m_itemId;

					public Reward(int itemCategory, int itemId)
					{
						m_itemCategory = itemCategory;
						m_itemId = itemId;
					}
				}

				private string m_title = "";

				public string Title => m_title;

				private string m_info = "";

				public string Info => m_info;

				private int m_difficultyRank;
				public int DifficultyRank => m_difficultyRank;

				private State m_questState;
				public State QuestState => m_questState;

				private Reward[] m_rewards;
				public Reward[] Rewards => m_rewards;

				public Quest(string title, string info, int difficultyRank, State questState, Reward[] rewards)
				{
					m_title = title;
					m_info = info;
					m_difficultyRank = difficultyRank;
					m_questState = questState;
					m_rewards = rewards;
				}
			}

			
			public enum Type
			{
				Notification,   // 通知（閉じるボタンのみ）
				Confirmation,   // 確認（任意テキストボタンと閉じるボタン）
				YesNo,          // ２択（２つの任意テキストボタン）
			}

			private string m_title = "";

			public string Title => m_title;

			private Quest[] m_quests;

			public Quest[] Quests => m_quests;

			private UnityAction<int> m_receiveEvent;

			public UnityAction<int> ReceiveEvent => m_receiveEvent;



			public Data(string title, Quest[] quests, UnityAction<int> receiveEvent)
			{
				m_title = title;
				m_quests = quests;
				m_receiveEvent = receiveEvent;
			}
		}

		/// <summary>
		/// アニメーター
		/// </summary>
		[SerializeField]
		private AnimatorExpansion m_animator;

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
		/// 依頼書リスト
		/// </summary>
		[SerializeField]
		private Common.ElementList m_questElementList;

		/// <summary>
		/// タブボタン（詳細）
		/// </summary>
		[SerializeField]
		private CommonUI.ButtonExpansion[] m_tabButtons;

		/// <summary>
		/// タブボタン（詳細）
		/// </summary>
		[SerializeField]
		private CommonUI.SwitchSprite[] m_tabButtonSwitchs;

		/// <summary>
		/// 詳細表記
		/// </summary>
		[SerializeField]
		private GameObject[] m_infoObjects;

		/// <summary>
		/// 依頼書説明テキスト
		/// </summary>
		[SerializeField]
		private Text m_infoText;

		/// <summary>
		/// 受注ボタン
		/// </summary>
		[SerializeField]
		private CommonUI.ButtonExpansion m_receiveButton;



		/// <summary>
		/// Finish時
		/// </summary>
		private UnityAction m_finishCallback = null;

		private Data m_data = null;

		private int m_selectQuestIndex = 0;

		private int m_selectTabButtonIndex = 0;



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
			m_closeButton.SetupClickEvent(() =>
			{
				m_data.ReceiveEvent(-1);
				m_sceneController.RemoveScene(this, null);
			});
			m_closeButton.interactable = false;

			m_titleText.text = m_data.Title;

			var elements = m_questElementList.GetElements();
			for (int i = 0; i < elements.Count; ++i)
			{
				if (i >= m_data.Quests.Length)
				{
					elements[i].SetActive(false);
					continue;
				}

				elements[i].SetActive(true);
				var questListElement = elements[i].GetComponent<QuestListElement>();
				var data = new QuestListElement.Data(
					title: m_data.Quests[i].Title,
					difficultyRank: m_data.Quests[i].DifficultyRank,
					questState: m_data.Quests[i].QuestState,
					onSelectEvent: OnQuestElementSelect,
					index: i);
				questListElement.Setup(data);
				questListElement.UpdateSelect(i == m_selectQuestIndex);
			}

			for (int i = 0; i < m_tabButtons.Length; ++i)
			{
				int index = i;
				m_tabButtons[i].SetupClickEvent(() =>
				{
					OnTabButtonPressed(index);
				});
			}

			m_receiveButton.SetupClickEvent(() =>
			{
				m_data.ReceiveEvent(m_selectQuestIndex);
				m_sceneController.RemoveScene(this, null);
			});
			m_receiveButton.interactable = false;

			UpdateInfo(true);

			bool isDone = false;
			m_animator.Play("In", () => { isDone = true; });
			while (!isDone) { yield return null; }

			m_closeButton.interactable = true;
			m_receiveButton.interactable = true;

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

		private void OnTabButtonPressed(int index)
		{
			if (m_selectTabButtonIndex == index)
			{
				return;
			}

			m_selectTabButtonIndex = index;
			UpdateInfo(true);
		}

		private void UpdateInfo(bool isTabChange)
		{
			if (isTabChange == true)
			{
				for (int i = 0; i < m_tabButtonSwitchs.Length; ++i)
				{
					m_tabButtonSwitchs[i].Setup(i == m_selectTabButtonIndex);
					m_infoObjects[i].SetActive(i == m_selectTabButtonIndex);
				}
			}

			if (m_selectTabButtonIndex == 0)
			{
				m_infoText.text = m_data.Quests[m_selectQuestIndex].Info;
			}
			else
			{

			}
		}

		private void OnQuestElementSelect(int index)
		{
			int beforeIndex = m_selectQuestIndex;
			m_selectQuestIndex = index;

			if (beforeIndex != m_selectQuestIndex)
			{
				var elements = m_questElementList.GetElements();
				elements[beforeIndex].GetComponent<QuestListElement>().UpdateSelect(false);
				elements[m_selectQuestIndex].GetComponent<QuestListElement>().UpdateSelect(true);
				UpdateInfo(false);
			}
		}
	}
}