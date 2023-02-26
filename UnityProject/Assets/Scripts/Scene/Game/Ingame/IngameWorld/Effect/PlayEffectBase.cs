using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.world.effect
{
	[System.Serializable]
	public abstract class PlayEffectBase : MonoBehaviour
	{
		public abstract void Play(UnityAction callback);

	}
}
