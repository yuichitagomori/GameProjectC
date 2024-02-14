using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace data
{
	/// <summary>
	/// マスターデータ
	/// </summary>
	public class MasterData : MonoBehaviour
	{
		/// <summary>
		/// ゲーム内容データ
		/// </summary>
		[SerializeField]
		private master.GameGunreData m_gameGunreData;
		public master.GameGunreData GameGunreData => m_gameGunreData;

		/// <summary>
		/// ムービー内容データ
		/// </summary>
		[SerializeField]
		private master.MovieListData m_movieListData;
		public master.MovieListData MovieListData => m_movieListData;

		/// <summary>
		/// キャラクターウィンドウ用ムービー内容データ
		/// </summary>
		[SerializeField]
		private master.CharaWindowMovieData m_charaWindowMovieData;
		public master.CharaWindowMovieData CharaWindowMovieData => m_charaWindowMovieData;

		/// <summary>
		/// チェックシートバグ内容データ
		/// </summary>
		[SerializeField]
		private master.CheckSheetBugData m_checkSheetBugData;
		public master.CheckSheetBugData CheckSheetBugData => m_checkSheetBugData;

		/// <summary>
		/// 報酬データ
		/// </summary>
		[SerializeField]
		private master.RewardData m_rewardData;
		public master.RewardData RewardData => m_rewardData;

		/// <summary>
		/// ローカライズデータ
		/// </summary>
		[SerializeField]
		private master.LocalizeData m_localizeData;
		public master.LocalizeData LocalizeData => m_localizeData;

		///// <summary>
		///// 汎用情報データ
		///// </summary>
		//[SerializeField]
		//private CommonSettingData m_commonSettingData;
		//public CommonSettingData CommonSettingData { get { return m_commonSettingData; } }



		/// <summary>
		/// アセットバンドル
		/// </summary>
		private AssetBundle m_assetBundle = null;

		/// <summary>
		/// 読み込み
		/// </summary>
		/// <param name="_assetBundle"></param>
		/// <param name="_success"></param>
		/// <param name="_failed"></param>
		/// <returns></returns>
		public IEnumerator Load(AssetBundle _assetBundle, UnityAction _success, UnityAction _failed)
		{
			yield return null;

			//m_assetBundle = _assetBundle;
			//AssetBundleRequest request = null;

			//{
			//	string fileName = "MapData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(MapData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_mapData = request.asset as MapData;
			//}

			//{
			//	string fileName = "MapWalkDataNames";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(FileNameListData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	FileNameListData fileNameList = request.asset as FileNameListData;

			//	m_mapWalkDatas = new Dictionary<string, MapWalkData>();
			//	foreach (string fileName2 in fileNameList.FileNames)
			//	{
			//		try
			//		{
			//			request = m_assetBundle.LoadAssetAsync(fileName2, typeof(MapWalkData));
			//		}
			//		catch (System.Exception e)
			//		{
			//			Debug.LogError(e);
			//			if (_failed != null) { _failed(); }
			//			yield break;
			//		}
			//		yield return new WaitWhile(() => request.isDone == false);
			//		m_mapWalkDatas.Add(fileName2, request.asset as MapWalkData);
			//	}
			//}

			//{
			//	string fileName = "MapOptionDataNames";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(FileNameListData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	FileNameListData fileNameList = request.asset as FileNameListData;

			//	m_mapOptionDatas = new Dictionary<string, MapOptionData>();
			//	foreach (string fileName2 in fileNameList.FileNames)
			//	{
			//		try
			//		{
			//			request = m_assetBundle.LoadAssetAsync(fileName2, typeof(MapOptionData));
			//		}
			//		catch (System.Exception e)
			//		{
			//			Debug.LogError(e);
			//			if (_failed != null) { _failed(); }
			//			yield break;
			//		}
			//		yield return new WaitWhile(() => request.isDone == false);
			//		m_mapOptionDatas.Add(fileName2, request.asset as MapOptionData);
			//	}
			//}

			//{
			//	string fileName = "MapRandomData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(MapRandomData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_mapRandomData = request.asset as MapRandomData;
			//}

			//{
			//	string fileName = "ActionEffectData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(ActionEffectData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_actionEffectData = request.asset as ActionEffectData;
			//}

			//{
			//	string fileName = "EnemyData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(EnemyData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_enemyData = request.asset as EnemyData;
			//}

			//{
			//	string fileName = "EnemyDropData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(EnemyDropData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_enemyDropData = request.asset as EnemyDropData;
			//}

			//{
			//	string fileName = "GimmickData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(GimmickData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_gimmickData = request.asset as GimmickData;
			//}

			//{
			//	string fileName = "GimmickDropData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(GimmickDropData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_gimmickDropData = request.asset as GimmickDropData;
			//}

			//{
			//	string fileName = "ItemData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(ItemData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_itemData = request.asset as ItemData;
			//}

			//{
			//	string fileName = "ItemDropData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(ItemDropData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_itemDropData = request.asset as ItemDropData;
			//}

			//{
			//	string fileName = "ItemLevelData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(ItemLevelData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_itemLevelData = request.asset as ItemLevelData;
			//}

			//{
			//	string fileName = "PlayerData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(PlayerData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_playerData = request.asset as PlayerData;
			//}

			//{
			//	string fileName = "StatusData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(StatusData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_statusData = request.asset as StatusData;
			//}

			//{
			//	string fileName = "MinorData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(MinorData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_minorData = request.asset as MinorData;
			//}

			//{
			//	string fileName = "ScenarioDataNames";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(FileNameListData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	FileNameListData fileNameList = request.asset as FileNameListData;

			//	m_scenarioDatas = new Dictionary<string, ScenarioData>();
			//	foreach (string fileName2 in fileNameList.FileNames)
			//	{
			//		try
			//		{
			//			request = m_assetBundle.LoadAssetAsync(fileName2, typeof(ScenarioData));
			//		}
			//		catch (System.Exception e)
			//		{
			//			Debug.LogError(e);
			//			if (_failed != null) { _failed(); }
			//			yield break;
			//		}
			//		yield return new WaitWhile(() => request.isDone == false);
			//		m_scenarioDatas.Add(fileName2, request.asset as ScenarioData);
			//	}
			//}

			//{
			//	string fileName = "ScenarioInfoData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(ScenarioInfoData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_scenarioInfoData = request.asset as ScenarioInfoData;
			//}

			//{
			//	string fileName = "ShopData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(ShopData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_shopData = request.asset as ShopData;
			//}

			//{
			//	string fileName = "ShopItemData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(ShopItemData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_shopItemData = request.asset as ShopItemData;
			//}

			//{
			//	string fileName = "LocalizeData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(LocalizeData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_localizeData = request.asset as LocalizeData;
			//}

			//{
			//	string fileName = "CommonSettingData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(CommonSettingData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_commonSettingData = request.asset as CommonSettingData;
			//}

			if (_success != null) { _success(); }
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
	}
}