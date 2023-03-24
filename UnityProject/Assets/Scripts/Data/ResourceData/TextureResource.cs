using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace data.resource
{
	/// <summary>
	/// テクスチャアセット
	/// </summary>
	public abstract class TextureResource : ResourceDataBase<TextureResource.Data>
	{
		[System.Serializable]
		public class Data : DataBase
		{
			[SerializeField]
			private Texture2D m_texture;
			public Texture2D Texture => m_texture;

			[SerializeField]
			private Sprite m_sprite;
			public Sprite Sprite => m_sprite;


			public Data(int id, Texture2D texture, Sprite sprite)
			{
				m_name = id.ToString();
				m_id = id;
				m_texture = texture;
				m_sprite = sprite;
			}
		}


#if UNITY_EDITOR
		protected Data CreateData(int id, string path)
		{
			Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
			Sprite sprite = (Sprite)AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
			if (texture == null && sprite == null)
			{
				return null;
			}
			return new Data(id, texture, sprite);
		}

		protected string CommonPath = "Assets/Contents/ResourceData/";
#endif
	}
}