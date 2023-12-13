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
			private string[] m_paramStrings;
			public string[] ParamStrings => m_paramStrings;
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="csvParam"></param>
		public override Data CreateData(string[] csvParam)
		{
			return null;
		}
	}
}