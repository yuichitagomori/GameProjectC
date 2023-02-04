using UnityEditor;
using UnityEngine;
using System.Collections;

namespace Common
{
	[CustomEditor(typeof(ElementList))]
	public class ElementListEditor : Editor
	{
		/// <summary>
		/// InspectorのGUIを更新
		/// </summary>
		public override void OnInspectorGUI()
		{
			ElementList elementList = target as ElementList;

			if (GUILayout.Button("SetupElements"))
			{
				elementList.SetupElements();
			}
			if (GUILayout.Button("DestroyElements"))
			{
				elementList.DestroyElements();
			}

			base.OnInspectorGUI();
		}
	}
}