using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.world
{
	public class NavMeshController : MonoBehaviour
	{
		[System.Serializable]
		public class Stair
		{
			[SerializeField]
			private Vector3[] m_points = null;
			public Vector3[] Points => m_points;
		}

		[SerializeField]
		private Vector3 m_pointRangeMin;

		[SerializeField]
		private Vector3 m_pointRangeMax;

		[SerializeField]
		private int m_setPointCount;

		[SerializeField]
		private Vector3[] m_points = null;


		private List<Vector3> navMeshBasePointList = new List<Vector3>();



		public Vector3[] GetNavMeshBasePoints(int count)
		{
			navMeshBasePointList.Clear();
			if (m_points.Length > 0)
			{
				for (int i = 0; i < count; ++i)
				{
					int index = UnityEngine.Random.Range(0, m_points.Length);
					navMeshBasePointList.Add(m_points[index]);
				}
			}
			return navMeshBasePointList.ToArray();
		}

		/// <summary>
		/// (Editor専用)Navmeshポイント設定
		/// </summary>
		public void SetNavmeshBasePoint()
		{
			List<Vector3> navmeshPointList = new List<Vector3>();
			Vector3[] checkPos = new Vector3[]
			{
				new Vector3(-2, 0, -2),
				new Vector3(-2, 0, 2),
				new Vector3(2, 0, -2),
				new Vector3(2, 0, 2)
			};
			for (int i = 0; i < m_setPointCount * 100; ++i)
			{
				float x = Mathf.FloorToInt(UnityEngine.Random.Range(m_pointRangeMin.x, m_pointRangeMax.x));
				float y = Mathf.FloorToInt(UnityEngine.Random.Range(m_pointRangeMin.y, m_pointRangeMax.y));
				float z = Mathf.FloorToInt(UnityEngine.Random.Range(m_pointRangeMin.z, m_pointRangeMax.z));
				Vector3 createPos = new Vector3(x, y, z);
				Vector3 rayHitPos = GetRayHitPosition(createPos);
				if (rayHitPos == Vector3.zero)
				{
					continue;
				}

				if (navmeshPointList.Contains(rayHitPos) == true)
				{
					continue;
				}

				bool isRayHit = true;
				for (int j = 0; j < checkPos.Length; ++j)
				{
					if (GetRayHitPosition(createPos + checkPos[j]) == Vector3.zero)
					{
						isRayHit = false;
						break;
					}
				}
				if (isRayHit == false)
				{
					continue;
				}

				navmeshPointList.Add(rayHitPos);

				if (navmeshPointList.Count >= m_setPointCount)
				{
					break;
				}
			}

			m_points = navmeshPointList.ToArray();
		}

		private Vector3 GetRayHitPosition(Vector3 position)
		{
			Ray ray = new Ray(position + Vector3.up, Vector3.down);

			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 2.0f))
			{
				return hit.point;
			}
			return Vector3.zero;
		}
	}
}