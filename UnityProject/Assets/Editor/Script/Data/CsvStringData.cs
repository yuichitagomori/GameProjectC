using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

/// <summary>
/// CSVデータクラス
/// </summary>
public class CsvStringData
{
	/// <summary>
	/// 読み込み
	/// </summary>
	/// <param name="inputPath"></param>
	/// <returns></returns>
	public static string[][] GetCsv(string inputPath)
	{
		Debug.Log("inputPath = " + inputPath);
		List<string[]> stringLists = new List<string[]>();
		TextAsset asset = (TextAsset)AssetDatabase.LoadAssetAtPath(inputPath, typeof(TextAsset));
		string[] separator = { "\r\n" };
		string[] splits = asset.text.Trim().Split(separator, System.StringSplitOptions.None);
		List<string> splitList = new List<string>(splits);
		foreach (string split in splitList)
		{
			stringLists.Add(split.Split('\t'));
		}
		return stringLists.ToArray();
	}
}