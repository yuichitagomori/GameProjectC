using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace data.resource
{
	/// <summary>
	/// 色データ
	/// </summary>
	public class ColorResource : ResourceDataBase<ColorResource.Data>
	{
		[System.Serializable]
		public class Data : DataBase
		{
			[System.Serializable]
			public class ColorData
			{
				[SerializeField]
				private int m_colorId = -1;
				public int ColorId => m_colorId;

				[SerializeField]
				private Color m_colors1 = Color.white;
				public Color Colors1 => m_colors1;

				[SerializeField]
				private Color m_colors2 = Color.white;
				public Color Colors2 => m_colors2;

				public ColorData(int colorId, Color colors1, Color colors2)
				{
					m_colorId = colorId;
					m_colors1 = colors1;
					m_colors2 = colors2;
				}
			}

			[SerializeField]
			private ColorData[] m_colorDatas = null;
			//public ColorData[] ColorDatas => m_colorDatas;

			public Data(int id, ColorData[] colorDatas)
			{
				m_name = id.ToString();
				m_id = id;
				m_colorDatas = colorDatas;
			}

			public ColorData Find(int colorId)
			{
				return m_colorDatas.FirstOrDefault(d => d.ColorId == colorId);
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// 生成
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public override Data CreateData(int id)
		{
			if (id != 30 && id != 33)
			{
				return null;
			}
			List<Data.ColorData> colorDataList = new List<Data.ColorData>();
			for (int i = 0; i < 1000; ++i)
			{
				Color color1 = Color.white;
				Color color2 = Color.white;
				int tryCount = 0;
				int tryCountMax = 100;
				while (tryCount < tryCountMax)
				{
					color1 = new Color(
						r: (float)(UnityEngine.Random.Range(0, 32) * 8) / 256,
						g: (float)(UnityEngine.Random.Range(0, 32) * 8) / 256,
						b: (float)(UnityEngine.Random.Range(0, 32) * 8) / 256);
					color2 = new Color(
						r: (float)(UnityEngine.Random.Range(0, 32) * 8) / 256,
						g: (float)(UnityEngine.Random.Range(0, 32) * 8) / 256,
						b: (float)(UnityEngine.Random.Range(0, 32) * 8) / 256);
					var findColorData = colorDataList.Find(d =>
					{
						// 0.0625 = 16 / 256
						if (Mathf.Abs(color1.r - d.Colors1.r) <= 0.0625f &&
							Mathf.Abs(color1.g - d.Colors1.g) <= 0.0625f &&
							Mathf.Abs(color1.b - d.Colors1.b) <= 0.0625f &&
							Mathf.Abs(color2.r - d.Colors2.r) <= 0.0625f &&
							Mathf.Abs(color2.g - d.Colors2.g) <= 0.0625f &&
							Mathf.Abs(color2.b - d.Colors2.b) <= 0.0625f)
						{
							return true;
						}
						return false;
					});
					if (findColorData == null)
					{
						break;
					}
					tryCount++;
				}
				if (tryCount >= tryCountMax)
				{
					continue;
				}
				var colorData = new Data.ColorData(
					colorId: i + 1,
					colors1: color1,
					colors2: color2);
				colorDataList.Add(colorData);
			}
			return new Data(
				id: id,
				colorDatas: colorDataList.ToArray());
		}
#endif
	}
}