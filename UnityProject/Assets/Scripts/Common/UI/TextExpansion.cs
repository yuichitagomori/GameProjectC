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
		public void PlayProgression(string str, float waitTime, UnityAction callback)
		{
			StartCoroutine(PlayProgressionCoroutine(str, waitTime, callback));
		}

		private IEnumerator PlayProgressionCoroutine(string str, float waitTime, UnityAction callback)
		{
			text = "";
			yield return null;

			var wait = new WaitForSeconds(waitTime);

			while (text.Length < str.Length)
			{
				text = str.Substring(0, text.Length + 1);
				yield return wait;
				yield return null;
			}

			if (callback != null)
			{
				callback();
			}
		}

#if UNITY_EDITOR
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
#endif
	}
}