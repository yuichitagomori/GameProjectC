using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.outgame.window
{
	[System.Serializable]
	public class MessageWindow : WindowBase
	{
		[SerializeField]
		private Common.ElementList m_elementList;



		private List<MessageElement.Node.Data> m_dataList = new List<MessageElement.Node.Data>();

		public new void Initialize(UnityAction holdCallback)
		{
			base.Initialize(holdCallback);

			var elements = m_elementList.GetElements();
			for (int i = 0; i < elements.Count; ++i)
			{
				elements[i].SetActive(false);
				var element = elements[i].GetComponent<MessageElement>();
				element.Initialize();
			}
		}

		private void Setting()
		{
			var elements = m_elementList.GetElements();
			for (int i = 0; i < elements.Count; ++i)
			{
				if (i < m_dataList.Count)
				{
					elements[i].SetActive(true);
					var element = elements[i].GetComponent<MessageElement>();
					element.Setting(m_dataList[i], (i == m_dataList.Count - 1));
				}
				else
				{
					elements[i].SetActive(false);
				}
			}
		}

		public void AddMessage(bool isMine, int messageId)
		{
			string message = "";
			if (messageId == 1)
			{
				message = "�N�ɂ͍�����\n<color=#DD4444>�f�o�b�O���</color>���s���Ă��炤�B";
			}
			else if (messageId == 2)
			{
				message = "�����ł��I";
			}
			else if (messageId == 3)
			{
				message = "�����҂��ĂȂ����c\n���j�^�[�E�B���h�E�́c�Ɓc";
			}
			else if (messageId == 4)
			{
				message = "�������A���ꂾ�B";
			}
			else if (messageId == 5)
			{
				message = "���傫�ȃ��j�^�[�E�B���h�E���W�J���ꂽ�Ǝv����\n�����ƌ����Ă��邩�ˁH";
			}
			else if (messageId == 6)
			{
				message = "�����Ă܂��I";
			}
			else if (messageId == 7)
			{
				message = "��낵���B\n�����̃��j�^�[�����ɉf���Ă���L�����N�^�[���N�̑���L�����N�^�[���B\n�܂��͂��̃L�����N�^�[�𑀍삵��\n�}�b�v�����R�ɕ�������Ă݂�Ƃ����B";
			}
			m_dataList.Add(new MessageElement.Node.Data(isMine, message));
			Setting();
		}
	}
}