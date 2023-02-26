using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scene.game.ingame.world
{
	[System.Serializable]
	public class MapCollider : MonoBehaviour
	{
		[SerializeField]
		private Collider[] m_colliderList = null;



		/// <summary>
		/// アクティブ設定
		/// </summary>
		/// <param name="value"></param>
		public void SetActive(bool value)
		{
			for (int i = 0; i < m_colliderList.Length; ++i)
			{
				m_colliderList[i].enabled = value;
			}
		}

		/// <summary>
		/// (Editor専用)Navmesh設定のための、コライダーメッシュ更新とStatic設定
		/// </summary>
		/// <param name="colliders"></param>
		public void SetupColliderMeshOn(Collider[] colliders)
		{
			m_colliderList = colliders;
			Debug.Log("m_colliderList.Length = " + m_colliderList.Length);

			var flag = UnityEditor.StaticEditorFlags.NavigationStatic;
			for (int i = 0; i < m_colliderList.Length; ++i)
			{
				var obj = m_colliderList[i].gameObject;
				UnityEditor.GameObjectUtility.SetStaticEditorFlags(obj, flag);
				var mesh = obj.GetComponent<MeshRenderer>();
				mesh.enabled = true;
				obj.tag = "Untagged";
			}
		}

		/// <summary>
		/// (Editor専用)Navmesh設定のための、コライダーメッシュ更新とStatic設定
		/// </summary>
		public void SetupColliderMeshOff()
		{
			for (int i = 0; i < m_colliderList.Length; ++i)
			{
				var obj = m_colliderList[i].gameObject;
				var mesh = obj.GetComponent<MeshRenderer>();
				mesh.enabled = false;
			}
		}
	}
}
