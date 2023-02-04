using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace scene.game.ingame.world
{
	[CustomEditor(typeof(MapCollider))]
	public class MapColliderEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			MapCollider mapCollider = target as MapCollider;

			if (GUILayout.Button("SetupColliderMeshOn"))
			{
				var colliders = mapCollider.GetComponentsInChildren<Collider>();
				mapCollider.SetupColliderMeshOn(colliders);
			}

			if (GUILayout.Button("SetupColliderMeshOff"))
			{
				mapCollider.SetupColliderMeshOff();
			}

			base.OnInspectorGUI();
		}
	}
}
