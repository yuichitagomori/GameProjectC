using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame
{
	[System.Serializable]
	public class IngameApp : MonoBehaviour
	{
		[SerializeField]
		private List<app.AppIcon> m_appIconList = null;



		private Game.GameMode m_gameMode = Game.GameMode.None;



		public void Initialize()
		{
			m_gameMode = Game.GameMode.None;
			for (int i = 0; i < m_appIconList.Count; ++i)
			{
				m_appIconList[i].Initialize();
			}
		}

		public void UpdateMode(Game.GameMode mode)
		{
			m_gameMode = mode;
		}

		public void UpdateApp()
		{
			//var appModeList = GeneralRoot.Instance.UserData.Data.AppModeList;
			//Debug.Log("appModeList = " + appModeList);

			//var enumList = System.Enum.GetValues(typeof(data.UserData.LocalSaveData.AppMode));
			//for (int i = 0; i < enumList.Length; ++i)
			//{
			//	data.UserData.LocalSaveData.AppMode enumValue = (data.UserData.LocalSaveData.AppMode)enumList.GetValue(i);
			//	if (enumValue == data.UserData.LocalSaveData.AppMode.None)
			//	{
			//		continue;
			//	}
			//	int index = i - 1;
			//	bool isActive = (appModeList & enumValue) == enumValue;
			//	m_appIconList[index].UpdateMode(isActive);
			//	Debug.Log("enumValue = " + enumValue);
			//	Debug.Log("isActive = " + isActive);
			//}
		}
	}
}
