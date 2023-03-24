using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace data.master
{
	public class CustomizeParts : MasterDataBase<CustomizeParts.Data>
	{
		[System.Serializable]
		public class Data : DataBase
		{
			[SerializeField]
			private int m_spriteId = 0;
			public int SpriteId => m_spriteId;

			[SerializeField]
			private int m_effectId = 0;
			public int EffectId => m_effectId;

			[SerializeField]
			private int m_areaId = 0;
			public int AreaId => m_areaId;


			public Data(
				int id,
				int spriteId,
				int effectId,
				int areaId)
			{
				m_name = id.ToString();
				m_id = id;
				m_spriteId = spriteId;
				m_effectId = effectId;
				m_areaId = areaId;
			}
		}

		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="csvParam"></param>
		public override Data CreateData(string[] csvParam)
		{
			int id = int.Parse(csvParam[0]);
			int spriteId = int.Parse(csvParam[1]);
			int effectId = int.Parse(csvParam[2]);
			int areaId = int.Parse(csvParam[3]);
			return new Data(
				id,
				spriteId,
				effectId,
				areaId);
		}
	}
}