using UnityEngine;

namespace data.master
{
	public class CheckSheetBugData : MasterDataBase<CheckSheetBugData.Data>
	{
		public enum BugType
		{
			BGM				= 1,
			SE				= 2,
			Button			= 3,
			Window			= 4,
			RoreignObject	= 5,

			Collision		= 50,
			Animation		= 51,
		}

		[System.Serializable]
		public class Data : DataBase
		{
			[SerializeField]
			private int m_infoTextId;
			public int InfoTextId => m_infoTextId;

			[SerializeField]
			private int m_rewardDataId;
			public int RewardDataId => m_rewardDataId;

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="id"></param>
			/// <param name="infoTextId"></param>
			/// <param name="rewardDataId"></param>
			public Data(
				int id,
				int infoTextId,
				int rewardDataId)
			{
				m_name = id.ToString();
				m_id = id;
				m_infoTextId = infoTextId;
				m_rewardDataId = rewardDataId;
			}
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="csvParam"></param>
		public override Data CreateData(string[] csvParam)
		{
			int id = int.Parse(csvParam[0]);
			int infoTextId = int.Parse(csvParam[1]);
			int rewardDataId = int.Parse(csvParam[2]);
			return new Data(
				id,
				infoTextId,
				rewardDataId);
		}
	}
}