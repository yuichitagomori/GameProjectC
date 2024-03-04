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
			private string m_sceneName;
			public string SceneName => m_sceneName;

			[SerializeField]
			private int m_movieDataId;
			public int MovieDataId => m_movieDataId;

			[SerializeField]
			private int m_rewardDataId;
			public int RewardDataId => m_rewardDataId;

			[SerializeField]
			private int[] m_checkSheetBugIds;
			public int[] CheckSheetBugIds => m_checkSheetBugIds;

			[SerializeField]
			private int m_tryCountMin;
			public int TryCountMin => m_tryCountMin;

			[SerializeField]
			private int m_tryCountMax;
			public int TryCountMax => m_tryCountMax;



			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="id"></param>
			/// <param name="sceneName"></param>
			/// <param name="movieDataId"></param>
			/// <param name="rewardDataId"></param>
			/// <param name="checkSheetBugIds"></param>
			/// <param name="tryCountMin"></param>
			/// <param name="tryCountMax"></param>
			public Data(
				int id,
				string sceneName,
				int movieDataId,
				int rewardDataId,
				int[] checkSheetBugIds,
				int tryCountMin,
				int tryCountMax)
			{
				m_name = id.ToString();
				m_id = id;
				m_sceneName = sceneName;
				m_movieDataId = movieDataId;
				m_rewardDataId = rewardDataId;
				m_checkSheetBugIds = checkSheetBugIds;
				m_tryCountMin = tryCountMin;
				m_tryCountMax = tryCountMax;
			}
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="csvParam"></param>
		public override Data CreateData(string[] csvParam)
		{
			int id = int.Parse(csvParam[0]);
			string sceneName = csvParam[1];
			int movieDataId = int.Parse(csvParam[2]);
			int rewardDataId = int.Parse(csvParam[3]);
			string[] checkSheetBugIdStrings = csvParam[4].Split(',');
			List<int> checkSheetBugIdList = new List<int>();
			for (int i = 0; i < checkSheetBugIdStrings.Length; ++i)
			{
				checkSheetBugIdList.Add(int.Parse(checkSheetBugIdStrings[i]));
			}
			int tryCountMin = int.Parse(csvParam[5]);
			int tryCountMax = int.Parse(csvParam[6]);
			return new Data(
				id,
				sceneName,
				movieDataId,
				rewardDataId,
				checkSheetBugIdList.ToArray(),
				tryCountMin,
				tryCountMax);
		}
	}
}