using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Analytics;
//using GoogleMobileAds.Api;

namespace system
{
	/// <summary>
	/// GoogleAdsやUnityAdsのコントローラー
	/// </summary>
	public class AdsController : MonoBehaviour
	{
		///// <summary>
		///// バナー表示位置
		///// </summary>
		//[SerializeField]
		//private AdPosition m_adPosition;

		//private BannerView m_bannerView;



		public void Initialize()
		{
			// 広告システム初期化
			//#if UNITY_ANDROID || UNITY_IOS
			//			if (Advertisement.isSupported == true)
			//			{
			//				bool testMode = false;
			//#if UNITY_EDITOR
			//				testMode = true;
			//#endif
			//#if UNITY_ANDROID
			//				Advertisement.Initialize("2864847", testMode);
			//#elif UNITY_IOS
			//				Advertisement.Initialize("2864846", testMode);
			//#endif
			//			}
			//#endif

#if UNITY_EDITOR
			string adUnitId = "	ca-app-pub-3940256099942544/6300978111";
#elif UNITY_ANDROID
			string adUnitId = "	ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IOS
			string adUnitId = "	ca-app-pub-3940256099942544/6300978111";
#else
			string adUnitId = "	ca-app-pub-3940256099942544/6300978111";
#endif
			//MobileAds.Initialize(InitializeHandler);

			//AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

			//m_bannerView = new BannerView(adUnitId, adaptiveSize, m_adPosition);
			//m_bannerView.OnAdLoaded += this.HandleAdLoaded;
			//m_bannerView.OnAdFailedToLoad += this.HandleAdFailedToLoad;
			//m_bannerView.OnAdOpening += this.HandleAdOpened;
			//m_bannerView.OnAdClosed += this.HandleAdClosed;
			//m_bannerView.OnPaidEvent += this.HandlePaidEvent;

			//AdRequest adRequest = new AdRequest.Builder()
			//	.AddKeyword(AdRequest.TestDeviceSimulator)
			//	.AddKeyword("0123456789ABCDEF0123456789ABCDEF")
			//	.Build();

			//m_bannerView.LoadAd(adRequest);
		}

		/// <summary>
		/// UnityAnalyticsカスタムイベント送信
		/// </summary>
		/// <param name="_eventName"></param>
		/// <param name="_customData"></param>
		public void SendAnalyticsCustomEvent(string _eventName, Dictionary<string, object> _customData)
		{
			Analytics.CustomEvent(_eventName, _customData);
		}

		/// <summary>
		/// 広告表示
		/// </summary>
		/// <param name="_failed"></param>
		/// <param name="_skipped"></param>
		/// <param name="_finished"></param>
		public void ShowAdvertisement(
			UnityAction _failed,
			UnityAction _skipped,
			UnityAction _finished)
		{
#if UNITY_ANDROID || UNITY_IOS
			//if (Advertisement.IsReady() == false)
			//{
			//	if (_failed != null)
			//	{
			//		_failed();
			//	}
			//	return;
			//};

			//Advertisement.Show(new ShowOptions()
			//{
			//	resultCallback = result =>
			//	{
			//		switch (result)
			//		{
			//			case ShowResult.Failed:
			//				{
			//						// 失敗時
			//						if (_failed != null)
			//					{
			//						_failed();
			//					}
			//					break;
			//				}
			//			case ShowResult.Skipped:
			//				{
			//						// スキップ再生終了
			//						if (_skipped != null)
			//					{
			//						_skipped();
			//					}
			//					break;
			//				}
			//			case ShowResult.Finished:
			//				{
			//						// 正常再生終了
			//						if (_finished != null)
			//					{
			//						_finished();
			//					}
			//					break;
			//				}
			//		}
			//	}
			//});
#else
			if (_finished != null)
			{
				_finished();
			}
			return;
#endif
		}

		//private void InitializeHandler(InitializationStatus _status)
		//{
		//}

		//private void HandleAdLoaded(object sender, EventArgs args)
		//{
		//	Debug.Log("HandleAdLoaded event received");
		//	Debug.Log(String.Format("Ad Height: {0}, width: {1}",
		//		m_bannerView.GetHeightInPixels(),
		//		m_bannerView.GetWidthInPixels()));
		//}

		//private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
		//{
		//	Debug.Log("HandleFailedToReceiveAd event received");
		//}

		//private void HandleAdOpened(object sender, EventArgs args)
		//{
		//	Debug.Log("HandleAdOpened event received");
		//}

		//private void HandleAdClosed(object sender, EventArgs args)
		//{
		//	Debug.Log("HandleAdClosed event received");
		//}

		//private void HandlePaidEvent(object sender, AdValueEventArgs args)
		//{
		//	Debug.Log("HandlePaidEvent event received");
		//}

		
	}
}
