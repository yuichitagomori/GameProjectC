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
				message = "君には今から\n<color=#DD4444>デバッグ作業</color>を行ってもらう。";
			}
			else if (messageId == 2)
			{
				message = "了解です！";
			}
			else if (messageId == 3)
			{
				message = "少し待ってなさい…\nモニターウィンドウは…と…";
			}
			else if (messageId == 4)
			{
				message = "あった、これだ。";
			}
			else if (messageId == 5)
			{
				message = "今大きなモニターウィンドウが展開されたと思うが\nちゃんと見えているかね？";
			}
			else if (messageId == 6)
			{
				message = "見えてます！";
			}
			else if (messageId == 7)
			{
				message = "よろしい。\nそこのモニター中央に映っているキャラクターが君の操作キャラクターだ。\nまずはそのキャラクターを操作して\nマップを自由に歩き回ってみるといい。";
			}
			m_dataList.Add(new MessageElement.Node.Data(isMine, message));
			Setting();
		}
	}
}