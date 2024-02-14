using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace system
{
	public class SoundSystem : MonoBehaviour
	{
		public enum SEType
		{
			OPEN_SCENE = 1,
			CLOSE_SCENE = 2,
			WINDOW_OPEN = 3,
			WINDOW_CLOSE = 4,
			BUTTON_PRESS = 5,
			BUTTON_LONG_PRESS = 6,

			TITLE_TAP = 20,
			PLAYER_WALK = 22,
			PLAYER_DAD_FANFARE = 24,
			ITEM_DROP = 25,
			ITEM_EAT = 26,
			//GOOD_FANFARE = 27, // シナリオ側で取り扱っている
			ITEM_RANKUP = 28,
			GOOD_STATUS = 29,
			BAD_STATUS = 30,
			GACHA = 31,
		}

		/// <summary>
		/// サウンド再生コンポーネント
		/// </summary>
		[SerializeField]
		private CriAtomSource m_criSourceBGM;

		/// <summary>
		/// サウンド再生コンポーネント
		/// </summary>
		[SerializeField]
		private CriAtomSource m_criSourceSE;



		/// <summary>
		/// アセットバンドル
		/// </summary>
		private AssetBundle m_assetBundle = null;

		/// <summary>
		/// サウンドシステムプレハブ
		/// </summary>
		private GameObject m_soundPrefab = null;

		/// <summary>
		/// BGM再生状態
		/// </summary>
		private CriAtomExPlayback m_playingBgm;

		/// <summary>
		/// 再生中CueId
		/// </summary>
		private int m_bgmCueId = -1;




		/// <summary>
		/// 読み込み
		/// </summary>
		/// <param name="assetBundle"></param>
		/// <param name="success"></param>
		/// <param name="failed"></param>
		/// <returns></returns>
		public IEnumerator Load(AssetBundle assetBundle, UnityAction success, UnityAction failed)
		{
			m_assetBundle = assetBundle;

			{
				string fileName = "CRIProject1.acf";
				AssetBundleRequest request = null;
				try
				{
					request = m_assetBundle.LoadAssetAsync(fileName, typeof(TextAsset));
				}
				catch (System.Exception e)
				{
					Debug.LogError(e);
					if (failed != null) { failed(); }
					yield break;
				}
				yield return new WaitWhile(() => request.isDone == false);
				var textAsset = request.asset as TextAsset;
				string path = string.Format("{0}/{1}", CriWare.streamingAssetsPath, fileName);
				try
				{
					if (GeneralRoot.Instance.IsExistFile(path) == true)
					{
						GeneralRoot.Instance.DeleteCache(path);
					}
					GeneralRoot.Instance.WhiteFile(path, textAsset.bytes);
				}
				catch (System.Exception e)
				{
					Debug.LogError(e);
					if (failed != null) { failed(); }
					yield break;
				}
			}

			{
				string fileName = "BGM.acb";
				AssetBundleRequest request = null;
				try
				{
					request = m_assetBundle.LoadAssetAsync(fileName, typeof(TextAsset));
				}
				catch (System.Exception e)
				{
					Debug.LogError(e);
					if (failed != null) { failed(); }
					yield break;
				}
				yield return new WaitWhile(() => request.isDone == false);
				var textAsset = request.asset as TextAsset;
				string path = string.Format("{0}/{1}", CriWare.streamingAssetsPath, fileName);
				try
				{
					if (GeneralRoot.Instance.IsExistFile(path) == true)
					{
						GeneralRoot.Instance.DeleteCache(path);
					}
					GeneralRoot.Instance.WhiteFile(path, textAsset.bytes);
				}
				catch (System.Exception e)
				{
					Debug.LogError(e);
					if (failed != null) { failed(); }
					yield break;
				}
			}

			{
				string fileName = "SE.acb";
				AssetBundleRequest request = null;
				try
				{
					request = m_assetBundle.LoadAssetAsync(fileName, typeof(TextAsset));
				}
				catch (System.Exception e)
				{
					Debug.LogError(e);
					if (failed != null) { failed(); }
					yield break;
				}
				yield return new WaitWhile(() => request.isDone == false);
				var textAsset = request.asset as TextAsset;
				string path = string.Format("{0}/{1}", CriWare.streamingAssetsPath, fileName);
				try
				{
					if (GeneralRoot.Instance.IsExistFile(path) == true)
					{
						GeneralRoot.Instance.DeleteCache(path);
					}
					GeneralRoot.Instance.WhiteFile(path, textAsset.bytes);
				}
				catch (System.Exception e)
				{
					Debug.LogError(e);
					if (failed != null) { failed(); }
					yield break;
				}
			}

			{
				string fileName = "SoundPrefab";
				AssetBundleRequest request = null;
				try
				{
					request = m_assetBundle.LoadAssetAsync(fileName, typeof(GameObject));
				}
				catch (System.Exception e)
				{
					Debug.LogError(e);
					if (failed != null) { failed(); }
					yield break;
				}
				yield return new WaitWhile(() => request.isDone == false);

				if (m_soundPrefab != null)
				{
					GameObject.Destroy(m_soundPrefab);
					yield return null;
				}
				m_soundPrefab = GameObject.Instantiate(request.asset as GameObject);
				m_soundPrefab.transform.parent = this.transform;

				var BGMObj = m_soundPrefab.transform.Find("BGM");
				m_criSourceBGM = BGMObj.GetComponent<CriAtomSource>();
				m_criSourceBGM.player.SetEnvelopeAttackTime(2000.0f);
				m_criSourceBGM.player.SetEnvelopeReleaseTime(2000.0f);

				var SEObj = m_soundPrefab.transform.Find("SE");
				m_criSourceSE = SEObj.GetComponent<CriAtomSource>();
			}

			if (success != null) { success(); }
		}

		/// <summary>
		/// BGM再生
		/// </summary>
		/// <param name="cueID"></param>
		public void PlayBGM(int cueID)
		{
			if (m_criSourceBGM == null) return;

			if (m_bgmCueId == cueID) return;

			m_bgmCueId = cueID;
			var status = m_playingBgm.GetStatus();
			if (status == CriAtomExPlayback.Status.Playing)
			{
				// すでに別のBGMが再生されているので、クロスフェード
				m_playingBgm.Stop();
				m_playingBgm = m_criSourceBGM.Play(m_bgmCueId);
			}
			else
			{
				// まだ再生されていない、または停止していたので、通常再生
				m_playingBgm = m_criSourceBGM.Play(m_bgmCueId);
			}
		}

		/// <summary>
		/// BGM停止
		/// </summary>
		public void StopBGM()
		{
			if (m_criSourceBGM == null) return;

			var status = m_playingBgm.GetStatus();
			if (status == CriAtomExPlayback.Status.Playing)
			{
				// BGM停止
				m_bgmCueId = -1;
				m_playingBgm.Stop();
			}
		}

		/// <summary>
		/// SE再生
		/// </summary>
		/// <param name="cueID"></param>
		public void PlaySE(int cueID)
		{
			if (m_criSourceSE == null) return;

			m_criSourceSE.Play(cueID);
		}

		/// <summary>
		/// 解放
		/// </summary>
		public void Unload()
		{
			if (m_assetBundle != null)
			{
				m_assetBundle.Unload(true);
			}
		}

		/// <summary>
		/// BGM音量
		/// </summary>
		/// <param name="volume"></param>
		public void SetBGMVolume(float volume)
		{
			if (m_criSourceBGM == null) return;

			m_criSourceBGM.volume = volume;
		}

		/// <summary>
		/// SE音量
		/// </summary>
		/// <param name="volume"></param>
		public void SetSEVolume(float volume)
		{
			if (m_criSourceSE == null) return;

			m_criSourceSE.volume = volume;
		}
	}
}