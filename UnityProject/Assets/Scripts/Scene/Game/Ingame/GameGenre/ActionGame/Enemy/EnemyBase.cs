using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.actiongame
{
	[System.Serializable]
	public abstract class EnemyBase : ObjectBase
	{
		[SerializeField]
		protected GameObject m_colliderObject;



		public virtual new void Initialize(UnityAction<string[]> onEvent)
		{
			base.Initialize(onEvent);
		}
	}
}
