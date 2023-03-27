using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//#if UNITY_EDITOR
//using UnityEditor;
//#endif

namespace CommonUI
{
	/// <summary>
	/// イベント拡張機能
	/// </summary>
	public class TextExpansion : Text
	{
		//private new void Start()
		//{
		//	base.Start();
		//}

		private void Setting()
		{
			fontSize = 48;
			supportRichText = false;
			raycastTarget = false;
		}

		protected override void Reset()
		{
			Setting();
			base.Reset();
		}
	}
}