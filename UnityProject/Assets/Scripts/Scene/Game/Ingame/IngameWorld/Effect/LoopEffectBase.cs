using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.world.effect
{
	[System.Serializable]
	public abstract class LoopEffectBase : MonoBehaviour
	{
		public abstract void PlayIn(UnityAction callback);

		public abstract void PlayOut(UnityAction callback);
	}
}
