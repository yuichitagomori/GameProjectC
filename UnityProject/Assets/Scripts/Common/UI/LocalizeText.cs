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
		/// ローカライズテキスト
		/// </summary>
		[SerializeField]
		private TextExpansion m_text;

		private void Reset()
		{
			if (m_text == null)
			{
				m_text = GetComponent<TextExpansion>();
			}
		}

		private void Awake()
		{
			m_text.text = GetString(m_id);
		}

		public static string GetString(int id)
		{
			if (id == -1)
			{
				return "";
			}

			var localizeMaster = GeneralRoot.Master.LocalizeData;
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

			var local = GeneralRoot.User.LocalSaveData;
			int index = (int)local.Language;
			return localizeMasterData.Texts[index];
		}
	}
}