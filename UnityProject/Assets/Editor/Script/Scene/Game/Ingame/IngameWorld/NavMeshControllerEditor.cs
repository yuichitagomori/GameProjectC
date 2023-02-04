using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace scene.game.ingame.world
{
	[CustomEditor(typeof(NavMeshController))]
	public class NavMeshControllerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			NavMeshController navMeshController = target as NavMeshController;

			if (GUILayout.Button("SetNavmeshBasePoint"))
			{
				navMeshController.SetNavmeshBasePoint();
			}

			base.OnInspectorGUI();
		}
	}
}
