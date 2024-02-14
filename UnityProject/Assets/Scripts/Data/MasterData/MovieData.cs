using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace data.master
{
	public class MovieData : MasterDataBase<MovieData.Data>
	{
		[System.Serializable]
		public class Data : DataBase
		{
			[SerializeField]
			private string m_param;
			public string Param => m_param;

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="id"></param>
			/// <param name="param"></param>
			public Data(
				int id,
				string param)
			{
				m_name = string.Format("{0}: {1}", id.ToString("000"), param);
				m_id = id;
				m_param = param;
			}
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="csvParam"></param>
		public override Data CreateData(string[] csvParam)
		{
			int id = int.Parse(csvParam[0]);
			string param = csvParam[1];
			return new Data(id, param);
		}
	}
}