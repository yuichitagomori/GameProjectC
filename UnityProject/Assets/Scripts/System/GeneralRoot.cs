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
	[System.Serializable]
	public class MouseCursor
	{
		public enum Type
		{
			Normal,
			Pinch,
		}

		[SerializeField]
		private Transform m_transform;

		[SerializeField]
		private GameObject[] m_iconObjects;


		private Type m_iconType = Type.Normal;

		public void Initialize()
		{
			UpdateIcon();
		}

		public void SetPoistion(Vector3 v)
		{
			m_transform.position = v;
		}

		public void SettingIcon(Type type)
		{
			if (m_iconType == type)
			{
				return;
			}

			m_iconType = type;
			UpdateIcon();
		}

		private void UpdateIcon()
		{
			for (int i = 0; i < m_iconObjects.Length; ++i)
			{
				m_iconObjects[i].SetActive(i == (int)m_iconType);
			}
		}
	}



	public static data.MasterData Master => Instance.m_masterData;

	public static data.UserData User => Instance.m_userData;

	public static data.ResourceData Resource => Instance.m_resourceData;



	public readonly static string DEBUG_ANDROID = "Debug_Android";
	public readonly static string RELEASE_ANDROID = "Release_Android";
	public readonly static string DEBUG_IOS = "Debug_IOS";
	public readonly static string RELEASE_IOS = "Release_IOS";
	public readonly static string DEBUG_STANDALONE = "Debug_Standalone";
	public readonly static string RELEASE_STANDALONE = "Release_Standalone";

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
	/// 入力管理
	/// </summary>
	[SerializeField]
	private system.InputSystem m_input;
	public system.InputSystem Input => m_input;

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
	private data.ResourceData m_resourceData;

	/// <summary>
	/// マスター情報
	/// </summary>
	[SerializeField]
	private data.MasterData m_masterData;

	/// <summary>
	/// ユーザー情報
	/// </summary>
	[SerializeField]
	private data.UserData m_userData;

	/// <summary>
	/// マウスカーソル
	/// </summary>
	[SerializeField]
	private MouseCursor m_mouseCursor;
	public MouseCursor mouseCursor => m_mouseCursor;

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
	/// デバッグメニュー
	/// </summary>
	[SerializeField]
	private scene.DebugMenu m_debugMenu;

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

		m_adsController.Initialize();
		m_sceneController.Initialize();
		m_input.Initialize();

		m_debugMenu.Show();

		StartCoroutine(UpdateMouseCursorCoroutine());
	}

	private IEnumerator UpdateMouseCursorCoroutine()
	{
		m_mouseCursor.Initialize();
		while (true)
		{
			var pos = Input.GetMousePosition();
			m_mouseCursor.SetPoistion(pos);
			bool isDown = Input.IsMouseDown();
			m_mouseCursor.SettingIcon(isDown ? MouseCursor.Type.Pinch: MouseCursor.Type.Normal);
			yield return null;
		}
	}

	/// <summary>
	/// 最前面レイキャスト表示切替
	/// </summary>
	/// <param name="active"></param>
	public void SetForeMostRayCast(bool active)
	{
		Input.SetRayCastActive(active);
		m_foremostRayCast.SetActive(active);
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
	/// <param name="path"></param>
	/// <param name="callback"></param>
	/// <returns></returns>
	public IEnumerator GetAssetBundle(string path, UnityAction<AssetBundle> callback)
	{
		var request = AssetBundle.LoadFromFileAsync(path);
		//yield return request;
		yield return new WaitWhile(() => request.isDone == false);
		var assetbundle = request.assetBundle;
		callback(assetbundle);
	}

	/// <summary>
	/// ファイルが存在しているかどうか
	/// </summary>
	/// <param name="path"></param>
	public bool IsExistFile(string path)
	{
		return File.Exists(path);
	}

	/// <summary>
	/// ファイル書き込み
	/// </summary>
	/// <param name="path"></param>
	/// <param name="bytes"></param>
	public void WhiteFile(string path, byte[] bytes)
	{
		File.WriteAllBytes(path, bytes);
	}

	/// <summary>
	/// ファイル読み込み
	/// </summary>
	/// <param name="path"></param>
	/// <returns></returns>
	public byte[] ReadFile(string path)
	{
		return File.ReadAllBytes(path);
	}

	/// <summary>
	/// キャッシュの削除
	/// </summary>
	/// <param name="path"></param>
	public void DeleteCache(string path)
	{
		File.Delete(path);
	}

	/// <summary>
	/// PCプラットフォームかどうか
	/// </summary>
	/// <returns></returns>
	public bool IsPCPlatform()
	{
#if UNITY_EDITOR
		return true;
#elif PLATFORM_STANDALONE
		return true;
#else
		return false;
#endif
	}
}
