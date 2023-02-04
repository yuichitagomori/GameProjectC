using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace data
{
	/// <summary>
	/// ファイル名リストデータ
	/// </summary>
	public class FileNameListData : ScriptableObjectData
	{
		/// <summary>
		/// ファイルデータ
		/// </summary>
		[System.Serializable]
		public class Data
		{
			/// <summary>
			/// ファイル名
			/// </summary>
			[SerializeField]
			private string m_name;
			public string Name { get { return m_name; } }

			/// <summary>
			/// 配列番号
			/// </summary>
			[SerializeField]
			private int m_index;
			public int Index { get { return m_index; } }

			public Data(string _name, int _index)
			{
				m_name = _name;
				m_index = _index;
			}
		}

		/// <summary>
		/// ファイルデータリスト
		/// </summary>
		[SerializeField]
		private List<Data> m_dataList;
		public List<Data> DataList { get { return m_dataList; } }



		/// <summary>
		/// 読み込み
		/// </summary>
		/// <param name="_csv"></param>
		public override void Load(List<List<string>> _csv)
		{
			m_dataList = new List<Data>();
			for (int i = 0; i < _csv.Count; ++i)
			{
				m_dataList.Add(new Data(_csv[i][0], i));
			}
		}
	}
}