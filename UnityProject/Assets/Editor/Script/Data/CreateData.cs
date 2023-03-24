using UnityEngine;

using UnityEditor;
using System.IO;

namespace data
{
	/// <summary>
	/// エディタスクリプト
	/// 各ScriptableObject作成
	/// </summary>
	public class CreateData : MonoBehaviour
	{
		[MenuItem("Assets/Create/Build AssetBundles")]
		public static void BuildAllAssetBundles()
		{
			string path = "";
#if UNITY_ANDROID
#if RELEASE
		path = string.Format("{0}/{1}", Application.streamingAssetsPath, GeneralRoot.RELEASE_ANDROID);
#else
		path = string.Format("{0}/{1}", Application.streamingAssetsPath, GeneralRoot.DEBUG_ANDROID);
#endif
		BuildTarget target = BuildTarget.Android;

#elif UNITY_IOS
#if RELEASE
		path = string.Format("{0}/{1}", Application.streamingAssetsPath, GeneralRoot.RELEASE_IOS);
#else
		path = string.Format("{0}/{1}", Application.streamingAssetsPath, GeneralRoot.DEBUG_IOS);
#endif
		BuildTarget target = BuildTarget.iOS;

#else
#if RELEASE
		path = string.Format("{0}/{1}", Application.streamingAssetsPath, GeneralRoot.RELEASE_STANDALONE);
#else
			path = string.Format("{0}/{1}", Application.streamingAssetsPath, GeneralRoot.DEBUG_STANDALONE);
#endif
			BuildTarget target = BuildTarget.StandaloneWindows64;

#endif
			if (Directory.Exists(path) == false)
			{
				Directory.CreateDirectory(path);
			}
			BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, target);
		}



		#region Master
		[MenuItem("Assets/Create/ScriptableObject/MasterData/CustomizeParts/CustomizePartsData")]
		public static void CreateCustomizePartsData()
		{
			CreateMasterAsset<master.CustomizeParts, master.CustomizeParts.Data>
				("CustomizeParts/CustomizeParts");
		}

		[MenuItem("Assets/Create/ScriptableObject/MasterData/CustomizeParts/CustomizePartsEffectData")]
		public static void CreateCustomizePartsEffectData()
		{
			CreateMasterAsset<master.CustomizePartsEffect, master.CustomizePartsEffect.Data>
				("CustomizeParts/CustomizePartsEffect");
		}

		[MenuItem("Assets/Create/ScriptableObject/MasterData/CustomizeParts/CustomizePartsAreaData")]
		public static void CreateCustomizePartsAreaData()
		{
			CreateMasterAsset<master.CustomizePartsArea, master.CustomizePartsArea.Data>
				("CustomizeParts/CustomizePartsArea");
		}
		#endregion



		#region Resource
		[MenuItem("Assets/Create/ScriptableObject/ResourceData/EnemyColor")]
		public static void CreateEnemyColor()
		{
			CreateResourceAsset<resource.EnemyColorResource, resource.EnemyColorResource.Data>
				("EnemyColor/EnemyColor");
		}

		[MenuItem("Assets/Create/ScriptableObject/ResourceData/CustomizeParts")]
		public static void CreateCustomizeParts()
		{
			CreateResourceAsset<resource.CustomizePartsTextureResource, resource.CustomizePartsTextureResource.Data>
				("CustomizeParts/CustomizeParts");
		}
		#endregion

		//[MenuItem("Assets/Create/ScriptableObject/MasterData/CommonSettingData")]
		//public static void CreateCommonSettingData()
		//{
		//	// アセット作成
		//	var data = ScriptableObject.CreateInstance<data.master.CommonSettingData>();
		//	string inputPath = string.Format("{0}/{1}", CsvBasePath, "CommonSetting/CommonSettingData");
		//	string outputPath = inputPath.Replace("Csv", "Bundle");
		//	CreateAsset(data, inputPath, outputPath);
		//}

		/// <summary>
		/// アセット作成
		/// </summary>
		/// <param name="path"></param>
		private static void CreateMasterAsset<TBase, TData>(string path)
			where TBase : master.MasterDataBase<TData>
			where TData : master.MasterDataBase<TData>.DataBase
		{
			var data = ScriptableObject.CreateInstance<TBase>();

			// エディタからの変更を無効に
			data.hideFlags = HideFlags.NotEditable;

			string inputPath = string.Format("Assets/Contents/MasterData/Text/{0}.txt", path);
			string outputPath = inputPath.Replace("Text", "AssetBundle").Replace(".txt", ".asset");

			// アセット作成
			AssetDatabase.DeleteAsset(outputPath);
			AssetDatabase.CreateAsset(data, outputPath);

			data.Load(CsvStringData.GetCsv(inputPath));
			SaveAsset(data);
		}

		/// <summary>
		/// アセット作成
		/// </summary>
		/// <param name="path"></param>
		private static void CreateResourceAsset<TBase, TData>(string path)
			where TBase : resource.ResourceDataBase<TData>
			where TData : resource.ResourceDataBase<TData>.DataBase
		{
			var data = ScriptableObject.CreateInstance<TBase>();

			// エディタからの変更を無効に
			data.hideFlags = HideFlags.NotEditable;

			string outputPath = string.Format("Assets/Contents/ResourceData/{0}.asset", path);
			
			// アセット作成
			AssetDatabase.DeleteAsset(outputPath);
			AssetDatabase.CreateAsset(data, outputPath);

			data.Setup();
			SaveAsset(data);
		}

		/// <summary>
		/// アセット内容保存
		/// </summary>
		/// <param name="data"></param>
		private static void SaveAsset(Object data)
		{
			// ダーティフラグをセットし、アセット保存
			EditorUtility.SetDirty(data);
			AssetDatabase.SaveAssets();
		}
	}
}