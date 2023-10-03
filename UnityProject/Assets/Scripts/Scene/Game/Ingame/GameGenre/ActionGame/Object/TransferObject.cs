using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.actiongame
{
	[System.Serializable]
	public class TransferObject : ObjectBase
	{
		private UnityAction<int> m_transferEvent;



		public void Initialize(UnityAction<int> transferEvent)
		{
			m_transferEvent = transferEvent;
			m_fbx.Anime.PlayLoop("InactiveLoop");
			base.Initialize(OnEvent);
		}

		private void OnEvent(string param)
		{
			string[] actionStrings = param.Split('_');
			switch (actionStrings[0])
			{
				case "Transfer":
					{
						int index = int.Parse(actionStrings[1]);
						m_transferEvent(index);
						break;
					}
				case "In":
					{
						m_fbx.Anime.Play("ActiveIn");
						break;
					}
				case "Out":
					{
						m_fbx.Anime.Play("ActiveOut", () =>
						{
							m_fbx.Anime.PlayLoop("InactiveLoop");
						});
						break;
					}
			}
		}
	}
}
