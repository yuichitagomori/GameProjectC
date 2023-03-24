using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace data.master
{
	public class CustomizePartsArea : MasterDataBase<CustomizePartsArea.Data>
	{
		[System.Serializable]
		public class Data : DataBase
		{
			[SerializeField]
			private Grid[] m_grids;
			public Grid[] Grids => m_grids;


			public Data(
				int id,
				Grid[] grids)
			{
				m_name = id.ToString();
				m_id = id;
				m_grids = grids;
			}
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="csvParam"></param>
		public override Data CreateData(string[] csvParam)
		{
			int id = int.Parse(csvParam[0]);
			List<Grid> gridList = new List<Grid>();
			for (int j = 1; j < csvParam.Length; ++j)
			{
				if (string.IsNullOrEmpty(csvParam[j]) == true)
				{
					break;
				}
				var grid = JsonUtility.FromJson<Grid>(csvParam[j]);
				gridList.Add(grid);
			}
			return new Data(
				id,
				gridList.ToArray());
		}
	}
}