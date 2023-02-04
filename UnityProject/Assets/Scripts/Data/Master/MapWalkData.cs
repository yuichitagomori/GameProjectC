using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace data
{
	/// <summary>
	/// マップデータ
	/// </summary>
	public class MapWalkData : ScriptableObjectData
	{
		/// <summary>
		/// マップ歩行　行データ
		/// </summary>
		[System.Serializable]
		private class Row
		{
			public List<short> m_column;
		}

		[SerializeField]
		private int m_width = 0;
		public int Width { get { return m_width; } }

		[SerializeField]
		private int m_height = 0;
		public int Height { get { return m_height; } }

		[SerializeField]
		private List<Row> m_param;
		public int[,] Param
		{
			get
			{
				int countY = m_param.Count;
				int countX = m_param[0].m_column.Count;
				int[,] param = new int[countY, countX];
				for (int y = 0; y < countY; ++y)
				{
					for (int x = 0; x < countX; ++x)
					{
						param[y, x] = m_param[y].m_column[x];
					}
				}
				return param;
			}
		}

		/// <summary>
		/// 読み込み
		/// </summary>
		/// <param name="_csv"></param>
		public override void Load(List<List<string>> _csv)
		{
			m_param = new List<Row>();
			m_width = int.Parse(_csv[0][0]);
			m_height = int.Parse(_csv[0][1]);
			for (int i = 1; i < m_height + 1; ++i)
			{
				Row row = new Row();
				row.m_column = new List<short>();
				for (int j = 0; j < m_width; ++j)
				{
					row.m_column.Add(short.Parse(_csv[i][j]));
				}
				m_param.Add(row);
			}
		}
	}
}