using UnityEngine;
using System.Collections.Generic;

namespace data.master
{
	/// <summary>
	/// ローカライズ情報
	/// </summary>
	public class LocalizeData : MasterDataBase<LocalizeData.Data>
	{
		/// <summary>
		/// 使用言語
		/// </summary>
		public enum LanguageType
		{
			JA = 0, // 日本語
			EN, // 英語
			ZH, // 中国語
		}

		/// <summary>
		/// アイテム情報内容
		/// </summary>
		[System.Serializable]
		public class Data : DataBase
		{
			/// <summary>
			/// ローカライズ文字列
			/// </summary>
			[SerializeField]
			private string[] m_texts;
			public string[] Texts => m_texts;

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="id"></param>
			/// <param name="texts"></param>
			public Data(
				int id,
				string[] texts)
			{
				m_name = id.ToString();
				m_id = id;
				m_texts = texts;
			}
		}


		/// <summary>
		/// 読み込み
		/// </summary>
		/// <param name="csvParam"></param>
		public override Data CreateData(string[] csvParam)
		{
			int id = int.Parse(csvParam[0]);
			List<string> textList = new List<string>();
			for (int i = 1; i < csvParam.Length; ++i)
			{
				textList.Add(csvParam[i].Replace("\\n", "\n"));
			}
			return new Data(
				id,
				textList.ToArray());
		}
	}
}