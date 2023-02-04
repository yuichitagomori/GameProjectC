using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scene.game.ingame.world
{
	[System.Serializable]
	public class Hand : MonoBehaviour
	{
		[SerializeField]
		private FBXBase m_fbx = null;



		private Transform m_transform = null;
		public Transform Transform => m_transform;

		public AnimatorExpansion Anime => m_fbx.Anime;

		public void Initialize()
		{
			m_transform = transform;
		}
	}
}