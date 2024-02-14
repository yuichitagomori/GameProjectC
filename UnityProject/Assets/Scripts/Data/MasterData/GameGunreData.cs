using UnityEngine;
using System.Collections.Generic;

namespace data.master
{
	public class GameGunreData : MasterDataBase<GameGunreData.Data>
	{
		[System.Serializable]
		public class Data : DataBase
		{
			[SerializeField]
			private int m_movieDataId;
			public int MovieDataId => m_movieDataId;

			[SerializeField]
			private int m_rewardDataId;
			public int RewardDataId => m_rewardDataId;

			[SerializeField]
			private int[] m_checkSheetBugIds;
			public int[] CheckSheetBugIds => m_checkSheetBugIds;

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="id"></param>
			/// <param name="movieDataId"></param>
			/// <param name="rewardDataId"></param>
			/// <param name="checkSheetBugIds"></param>
			public Data(
				int id,
				int movieDataId,
				int rewardDataId,
				int[] checkSheetBugIds)
			{
				m_name = id.ToString();
				m_id = id;
				m_movieDataId = movieDataId;
				m_rewardDataId = rewardDataId;
				m_checkSheetBugIds = checkSheetBugIds;
			}
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="csvParam"></param>
		public override Data CreateData(string[] csvParam)
		{
			int id = int.Parse(csvParam[0]);
			int movieDataId = int.Parse(csvParam[1]);
			int rewardDataId = int.Parse(csvParam[2]);
			string[] checkSheetBugIdStrings = csvParam[3].Split(',');
			List<int> checkSheetBugIdList = new List<int>();
			for (int i = 0; i < checkSheetBugIdStrings.Length; ++i)
			{
				checkSheetBugIdList.Add(int.Parse(checkSheetBugIdStrings[i]));
			}
			return new Data(
				id,
				movieDataId,
				rewardDataId,
				checkSheetBugIdList.ToArray());
		}
	}
}