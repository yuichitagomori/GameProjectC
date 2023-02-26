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
		private int[] m_floors = null;

		[SerializeField]
		private Vector3[] m_points = null;

		[SerializeField]
		private Stair[] m_stairs = null;


		List<Vector3> navMeshBasePointList = new List<Vector3>();

		public Vector3[] GetNavMeshBasePoints()
		{
			navMeshBasePointList.Clear();
			for (int i = 0; i < 100; ++i)
			{
				int index = UnityEngine.Random.Range(0, m_points.Length);
				navMeshBasePointList.Add(m_points[index]);
			}
			return navMeshBasePointList.ToArray();

			//List<Stair> stairList = new List<Stair>();
			//for (int i = 0; i < 100; ++i)
			//{
			//	int index = UnityEngine.Random.Range(0, m_points.Length);
			//	if (navMeshBasePointList.Count <= 0)
			//	{
			//		navMeshBasePointList.Add(m_points[index]);
			//		continue;
			//	}

			//	Vector3 beforePoint = navMeshBasePointList[navMeshBasePointList.Count - 1];
			//	if (beforePoint.y != m_points[index].y)
			//	{
			//		// äKÇà⁄ìÆÇ∑ÇÈÇÃÇ≈ÅAstairPointÇç∑ÇµçûÇﬁ
			//		stairList.Clear();
			//		for (int j = 0; j < m_stairs.Length; ++j)
			//		{
			//			if (m_stairs[j].Points[0].y == beforePoint.y &&
			//				m_stairs[j].Points[1].y == m_points[index].y)
			//			{
			//				stairList.Add(m_stairs[j]);
			//			}
			//			else if (
			//				m_stairs[j].Points[1].y == beforePoint.y &&
			//				m_stairs[j].Points[0].y == m_points[index].y)
			//			{
			//				stairList.Add(m_stairs[j]);
			//			}
			//		}
			//		Stair nearStair = null;
			//		for (int j = 0; j < stairList.Count; ++j)
			//		{
			//			if (nearStair == null)
			//			{
			//				nearStair = stairList[j];
			//				continue;
			//			}

			//			Vector3 checkBeforeStairPoint = Vector3.zero;
			//			Vector3 checkAfterStairPoint = Vector3.zero;
			//			if (nearStair.Points[0].y == beforePoint.y &&
			//				stairList[j].Points[0].y == beforePoint.y)
			//			{
			//				checkBeforeStairPoint = nearStair.Points[0];
			//				checkAfterStairPoint = stairList[j].Points[0];
			//			}
			//			else if (
			//				nearStair.Points[0].y == beforePoint.y &&
			//				stairList[j].Points[1].y == beforePoint.y)
			//			{
			//				checkBeforeStairPoint = nearStair.Points[0];
			//				checkAfterStairPoint = stairList[j].Points[1];
			//			}
			//			else if (
			//				nearStair.Points[1].y == beforePoint.y &&
			//				stairList[j].Points[0].y == beforePoint.y)
			//			{
			//				checkBeforeStairPoint = nearStair.Points[1];
			//				checkAfterStairPoint = stairList[j].Points[0];
			//			}
			//			else if (
			//				nearStair.Points[1].y == beforePoint.y &&
			//				stairList[j].Points[1].y == beforePoint.y)
			//			{
			//				checkBeforeStairPoint = nearStair.Points[1];
			//				checkAfterStairPoint = stairList[j].Points[1];
			//			}

			//			float magnitudeBefore = (beforePoint - checkBeforeStairPoint).magnitude;
			//			float magnitudeAfter = (beforePoint - checkAfterStairPoint).magnitude;
			//			if (magnitudeAfter < magnitudeBefore)
			//			{
			//				nearStair = stairList[j];
			//			}
			//		}

			//		if (nearStair == null)
			//		{
			//			// äKà⁄ìÆé∏îs
			//			continue;
			//		}

			//		if (nearStair.Points[0].y == beforePoint.y)
			//		{
			//			navMeshBasePointList.Add(nearStair.Points[0]);
			//			navMeshBasePointList.Add(nearStair.Points[1]);
			//		}
			//		else if (nearStair.Points[1].y == beforePoint.y)
			//		{
			//			navMeshBasePointList.Add(nearStair.Points[1]);
			//			navMeshBasePointList.Add(nearStair.Points[0]);
			//		}
			//	}

			//	navMeshBasePointList.Add(m_points[index]);

			//}
			//return navMeshBasePointList.ToArray();
		}

		/// <summary>
		/// (EditorêÍóp)NavmeshÉ|ÉCÉìÉgê›íË
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
			for (int i = 0; i < 1000; ++i)
			{
				float posY = m_floors[UnityEngine.Random.Range(0, m_floors.Length)];
				Vector3 createPos = new Vector3(
					UnityEngine.Random.Range(-100, 100),
					posY,
					UnityEngine.Random.Range(-100, 100));

				if (navmeshPointList.Contains(createPos) == true)
				{
					i--;
					continue;
				}
				if (IsRayHit(createPos) == false)
				{
					i--;
					continue;
				}

				bool isRayHit = true;
				for (int j = 0; j < checkPos.Length; ++j)
				{
					if (IsRayHit(createPos + checkPos[j]) == false)
					{
						isRayHit = false;
						break;
					}
				}
				if (isRayHit == false)
				{
					i--;
					continue;
				}

				navmeshPointList.Add(createPos);
			}

			m_points = navmeshPointList.ToArray();
		}

		private bool IsRayHit(Vector3 position)
		{
			Ray ray = new Ray(position + Vector3.up * 0.5f, Vector3.down);

			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 1.0f))
			{
				return true;
			}
			return false;
		}
	}
}