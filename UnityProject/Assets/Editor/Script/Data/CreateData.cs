using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		private const string k_masterFolderRoot = "Assets/Contents/MasterData";

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
		[MenuItem("Assets/Create/ScriptableObject/MasterData/LocalizeData")]
		public static void CreateLocalizeData()
		{
			CreateMasterAsset<master.LocalizeData, master.LocalizeData.Data>("LocalizeData");
		}

		[MenuItem("Assets/Create/ScriptableObject/MasterData/GameGunreData")]
		public static void CreateGameGunreData()
		{
			CreateMasterAsset<master.GameGunreData, master.GameGunreData.Data>("GameGunreData");
		}

		[MenuItem("Assets/Create/ScriptableObject/MasterData/CheckSheetBugData")]
		public static void CreateCheckSheetBugData()
		{
			CreateMasterAsset<master.CheckSheetBugData, master.CheckSheetBugData.Data>("CheckSheetBugData");
		}

		[MenuItem("Assets/Create/ScriptableObject/MasterData/RewardData")]
		public static void CreateRewardData()
		{
			CreateMasterAsset<master.RewardData, master.RewardData.Data>("RewardData");
		}

		[MenuItem("Assets/Create/ScriptableObject/MasterData/MovieListData")]
		public static void CreateMovieData()
		{
			CreateMasterAssets<master.MovieListData, master.MovieData, master.MovieData.Data>("MovieListData");
		}

		[MenuItem("Assets/Create/ScriptableObject/MasterData/CharaWindowMovieData")]
		public static void CreateCharaWindowMovieData()
		{
			CreateMasterAsset<master.CharaWindowMovieData, master.CharaWindowMovieData.Data>("CharaWindowMovieData");
		}
#endregion

#region Resource
		[MenuItem("Assets/Create/ScriptableObject/ResourceData/EnemyColor")]
		public static void CreateEnemyColor()
		{
			CreateResourceAsset<resource.ColorResource, resource.ColorResource.Data>
				("EnemyColor/EnemyColor");
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
		/// 複数アセット作成（まとめ形式）
		/// </summary>
		/// <typeparam name="TListMaster"></typeparam>
		/// <typeparam name="TMaster"></typeparam>
		/// <typeparam name="TData"></typeparam>
		/// <param name="folderName"></param>
		private static void CreateMasterAssets<TListMaster, TMaster, TData>(string folderName)
			where TListMaster : master.MasterListDataBase<TMaster, TData>
			where TMaster : master.MasterDataBase<TData>
			where TData : master.MasterDataBase<TData>.DataBase
		{
			string folderPath = string.Format("{0}/Text/{1}", k_masterFolderRoot, folderName);
			Debug.Log("folderPath = " + folderPath);
			string[] fileNames = Directory.GetFiles(folderPath, "*.txt");

			List<master.MasterListDataBase<TMaster, TData>.Data> masterListDataList =
				new List<master.MasterListDataBase<TMaster, TData>.Data>();
			for (int i = 0; i < fileNames.Length; ++i)
			{
				Debug.Log("fileNames[i] = " + fileNames[i]);
				string fileName = Path.GetFileNameWithoutExtension(fileNames[i]);
				string createFileName = string.Format("{0}/{1}", folderName, fileName);
				var master = CreateMasterAsset<TMaster, TData>(createFileName);
				masterListDataList.Add(new data.master.MasterListDataBase<TMaster, TData>.Data(
					int.Parse(fileName),
					master));
			}

			var listMaster = ScriptableObject.CreateInstance<TListMaster>();

			// エディタからの変更を無効に
			//data.hideFlags = HideFlags.NotEditable;

			string outputPath = string.Format("{0}/AssetBundle/{1}.asset", k_masterFolderRoot, folderName);

			// アセット作成
			AssetDatabase.DeleteAsset(outputPath);
			AssetDatabase.CreateAsset(listMaster, outputPath);

			listMaster.Load(masterListDataList.ToArray());
			SaveAsset(listMaster);
		}

		/// <summary>
		/// アセット作成
		/// </summary>
		/// <typeparam name="TMaster"></typeparam>
		/// <typeparam name="TData"></typeparam>
		/// <param name="fileName"></param>
		private static TMaster CreateMasterAsset<TMaster, TData>(string fileName)
			where TMaster : master.MasterDataBase<TData>
			where TData : master.MasterDataBase<TData>.DataBase
		{
			var data = ScriptableObject.CreateInstance<TMaster>();

			// エディタからの変更を無効に
			//data.hideFlags = HideFlags.NotEditable;

			string inputPath = string.Format("{0}/Text/{1}.txt", k_masterFolderRoot, fileName);
			string outputPath = inputPath.Replace("Text", "AssetBundle").Replace(".txt", ".asset");

			// アセット作成
			AssetDatabase.DeleteAsset(outputPath);
			AssetDatabase.CreateAsset(data, outputPath);

			data.Load(CsvStringData.GetCsv(inputPath));
			SaveAsset(data);
			return data;
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