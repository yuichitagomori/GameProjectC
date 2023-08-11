using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace scene.game.outgame.window
{
	[System.Serializable]
	public class MessageElement : MonoBehaviour
	{
		[System.Serializable]
		public class Node
		{
			public class Data
			{
				private bool m_isMine;
				public bool IsMine => m_isMine;

				private string m_message;
				public string Message => m_message;

				public Data(bool isMine, string message)
				{
					m_isMine = isMine;
					m_message = message;
				}
			}

			[SerializeField]
			private GameObject m_object;

			[SerializeField]
			private Image m_iconImage;

			[SerializeField]
			private CommonUI.TextExpansion m_messageText;

			public void SetActive(bool value)
			{
				m_object.SetActive(value);
			}

			public void Setting(string message)
			{
				m_messageText.text = message;
			}
		}

		[SerializeField]
		private Common.AnimatorExpansion m_animator;

		[SerializeField]
		private Node m_mineNode;

		[SerializeField]
		private Node m_otherNode;



		public void Initialize()
		{
		}

		public void Setting(Node.Data data, bool isIn)
		{
			m_mineNode.SetActive(data.IsMine);
			m_otherNode.SetActive(!data.IsMine);
			if (data.IsMine == true)
			{
				m_mineNode.Setting(data.Message);
				if (isIn == true)
				{
					m_animator.Play("MineIn");
				}
			}
			else
			{
				m_otherNode.Setting(data.Message);
				if (isIn == true)
				{
					m_animator.Play("OtherIn");
				}
			}
		}
	}
}
