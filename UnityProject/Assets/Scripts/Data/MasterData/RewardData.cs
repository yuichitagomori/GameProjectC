using UnityEngine;

namespace data.master
{
	public class RewardData : MasterDataBase<RewardData.Data>
	{
		[System.Serializable]
		public class Data : DataBase
		{
			[SerializeField]
			private int m_reviewTextId_1;
			public int ReviewTextId_1 => m_reviewTextId_1;

			[SerializeField]
			private int m_reviewStar_1;
			public int ReviewStar_1 => m_reviewStar_1;

			[SerializeField]
			private int m_reviewTextId_2;
			public int ReviewTextId_2 => m_reviewTextId_2;

			[SerializeField]
			private int m_reviewStar_2;
			public int ReviewStar_2 => m_reviewStar_2;

			[SerializeField]
			private int m_reviewTextId_3;
			public int ReviewTextId_3 => m_reviewTextId_3;

			[SerializeField]
			private int m_reviewStar_3;
			public int ReviewStar_3 => m_reviewStar_3;

			[SerializeField]
			private int m_money;
			public int Money => m_money;

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="id"></param>
			/// <param name="reviewTextId_1"></param>
			/// <param name="reviewStar_1"></param>
			/// <param name="reviewTextId_2"></param>
			/// <param name="reviewStar_2"></param>
			/// <param name="reviewTextId_3"></param>
			/// <param name="reviewStar_3"></param>
			/// <param name="money"></param>
			public Data(
				int id,
				int reviewTextId_1,
				int reviewStar_1,
				int reviewTextId_2,
				int reviewStar_2,
				int reviewTextId_3,
				int reviewStar_3,
				int money)
			{
				m_name = id.ToString();
				m_id = id;
				m_reviewTextId_1 = reviewTextId_1;
				m_reviewStar_1 = reviewStar_1;
				m_reviewTextId_2 = reviewTextId_2;
				m_reviewStar_2 = reviewStar_2;
				m_reviewTextId_3 = reviewTextId_3;
				m_reviewStar_3 = reviewStar_3;
				m_money = money;
			}
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="csvParam"></param>
		public override Data CreateData(string[] csvParam)
		{
			int id = int.Parse(csvParam[0]);
			int reviewTextId_1 = int.Parse(csvParam[1]);
			int reviewStar_1 = int.Parse(csvParam[2]);
			int reviewTextId_2 = int.Parse(csvParam[3]);
			int reviewStar_2 = int.Parse(csvParam[4]);
			int reviewTextId_3 = int.Parse(csvParam[5]);
			int reviewStar_3 = int.Parse(csvParam[6]);
			int money = int.Parse(csvParam[7]);
			return new Data(
				id,
				reviewTextId_1,
				reviewStar_1,
				reviewTextId_2,
				reviewStar_2,
				reviewTextId_3,
				reviewStar_3,
				money);
		}
	}
}