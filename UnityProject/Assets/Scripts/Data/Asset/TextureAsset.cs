using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// テクスチャアセット
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObject/AssetData/TextureAsset")]
public class TextureAsset : ScriptableObject
{
	[System.Serializable]
	public class Data
	{
		[SerializeField]
		private string m_name = "";

		[SerializeField]
		private Texture2D m_texture;
		public Texture2D Texture { get { return m_texture; } }

		[SerializeField]
		private Sprite m_sprite;
		public Sprite Sprite { get { return m_sprite; } }


		public Data(string _name, Texture2D _texture, Sprite _sprite)
		{
			m_name = _name;
			m_texture = _texture;
			m_sprite = _sprite;
		}
	}

	[SerializeField]
	private List<Data> m_list;
	public List<Data> List { get { return m_list; } }


#if UNITY_EDITOR
	[SerializeField]
	private string m_listPathFormat;

	[SerializeField]
	private int m_listCount;



	[ContextMenu("Setup")]
	public void Setup()
	{
		m_list.Clear();
		for (int i = 0; i < m_listCount; ++i)
		{
			string path = string.Format(m_listPathFormat, i + 1);
			Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
			Sprite sprite = (Sprite)AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
			string name = string.Format("{0}_{1}_{2}",
				i,
				(texture != null) ? texture.name : "null",
				(sprite != null) ? sprite.name : "null");
			Data data = new Data(name, texture, sprite);
			m_list.Add(data);
		}

		// ダーティフラグをセットし、アセット保存
		EditorUtility.SetDirty(this);
		AssetDatabase.SaveAssets();
	}
#endif
}
