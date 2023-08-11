using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.world
{
	[System.Serializable]
	public abstract class ActionTargetBase : MonoBehaviour
	{
		public enum Category
		{
			NPC = 0,
			Object = 1
		}

		[SerializeField]
		protected FBXBase m_fbx = null;

		[SerializeField]
		protected Transform m_headTransform = null;
		public Transform HeadTransform => m_headTransform;

		[SerializeField]
		protected EventBase m_event = null;



		protected int m_controllId = -1;
		public int ControllId => m_controllId;

		protected Vector3 m_transformPosition = Vector3.zero;
		public Vector3 TransformPosition => m_transformPosition;

		protected bool m_enable = true;

		public void SetEnable(bool value)
		{
			m_enable = value;
		}

		public abstract string GetActionEventParam();

		public abstract void SearchIn();

		public abstract void SearchOut();

		public abstract void OnCharaActionButtonPressed(UnityAction callback);

		public abstract void PlayReaction(
			IngameWorld.ReactionType type,
			UnityAction loopEffectOutEvent,
			UnityAction callback);
	}
}
