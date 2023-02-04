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
		public class LocalSaveData
		{
			//public enum AppMode
			//{
			//	None = 0,
			//	CameraWide = 1,
			//	CameraWide2 = 1 << 1,
			//	CameraRotate = 1 << 2,
			//	CircleHit = 1 << 3,
			//	CircleMark = 1 << 4,
			//}

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

			[SerializeField]
			private int m_stageId = -1;
			public int StageID => m_stageId;

			//[SerializeField]
			//private AppMode m_appModeList = AppMode.None;
			//public AppMode AppModeList => m_appModeList;

			[SerializeField]
			private List<SearchTargetData> m_searchTargetList = null;
			public List<SearchTargetData> SearchTargetList => m_searchTargetList;

			//public void SetAppModeList(AppMode mode)
			//{
			//	m_appModeList = mode;
			//}

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


			public LocalSaveData()
			{
			}
		}


		/// <summary>
		/// セーブデータ
		/// </summary>
		[SerializeField]
		private LocalSaveData m_data = null;
		public LocalSaveData Data => m_data;

		/// <summary>
		/// セーブ
		/// </summary>
		/// <param name="_callback"></param>
		/// <returns></returns>
		public IEnumerator Save(UnityAction _callback)
		{
			//yield return GeneralRoot.Instance.SetSavingView();

			string json = JsonUtility.ToJson(m_data);
			yield return null;
			string str = Encrypt(json);
			yield return null;
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
			yield return null;
			string path = GetUserDataPath();
			GeneralRoot.Instance.WhiteFile(path, bytes);

			//yield return GeneralRoot.Instance.SetSavedView();

			if (_callback != null)
			{
				_callback();
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
				m_data = JsonUtility.FromJson<LocalSaveData>(json);
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
		/// <param name="_str">暗号化する文字列</param>
		/// <returns>暗号化された文字列</returns>
		private string Encrypt(string _str)
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
							sw.Write(_str);
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
		/// <param name="_str">暗号化された文字列</param>
		/// <returns>復号された文字列</returns>
		private string Decrypt(string _str)
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
				using (MemoryStream mStream = new MemoryStream(System.Convert.FromBase64String(_str)))
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