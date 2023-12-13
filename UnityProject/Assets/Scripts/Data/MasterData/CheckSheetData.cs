using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace data.master
{
	public class CheckSheetData : MasterDataBase<CheckSheetData.Data>
	{
		[System.Serializable]
		public class Data : DataBase
		{
			[System.Serializable]
			public class CheckData
			{
				[SerializeField]
				private int m_accuracyLevel;
				public int AccuracyLevel => m_accuracyLevel;

				[SerializeField]
				private int m_bugCountMin;
				public int BugCountMin => m_bugCountMin;

				[SerializeField]
				private int m_bugCountMax;
				public int BugCountMax => m_bugCountMax;

				[SerializeField]
				private int[] m_busIds;
				public int[] BudIds => m_busIds;
			}

			[SerializeField]
			private CheckData[] m_checkDatas;
			public CheckData[] CheckDatas => m_checkDatas;
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