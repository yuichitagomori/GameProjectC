using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scene.game.ingame.world
{
	[SerializeField]
	public class TransformData : MonoBehaviour
	{
		[SerializeField]
		private Vector3 m_position = Vector3.zero;

		[SerializeField]
		private Vector3 m_euler = Vector3.zero;



		public void SetupTransform(Transform transform)
		{
			transform.position = m_position;
			transform.rotation = Quaternion.Euler(m_euler);
		}
	}
}
