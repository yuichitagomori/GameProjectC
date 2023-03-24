using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace data.master
{
	public class CustomizePartsEffect : MasterDataBase<CustomizePartsEffect.Data>
	{
		[System.Serializable]
		public class Data : DataBase
		{
			[SerializeField]
			private int m_category = 0;
			public int Category => m_category;

			[SerializeField]
			private int m_param = 0;
			public int Param => m_param;


			public Data(
				int id,
				int category,
				int param)
			{
				m_name = id.ToString();
				m_id = id;
				m_category = category;
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
			int category = int.Parse(csvParam[1]);
			int param = int.Parse(csvParam[2]);
			return new Data(
				id,
				category,
				param);
		}
	}
}