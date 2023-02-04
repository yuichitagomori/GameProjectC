using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace data
{
	/// <summary>
	/// SSデータ
	/// </summary>
	public class SpriteStudioData : MonoBehaviour
	{
		/// <summary>
		/// SpriteStudioPrefabデータ
		/// </summary>
		[SerializeField]
		private List<GameObject> m_prefabs;
		public List<GameObject> Prefabs { get { return m_prefabs; } }



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

			m_assetBundle = _assetBundle;
			
			foreach (string filePath in m_assetBundle.GetAllAssetNames())
			{
				string[] split = filePath.Split('/');
				if (split[split.Length - 2] == "prefabanimation")
				{
					string fileName = split[split.Length - 1].Split('.')[0];
					AssetBundleRequest request = null;
					try
					{
						request = m_assetBundle.LoadAssetAsync(fileName, typeof(GameObject));
					}
					catch (System.Exception e)
					{
						Debug.LogError(e);
						if (_failed != null) { _failed(); }
						yield break;
					}
					yield return new WaitWhile(() => request.isDone == false);
					m_prefabs.Add((GameObject)request.asset);
				}
			}

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