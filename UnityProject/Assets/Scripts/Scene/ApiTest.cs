using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace scene
{
	public class ApiTest : SceneBase
	{
		private const string URL = "https://j07jxwh55l.execute-api.ap-southeast-2.amazonaws.com";

		[System.Serializable]
		public class DataBase
		{
			[System.Serializable]
			public class Data
			{
				public int id;
				public int max_count;
				public int now_count;
			}

			public Data[] datas;
		}
		[SerializeField]
		private DataBase m_putDatabase;

		[SerializeField]
		private int m_deleteId;

		[SerializeField]
		private CommonUI.TextExpansion m_messageText;

		[SerializeField]
		private CommonUI.ButtonExpansion m_putButton;

		[SerializeField]
		private CommonUI.ButtonExpansion m_deleteButton;

		[SerializeField]
		private CommonUI.ButtonExpansion m_getButton;



		private StringBuilder m_strBuilder = new StringBuilder();



		private void Start()
		{
			Ready(Go);
		}
		public override void Ready(UnityAction callback)
		{
			m_messageText.text = "";
			m_putButton.SetupClickEvent(PutButtonEvent);
			m_deleteButton.SetupClickEvent(DeleteButtonEvent);
			m_getButton.SetupClickEvent(GetButtonEvent);
			m_strBuilder.Clear();
		}

		public override void Go()
		{
		}

		private void PutButtonEvent()
		{
			string json = JsonUtility.ToJson(m_putDatabase);
			//string json = "{\"id\": 123, \"price\": 12345, \"name\": \"myitem\"}";
			byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
			string url = string.Format("{0}/{1}", URL, "items");
			UnityWebRequest request = UnityWebRequest.Put(url, jsonBytes);
			StartCoroutine(RequestCoroutine(request));
		}

		private void DeleteButtonEvent()
		{
			string url = string.Format("{0}/{1}/{2}", URL, "items", m_deleteId);
			UnityWebRequest request = UnityWebRequest.Delete(url);
			StartCoroutine(RequestCoroutine(request));
		}

		private void GetButtonEvent()
		{
			string url = string.Format("{0}/{1}", URL, "items");
			UnityWebRequest request = UnityWebRequest.Get(url);
			StartCoroutine(RequestCoroutine(request));
		}

		private IEnumerator RequestCoroutine(UnityWebRequest request)
		{
			//ヘッダーにタイプを設定
			request.SetRequestHeader("Content-Type", "application/json");

			yield return request.SendWebRequest();

			if (string.IsNullOrEmpty(request.error) == false)
			{
				m_strBuilder.AppendLine(string.Format("<color=#DD4444FF>error</color>:{0}", request.error));
				yield break;
			}

			m_strBuilder.AppendLine(string.Format("<color=#DDDD66FF>success</color>:{0}", request.downloadHandler.text));
			m_messageText.text = m_strBuilder.ToString();
		}
	}
}