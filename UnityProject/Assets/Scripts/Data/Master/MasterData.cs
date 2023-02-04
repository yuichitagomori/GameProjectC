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
		///// <summary>
		///// マップデータ
		///// </summary>
		//[SerializeField]
		//private MapData m_mapData;
		//public MapData MapData { get { return m_mapData; } }

		/// <summary>
		/// マップ歩行データ名
		/// </summary>
		[SerializeField]
		private FileNameListData m_mapWalkNameList;
		public FileNameListData MapWalkNameList { get { return m_mapWalkNameList; } }

		/// <summary>
		/// マップ歩行データリスト
		/// </summary>
		[SerializeField]
		private List<MapWalkData> m_mapWalkDataList;
		public List<MapWalkData> MapWalkDataList { get { return m_mapWalkDataList; } }

		///// <summary>
		///// マップ歩行データリスト
		///// </summary>
		//[SerializeField]
		//private Dictionary<string, MapOptionData> m_mapOptionDatas;
		//public Dictionary<string, MapOptionData> MapOptionDatas { get { return m_mapOptionDatas; } }

		///// <summary>
		///// マップランダムIDデータ
		///// </summary>
		//[SerializeField]
		//private MapRandomData m_mapRandomData;
		//public MapRandomData MapRandomData { get { return m_mapRandomData; } }

		///// <summary>
		///// 行動効果データ
		///// </summary>
		//[SerializeField]
		//private ActionEffectData m_actionEffectData;
		//public ActionEffectData ActionEffectData { get { return m_actionEffectData; } }

		///// <summary>
		///// 敵データ
		///// </summary>
		//[SerializeField]
		//private EnemyData m_enemyData;
		//public EnemyData EnemyData { get { return m_enemyData; } }

		///// <summary>
		///// 敵ドロップデータ
		///// </summary>
		//[SerializeField]
		//private EnemyDropData m_enemyDropData;
		//public EnemyDropData EnemyDropData { get { return m_enemyDropData; } }

		///// <summary>
		///// ギミックデータ
		///// </summary>
		//[SerializeField]
		//private GimmickData m_gimmickData;
		//public GimmickData GimmickData { get { return m_gimmickData; } }

		///// <summary>
		///// ギミックドロップデータ
		///// </summary>
		//[SerializeField]
		//private GimmickDropData m_gimmickDropData;
		//public GimmickDropData GimmickDropData { get { return m_gimmickDropData; } }

		///// <summary>
		///// アイテムデータ
		///// </summary>
		//[SerializeField]
		//private ItemData m_itemData;
		//public ItemData ItemData { get { return m_itemData; } }

		///// <summary>
		///// アイテムドロップデータ
		///// </summary>
		//[SerializeField]
		//private ItemDropData m_itemDropData;
		//public ItemDropData ItemDropData { get { return m_itemDropData; } }

		///// <summary>
		///// アイテムレベルデータ
		///// </summary>
		//[SerializeField]
		//private ItemLevelData m_itemLevelData;
		//public ItemLevelData ItemLevelData { get { return m_itemLevelData; } }

		///// <summary>
		///// プレイヤーデータ
		///// </summary>
		//[SerializeField]
		//private PlayerData m_playerData;
		//public PlayerData PlayerData { get { return m_playerData; } }

		///// <summary>
		///// ステータスデータ
		///// </summary>
		//[SerializeField]
		//private StatusData m_statusData;
		//public StatusData StatusData { get { return m_statusData; } }

		///// <summary>
		///// マイナーデータ
		///// </summary>
		//[SerializeField]
		//private MinorData m_minorData;
		//public MinorData MinorData { get { return m_minorData; } }

		///// <summary>
		///// シナリオデータリスト
		///// </summary>
		//[SerializeField]
		//private Dictionary<string, ScenarioData> m_scenarioDatas;
		//public Dictionary<string, ScenarioData> ScenarioDatas { get { return m_scenarioDatas; } }

		///// <summary>
		///// シナリオ詳細データ
		///// </summary>
		//[SerializeField]
		//private ScenarioInfoData m_scenarioInfoData;
		//public ScenarioInfoData ScenarioInfoData { get { return m_scenarioInfoData; } }

		///// <summary>
		///// 商品データ
		///// </summary>
		//[SerializeField]
		//private ShopData m_shopData;
		//public ShopData ShopData { get { return m_shopData; } }

		///// <summary>
		///// 商品アイテムデータ
		///// </summary>
		//[SerializeField]
		//private ShopItemData m_shopItemData;
		//public ShopItemData ShopItemData { get { return m_shopItemData; } }

		/// <summary>
		/// ローカライズデータ
		/// </summary>
		[SerializeField]
		private LocalizeData m_localizeData;
		public LocalizeData LocalizeData { get { return m_localizeData; } }

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