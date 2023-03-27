using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CommonUI
{
	[System.Serializable, RequireComponent(typeof(TextExpansion))]
	public class LocalizeText : MonoBehaviour
	{
		/// <summary>
		/// ローカライズID
		/// </summary>
		[SerializeField]
		private int m_id = -1;

		/// <summary>
		/// フォーマットの値
		/// </summary>
		[SerializeField]
		private string[] m_param;

		/// <summary>
		/// ローカライズテキスト
		/// </summary>
		[SerializeField]
		private TextExpansion m_text;



		public string text { set { m_text.text = value; } }
		public Color color { set { m_text.color = value; } }

		private void Reset()
		{
			if (m_text == null)
			{
				m_text = GetComponent<TextExpansion>();
			}
		}

		/// <summary>
		/// 文字設定
		/// </summary>
		/// <param name="id"></param>
		/// <param name="param"></param>
		public void SetText(int id, string[] param)
		{
			m_id = id;
			m_param = param;
			SetText();
		}

		/// <summary>
		/// 文字設定
		/// </summary>
		/// <param name="id"></param>
		public void SetText(int id)
		{
			m_id = id;
			m_param = null;
			SetText();
		}

		/// <summary>
		/// 文字設定
		/// </summary>
		public void SetText()
		{
			if (m_id == -1)
			{
				return;
			}

			string localizeText = GetString(m_id);
			if (string.IsNullOrEmpty(localizeText) == false)
			{
				if (m_param != null && m_param.Length > 0)
				{
					m_text.text = string.Format(localizeText, m_param);
				}
				else
				{
					m_text.text = localizeText;
				}
			}
			else
			{
				m_text.text = "";
			}
		}

		public static string GetString(int id)
		{
			if (id == -1)
			{
				return "";
			}

			var localizeMaster = GeneralRoot.Master.Localize;
			if (localizeMaster == null)
			{
				// マスターデータロード前に呼ばれている（CommonDialogの「所持数」等）
				return "";
			}

			var localizeMasterData = localizeMaster.Find(id);
			if (localizeMasterData == null)
			{
				Debug.LogError(string.Format("存在しないローカライズID（ID:{0}）", id));
				return id.ToString();
			}

			//var saveData = GeneralRoot.Instance.UserData;
			int index = 0;//(int)saveData.Param.m_language;
			return localizeMasterData.Texts[index];
		}
	}
}