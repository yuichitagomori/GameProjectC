using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace data.master
{
	/// <summary>
	/// ファイル名リストデータ
	/// </summary>
	public class FileNameList : MasterDataBase<FileNameList.Data>
	{
		/// <summary>
		/// ファイルデータ
		/// </summary>
		[System.Serializable]
		public class Data : DataBase
		{
			public Data(int id, string name)
			{
				m_name = name;
				m_id = id;
			}
		}

		/// <summary>
		/// 読み込み
		/// </summary>
		/// <param name="csvParam"></param>
		public override Data CreateData(string[] csvParam)
		{
			int id = int.Parse(csvParam[0]);
			string name = csvParam[1];
			return new Data(id, name);
		}
	}
}