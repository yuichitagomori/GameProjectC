using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame
{
	[System.Serializable]
	public class ObjectBase : MonoBehaviour
	{
		[SerializeField]
		private EventBase m_event;

		[SerializeField]
		protected FBXBase m_fbx;



		private Transform m_transform = null;
		public new Transform transform => m_transform;



		public void Initialize(UnityAction<string[]> callback)
		{
			m_transform = base.transform;
			m_event.Initialize(callback);
		}
	}
}
