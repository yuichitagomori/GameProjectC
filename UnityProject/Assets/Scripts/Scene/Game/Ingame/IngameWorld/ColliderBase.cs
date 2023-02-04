using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scene.game.ingame.world
{
	/// <summary>
	/// ※おそらく必要ないクラス
	/// </summary>
	[System.Serializable]
	public class ColliderBase : MonoBehaviour
	{
		[SerializeField]
		private Collider m_collider = null;
		public bool EnableCollider { get { return m_collider.enabled; } set { m_collider.enabled = value; } }

		public bool IsHit(Vector3 _origin, Vector3 _target, float _length)
		{
			RaycastHit hit;
			return IsHit(_origin, _target, _length, out hit);
		}

		public bool IsHit(Vector3 _origin, Vector3 _target, float _length, out RaycastHit _hit)
		{
			var ray = new Ray(_target, _origin - _target);
			return m_collider.Raycast(ray, out _hit, _length);
		}
	}
}
