using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable, RequireComponent(typeof(Text))]
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
	private Text m_text;



	public string text { set { m_text.text = value; } }
	public Color color { set { m_text.color = value; } }

	private void Reset()
	{
		if (m_text == null)
		{
			m_text = GetComponent<Text>();
		}
	}

	/// <summary>
	/// 文字設定
	/// </summary>
	/// <param name="_id"></param>
	/// <param name="_param"></param>
	public void SetText(int _id, string[] _param)
	{
		m_id = _id;
		m_param = _param;
		SetText();
	}

	/// <summary>
	/// 文字設定
	/// </summary>
	/// <param name="_id"></param>
	public void SetText(int _id)
	{
		m_id = _id;
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

	public static string GetString(int _id)
	{
		if (_id == -1)
		{
			return "";
		}

		string localizeText = "";
		var localizeMaster = GeneralRoot.Instance.MasterData.LocalizeData;
		if (localizeMaster == null)
		{
			// マスターデータロード前に呼ばれている（CommonDialogの「所持数」等）
			return "";
		}

		var localizeMasterData = localizeMaster.Find(_id);
		if (localizeMasterData != null)
		{
			var saveData = GeneralRoot.Instance.UserData;
			int index = 0;//(int)saveData.Param.m_language;
			localizeText = localizeMasterData.TextList[index];
		}
		else
		{
			Debug.LogError(string.Format("存在しないローカライズID（ID:{0}）", _id));
			localizeText = _id.ToString();
		}
		return localizeText;
	}
}
