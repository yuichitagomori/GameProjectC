using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;

namespace data
{
	/// <summary>
	/// ユーザーデータ
	/// </summary>
	public class UserData : MonoBehaviour
	{
		private string kAesIV = "pf69DL6GrWFyZcMK";
		private string kAesKey = "9Fix4L4HB4PKeKWY";

		[System.Serializable]
		public class LocalSave
		{
			[System.Serializable]
			public class SearchTargetData
			{
				[SerializeField]
				private int m_enemyId;
				public int EnemyId => m_enemyId;

				[SerializeField]
				private int m_controllId;
				public int ControllId => m_controllId;

				public SearchTargetData(int enemyId, int controllId)
				{
					m_enemyId = enemyId;
					m_controllId = controllId;
				}
			}

			[System.Serializable]
			public class Item
			{
				public enum CategoryType
				{
				}

				[SerializeField]
				private CategoryType m_category;
				public CategoryType Category => m_category;

				[SerializeField]
				private int m_id;
				public int Id => m_id;

				[SerializeField]
				private int m_num;
				public int Num => m_num;
			}

			[System.Serializable]
			public class UniqueItem
			{
				public enum CategoryType
				{
					CustomizeParts,
				}

				[SerializeField]
				private CategoryType m_category;
				public CategoryType Category => m_category;

				[SerializeField]
				private int m_id;
				public int Id => m_id;

				[SerializeField]
				private int m_uniqueId;
				public int UniqueId => m_uniqueId;
			}

			[SerializeField]
			private int m_stageId = -1;
			public int StageId => m_stageId;

			[SerializeField]
			private List<Item> m_itemList;
			public List<Item> ItemList => m_itemList;

			[SerializeField]
			private List<UniqueItem> m_uniqueItemList;
			public List<UniqueItem> UniqueItemList => m_uniqueItemList;


			[SerializeField]
			private List<SearchTargetData> m_searchTargetList;
			public List<SearchTargetData> SearchTargetList => m_searchTargetList;

			#region カスタマイズ

			[System.Serializable]
			public class CustomizeBoardPartsData
			{
				public enum Rotate
				{
					Z0,
					Z90,
					Z180,
					Z270,
				}

				[SerializeField]
				private int m_uniqueId;
				public int UniqueId => m_uniqueId;

				[SerializeField]
				private Grid m_grid;
				public Grid Grid => m_grid;

				[SerializeField]
				private Rotate m_rot;
				public Rotate Rot => m_rot;

				public CustomizeBoardPartsData(
					int uniqueId,
					Grid grid,
					Rotate rot)
				{
					m_uniqueId = uniqueId;
					m_grid = grid;
					m_rot = rot;
				}

				public void UpdateGrid(Grid grid)
				{
					m_grid = grid;
				}

				public void UpdateRotate(Rotate rot)
				{
					m_rot = rot;
				}
			}

			[SerializeField]
			private List<CustomizeBoardPartsData> m_customizeBoardPartsDataList;
			public List<CustomizeBoardPartsData> CustomizeBoardPartsDataList => m_customizeBoardPartsDataList;

			public void AddCustomizeBoardPartsData(
				int uniqueId,
				Grid grid,
				CustomizeBoardPartsData.Rotate rot)
			{
				m_customizeBoardPartsDataList.Add(new CustomizeBoardPartsData(
					uniqueId,
					grid,
					rot));
			}

			#endregion

			/// <summary>
			/// BGMボリューム
			/// </summary>
			[SerializeField]
			private float m_bgmVolume = 0.5f;
			public float BgmVolume { get { return m_bgmVolume; } }

			public void SetBgmVolume(float value)
			{
				m_bgmVolume = value;
			}

			/// <summary>
			/// SEボリューム
			/// </summary>
			[SerializeField]
			private float m_seVolume = 0.5f;
			public float SEVolume { get { return m_seVolume; } }

			public void SetSEVolume(float value)
			{
				m_seVolume = value;
			}


			public LocalSave()
			{
			}
		}


		/// <summary>
		/// セーブデータ
		/// </summary>
		[SerializeField]
		private LocalSave m_localSaveData = null;
		public LocalSave LocalSaveData => m_localSaveData;

		/// <summary>
		/// セーブ
		/// </summary>
		/// <param name="callback"></param>
		/// <returns></returns>
		public IEnumerator Save(UnityAction callback)
		{
			//yield return GeneralRoot.Instance.SetSavingView();

			string json = JsonUtility.ToJson(m_localSaveData);
			yield return null;
			string str = Encrypt(json);
			yield return null;
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
			yield return null;
			string path = GetUserDataPath();
			GeneralRoot.Instance.WhiteFile(path, bytes);

			//yield return GeneralRoot.Instance.SetSavedView();

			if (callback != null)
			{
				callback();
			}
		}

		/// <summary>
		/// 読み込み
		/// </summary>
		/// <returns></returns>
		public IEnumerator Load()
		{
			string path = GetUserDataPath();
			if (GeneralRoot.Instance.IsExistFile(path) == true)
			{
				byte[] bytes = GeneralRoot.Instance.ReadFile(path);
				yield return null;
				string str = System.Text.Encoding.UTF8.GetString(bytes);
				yield return null;
				string json = Decrypt(str);
				yield return null;
				m_localSaveData = JsonUtility.FromJson<LocalSave>(json);
			}
		}

		/// <summary>
		/// セーブデータ削除
		/// </summary>
		public void SaveDataDelete()
		{
			string path = GetUserDataPath();
			GeneralRoot.Instance.DeleteCache(path);
			StartCoroutine(Load());
		}

		/// <summary>
		/// セーブデータのパス取得
		/// </summary>
		/// <returns></returns>
		private string GetUserDataPath()
		{
			return string.Format("{0}/{1}", Application.persistentDataPath, "userdata");
		}

		/// <summary>
		/// 対称鍵暗号を使って文字列を暗号化する
		/// </summary>
		/// <param name="str">暗号化する文字列</param>
		/// <returns>暗号化された文字列</returns>
		private string Encrypt(string str)
		{
			using (RijndaelManaged rijndael = new RijndaelManaged())
			{
				rijndael.BlockSize = 128;
				rijndael.KeySize = 128;
				rijndael.Mode = CipherMode.CBC;
				rijndael.Padding = PaddingMode.PKCS7;
				rijndael.IV = System.Text.Encoding.UTF8.GetBytes(kAesIV);
				rijndael.Key = System.Text.Encoding.UTF8.GetBytes(kAesKey);
				ICryptoTransform encryptor = rijndael.CreateEncryptor(rijndael.Key, rijndael.IV);

				byte[] encrypted;
				using (MemoryStream mStream = new MemoryStream())
				{
					using (CryptoStream ctStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter sw = new StreamWriter(ctStream))
						{
							sw.Write(str);
						}
						encrypted = mStream.ToArray();
					}
				}
				return (System.Convert.ToBase64String(encrypted));
			}
		}

		/// <summary>
		/// 対称鍵暗号を使って暗号文を復号する
		/// </summary>
		/// <param name="str">暗号化された文字列</param>
		/// <returns>復号された文字列</returns>
		private string Decrypt(string str)
		{
			using (RijndaelManaged rijndael = new RijndaelManaged())
			{
				rijndael.BlockSize = 128;
				rijndael.KeySize = 128;
				rijndael.Mode = CipherMode.CBC;
				rijndael.Padding = PaddingMode.PKCS7;
				rijndael.IV = System.Text.Encoding.UTF8.GetBytes(kAesIV);
				rijndael.Key = System.Text.Encoding.UTF8.GetBytes(kAesKey);

				ICryptoTransform decryptor = rijndael.CreateDecryptor(rijndael.Key, rijndael.IV);

				string plain = string.Empty;
				using (MemoryStream mStream = new MemoryStream(System.Convert.FromBase64String(str)))
				{
					using (CryptoStream ctStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Read))
					{
						using (StreamReader sr = new StreamReader(ctStream))
						{
							plain = sr.ReadLine();
						}
					}
				}
				return plain;
			}
		}
	}
}