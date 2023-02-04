using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.world.effect
{
	[System.Serializable]
	public abstract class EffectBase : MonoBehaviour
	{
		[SerializeField]
		protected FBXBase m_fbx = null;



		public abstract void Initialize(
			UnityAction effectEvent,
			UnityAction callback);

	}
}
