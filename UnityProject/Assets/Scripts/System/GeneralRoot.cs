using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Advertisements;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 一般的な機能取得先
/// </summary>
public class GeneralRoot : SingletonMonoBehaviour<GeneralRoot>
{
	//public readonly static string DEBUG_ANDROID = "Debug_Android";
	//public readonly static string RELEASE_ANDROID = "Release_Android";
	//public readonly static string DEBUG_IOS = "Debug_IOS";
	//public readonly static string RELEASE_IOS = "Release_IOS";
	//public readonly static string DEBUG_STANDALONE = "Debug_Standalone";
	//public readonly static string RELEASE_STANDALONE = "Release_Standalone";

	//public readonly static string kVersionDataName = "versiondata";
	//public readonly static string kBundleDataName = "bundledata";
	//public readonly static string kMasterDataName = "masterdata";
	//public readonly static string kSSDataName = "ssdata";
	//public readonly static string kSoundDataName = "sounddata";


	/// <summary>
	/// シーン操作
	/// </summary>
	[SerializeField]
	private scene.SceneController m_sceneController;

	/// <summary>
	/// Ads操作
	/// </summary>
	[SerializeField]
	private system.AdsController m_adsController;
	public system.AdsController AdsController { get { return m_adsController; } }

	///// <summary>
	///// 通信コネクター
	///// </summary>
	//[SerializeField]
	//private AWSConnect m_connecter;
	//public AWSConnect Connecter { get { return m_connecter; } }

	/// <summary>
	/// アセット情報
	/// </summary>
	[SerializeField]
	private data.AssetData m_assetData;
	public data.AssetData AssetData { get { return m_assetData; } }

	/// <summary>
	/// SpriteStudio情報
	/// </summary>
	[SerializeField]
	private data.SpriteStudioData m_ssData;
	public data.SpriteStudioData SSData { get { return m_ssData; } }

	/// <summary>
	/// マスター情報
	/// </summary>
	[SerializeField]
	private data.MasterData m_masterData;
	public data.MasterData MasterData { get { return m_masterData; } }

	/// <summary>
	/// ユーザー情報
	/// </summary>
	[SerializeField]
	private data.UserData m_userData;
	public data.UserData UserData { get { return m_userData; } }

	///// <summary>
	///// サウンドシステム
	///// </summary>
	//[SerializeField]
	//private SoundSystem m_soundSystem;
	//public SoundSystem SoundSystem { get { return m_soundSystem; } }

	///// <summary>
	///// ストアシステム
	///// </summary>
	//[SerializeField]
	//private StoreSystem m_storeSystem;
	//public StoreSystem StoreSystem { get { return m_storeSystem; } }



	/// <summary>
	/// 最前面レイキャスト
	/// </summary>
	[SerializeField]
	private GameObject m_foremostRayCast;

	/// <summary>
	/// カメラ
	/// </summary>
	[SerializeField]
	private Camera m_ingameCamera;
	public Camera GetIngameCamera() => m_ingameCamera;

	/// <summary>
	/// カメラ
	/// </summary>
	[SerializeField]
	private Camera m_ingame2Camera;
	public Camera GetIngame2Camera() => m_ingame2Camera;

	/// <summary>
	/// カメラ
	/// </summary>
	[SerializeField]
	private Camera m_outgameCamera;
	public Camera GetOutgameCamera() => m_outgameCamera;

	/// <summary>
	/// デバッグメニュー
	/// </summary>
	[SerializeField]
	private scene.DebugMenu m_debugMenu;
	public scene.DebugMenu DebugMenu => m_debugMenu;

	/// <summary>
	/// ポスプロ
	/// </summary>
	[SerializeField]
	private Volume m_volume;
	public Vignette GetVignette()
	{
		Vignette vignette = null;
		if (m_volume.profile.TryGet<Vignette>(out vignette))
		{
			return vignette;
		}
		return null;
	}
	public Bloom GetBloom()
	{
		Bloom bloom = null;
		if (m_volume.profile.TryGet<Bloom>(out bloom))
		{
			return bloom;
		}
		return null;
	}



	private void Start()
    {
        DontDestroyOnLoad(this);

		// フレームレート初期化
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;

		// マルチタッチ無効
		Input.multiTouchEnabled = false;

		m_adsController.Initialize();
		m_sceneController.Initialize();

		m_debugMenu.Show();
	}

	/// <summary>
	/// 最前面レイキャスト表示切替
	/// </summary>
	/// <param name="_active"></param>
	public void SetForeMostRayCast(bool _active)
	{
		m_foremostRayCast.SetActive(_active);
	}

	///// <summary>
	///// セーブ中表示
	///// </summary>
	///// <returns></returns>
	//public IEnumerator SetSavingView()
	//{
	//	m_saveViewObject.SetActive(true);
	//	m_saveViewText.SetText(3);

	//	bool isDone = false;
	//	m_saveViewAnimator.Play("In", () => { isDone = true; });
	//	while (!isDone) { yield return null; }

	//	m_saveViewAnimator.PlayLoop("Loop");
	//}

	///// <summary>
	///// セーブ完了表示
	///// </summary>
	///// <returns></returns>
	//public IEnumerator SetSavedView()
	//{
	//	yield return new WaitForSeconds(1.0f);

	//	m_saveViewText.SetText(4);

	//	bool isDone = false;
	//	m_saveViewAnimator.Play("Out", () => { isDone = true; });
	//	while (!isDone) { yield return null; }

	//	m_saveViewObject.SetActive(false);
	//}





	//	/// <summary>
	//	/// アセットバンドルのロード
	//	/// </summary>
	//	/// <param name="_fileName"></param>
	//	/// <param name="_successed"></param>
	//	/// <param name="_failed"></param>
	//	/// <param name="_progress"></param>
	//	/// <returns></returns>
	//	public IEnumerator DownloadAssetBundle(
	//		string _fileName,
	//		UnityAction<byte[]> _successed,
	//		UnityAction _failed,
	//		UnityAction<float> _progress = null)
	//	{
	//		string downloadPath = "";

	//#if UNITY_ANDROID
	//#if RELEASE
	//		downloadPath = string.Format("{0}/{1}", RELEASE_ANDROID, _fileName);
	//#else
	//		downloadPath = string.Format("{0}/{1}", DEBUG_ANDROID, _fileName);
	//#endif

	//#elif UNITY_IOS
	//#if RELEASE
	//		downloadPath = string.Format("{0}/{1}", RELEASE_IOS, _fileName);
	//#else
	//		downloadPath = string.Format("{0}/{1}", DEBUG_IOS, _fileName);
	//#endif

	//#else
	//#if RELEASE
	//		downloadPath = string.Format("{0}/{1}", RELEASE_STANDALONE, _fileName);
	//#else

	//		downloadPath = string.Format("{0}/{1}", DEBUG_STANDALONE, _fileName);
	//#endif
	//#endif

	//		bool isSuccess = false;
	//		var bytes = new byte[] { };
	//		yield return GeneralRoot.Instance.Connecter.GetBytesS3Coroutine(
	//			downloadPath,
	//			false,
	//			(result) =>
	//			{
	//				isSuccess = true;
	//				bytes = result;
	//			},
	//			() =>
	//			{
	//				isSuccess = false;
	//			},
	//			_progress);

	//		if (isSuccess == false)
	//		{
	//			// 取得失敗
	//			if (_failed != null)
	//			{
	//				_failed();
	//			}
	//			yield break;
	//		}

	//		if (_successed != null)
	//		{
	//			_successed(bytes);
	//		}
	//	}

	/// <summary>
	/// アセットバンドル取得
	/// </summary>
	/// <param name="_path"></param>
	/// <param name="_callback"></param>
	/// <returns></returns>
	public IEnumerator GetAssetBundle(string _path, UnityAction<AssetBundle> _callback)
	{
		var request = AssetBundle.LoadFromFileAsync(_path);
		//yield return request;
		yield return new WaitWhile(() => request.isDone == false);
		var assetbundle = request.assetBundle;
		_callback(assetbundle);
	}

	/// <summary>
	/// ファイルが存在しているかどうか
	/// </summary>
	/// <param name="_path"></param>
	public bool IsExistFile(string _path)
	{
		return File.Exists(_path);
	}

	/// <summary>
	/// ファイル書き込み
	/// </summary>
	/// <param name="_path"></param>
	/// <param name="_bytes"></param>
	public void WhiteFile(string _path, byte[] _bytes)
	{
		File.WriteAllBytes(_path, _bytes);
	}

	/// <summary>
	/// ファイル読み込み
	/// </summary>
	/// <param name="_path"></param>
	/// <returns></returns>
	public byte[] ReadFile(string _path)
	{
		return File.ReadAllBytes(_path);
	}

	/// <summary>
	/// キャッシュの削除
	/// </summary>
	/// <param name="_path"></param>
	public void DeleteCache(string _path)
	{
		File.Delete(_path);
	}
}
