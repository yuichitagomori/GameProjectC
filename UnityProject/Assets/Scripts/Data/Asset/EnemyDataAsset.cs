using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// テクスチャアセット
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObject/AssetData/EnemyDataAsset")]
public class EnemyDataAsset : ScriptableObject
{
	[System.Serializable]
	public class Data
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
		private int m_enemyId = -1;
		public int EnemyId => m_enemyId;

		[SerializeField]
		private ColorData[] m_colorDatas = null;
		public ColorData[] ColorDatas => m_colorDatas;

		public Data(int enemyId, ColorData[] colorDatas)
		{
			m_enemyId = enemyId;
			m_colorDatas = colorDatas;
		}
	}

	[SerializeField]
	private List<Data> m_list;
	public List<Data> List { get { return m_list; } }


#if UNITY_EDITOR
	
	[SerializeField]
	private int[] m_setupEnemyIds;
	public int[] SetupEnemyIds => m_setupEnemyIds;

	[ContextMenu("Setup")]
	public void Setup()
	{
		m_list.Clear();
		for (int i = 0; i < m_setupEnemyIds.Length; ++i)
		{
			List<Data.ColorData> colorDataList = new List<Data.ColorData>();
			for (int j = 0; j < 1000; ++j)
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
					colorId: j + 1,
					colors1: color1,
					colors2: color2);
				colorDataList.Add(colorData);
			}
			Data data = new Data(
				enemyId: m_setupEnemyIds[i],
				colorDatas: colorDataList.ToArray());
			m_list.Add(data);
		}

		// ダーティフラグをセットし、アセット保存
		EditorUtility.SetDirty(this);
		AssetDatabase.SaveAssets();
	}
#endif
}
