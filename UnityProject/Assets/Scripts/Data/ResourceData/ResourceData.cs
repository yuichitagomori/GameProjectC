using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace data
{
	/// <summary>
	/// アセットデータ
	/// </summary>
	public class ResourceData : MonoBehaviour
	{
		/// <summary>
		/// サブキャラ色データ
		/// </summary>
		[SerializeField]
		private resource.EnemyColorResource m_enemyColorResource;
		public resource.EnemyColorResource EnemyColorResource => m_enemyColorResource;

		/// <summary>
		/// ピース
		/// </summary>
		[SerializeField]
		private resource.TextureResource m_customizePartsResource;
		public resource.TextureResource CustomizePartsResource => m_customizePartsResource;



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
			//{
			//	string fileName = "EnemyIconData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(IconData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_enemyIcon = request.asset as IconData;
			//}

			//{
			//	string fileName = "ItemIconData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(IconData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_itemIcon = request.asset as IconData;
			//}

			//{
			//	string fileName = "StatusIconData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(IconData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_statusIcon = request.asset as IconData;
			//}

			//{
			//	string fileName = "TrophyIconData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(IconData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_trophyIcon = request.asset as IconData;
			//}

			//{
			//	string fileName = "ShopIconData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(IconData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_shopIcon = request.asset as IconData;
			//}

			//{
			//	string fileName = "MapBgData";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(MapBgData));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_mapBg = request.asset as MapBgData;
			//}

			//{
			//	string fileName = "MapTileSprite";
			//	try
			//	{
			//		request = m_assetBundle.LoadAssetAsync(fileName, typeof(MapTileSprite));
			//	}
			//	catch (System.Exception e)
			//	{
			//		Debug.LogError(e);
			//		if (_failed != null) { _failed(); }
			//		yield break;
			//	}
			//	yield return new WaitWhile(() => request.isDone == false);
			//	m_mapTile = request.asset as MapTileSprite;
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