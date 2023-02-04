using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.world
{
	[System.Serializable]
	public class ObjectBase : MonoBehaviour
	{
		[SerializeField]
		private FBXBase m_fbx = null;

		[SerializeField]
		private EventBase m_event = null;

		[SerializeField]
		private Collider m_collider = null;


		private Transform m_transform = null;
		public new Transform transform => m_transform;



		public void Initialize(
			UnityAction<string> enterCallback,
			UnityAction<string> exitCallback)
		{
			m_event.Initialize(enterCallback, exitCallback);
			m_transform = base.transform;
		}
	}
}
