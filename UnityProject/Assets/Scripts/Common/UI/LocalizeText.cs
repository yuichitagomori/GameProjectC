using UnityEngine;
using UnityEngine.Events;

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
		public TextExpansion Text => m_text;


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

		public void PlayProgression(float waitTime, UnityAction callback)
		{
			string str = GetString(m_id);
			m_text.PlayProgression(str, waitTime, callback);
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