using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace data
{
	/// <summary>
	/// ローカライズ情報
	/// </summary>
	public class LocalizeData : ScriptableObjectData
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
		public class Table
		{
			[SerializeField, HideInInspector]
			public string m_name = "";

			/// <summary>
			/// ID
			/// </summary>
			[SerializeField]
			private int m_id;
			public int Id
			{
				get { return m_id; }
			}

			/// <summary>
			/// ローカライズ文字列
			/// </summary>
			[SerializeField]
			private List<string> m_textList;
			public List<string> TextList
			{
				get { return m_textList; }
			}

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="_id"></param>
			/// <param name="_textList"></param>
			public Table(
				int _id,
				List<string> _textList)
			{
				m_name = _id.ToString();
				m_id = _id;
				m_textList = _textList;
			}
		}

		[SerializeField]
		private List<Table> m_tables;
		public List<Table> Tables
		{
			get { return m_tables; }
		}

		/// <summary>
		/// データ取得
		/// </summary>
		/// <param name="_id"></param>
		/// <returns></returns>
		public Table Find(int _id)
		{
			//Table table = m_tables.Find(d => d.Id == _id);
			//if (table == null)
			//{
			//	Debug.LogError("Not Found:id = " + _id);
			//}
			//return table;
			return m_tables[_id - 1];
		}


		/// <summary>
		/// 読み込み
		/// </summary>
		/// <param name="_csv"></param>
		public override void Load(List<List<string>> _csv)
		{
			m_tables = new List<Table>();
			foreach (List<string> strings in _csv)
			{
				int id = int.Parse(strings[0]);
				List<string> textList = new List<string>();
				for (int i = 1; i < strings.Count; ++i)
				{
					textList.Add(strings[i].Replace("\\n", "\n"));
				}
				Table info = new Table(
					id,
					textList);
				m_tables.Add(info);
			}
		}
	}
}