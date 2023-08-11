using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scene.game.ingame.world
{
	[System.Serializable]
	public class FBXBase : MonoBehaviour
	{
		[System.Serializable]
		public class Model
		{
			[SerializeField]
			private GameObject m_modelObject = null;
			public GameObject ModelObject => m_modelObject;

			[SerializeField]
			private SkinnedMeshRenderer m_mesh = null;
			public SkinnedMeshRenderer Mesh => m_mesh;
		}

		[SerializeField]
		private GameObject m_fbxObject = null;
		public GameObject FBXObject => m_fbxObject;

		[SerializeField]
		private Common.AnimatorExpansion m_anime = null;
		public Common.AnimatorExpansion Anime => m_anime;

		[SerializeField]
		private Model[] m_models = null;
		public Model[] Models => m_models;

		[SerializeField]
		private Transform m_armatureRootTransform = null;
		public Transform ArmatureRootTransform => m_armatureRootTransform;
	}
}