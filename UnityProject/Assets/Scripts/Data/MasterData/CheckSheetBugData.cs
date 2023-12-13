using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace data.master
{
	public class CheckSheetBugData : MasterDataBase<CheckSheetBugData.Data>
	{
		[System.Serializable]
		public class Data : DataBase
		{
			[SerializeField, Multiline(3)]
			private string m_info;
			public string Info => m_info;
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