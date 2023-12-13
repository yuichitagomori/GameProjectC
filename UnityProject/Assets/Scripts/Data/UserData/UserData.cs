using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Linq;

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
			/// <summary>
			/// チェックシートID
			/// </summary>
			[SerializeField]
			private int m_checkSheetId;
			public int CheckSheetId => m_checkSheetId;

			/// <summary>
			/// 精度レベル
			/// </summary>
			[SerializeField]
			private int m_accuracyLevel;
			public int AccuracyLevel => m_accuracyLevel;

			/// <summary>
			/// 発生バグIDリスト
			/// </summary>
			[SerializeField]
			private int[] m_occurredBugIds;
			public int[] OccurredBugIds => m_occurredBugIds;

			/// <summary>
			/// BGMボリューム
			/// </summary>
			[SerializeField]
			private float m_bgmVolume = 0.5f;
			public float BgmVolume { get { return m_bgmVolume; } }

			public void UpdateBgmVolume(float value)
			{
				m_bgmVolume = value;
			}

			/// <summary>
			/// SEボリューム
			/// </summary>
			[SerializeField]
			private float m_seVolume = 0.5f;
			public float SEVolume { get { return m_seVolume; } }

			public void UpdateSEVolume(float value)
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