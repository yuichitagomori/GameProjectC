using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// スクリプタブルオブジェクトデータ基底クラス
/// </summary>
public abstract class ScriptableObjectData : ScriptableObject
{
    /// <summary>
    /// 読み込み
    /// </summary>
    /// <param name="_data"></param>
    public abstract void Load(List<List<string>> _csv);
}
