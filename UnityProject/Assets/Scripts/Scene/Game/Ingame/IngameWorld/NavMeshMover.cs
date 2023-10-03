using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

namespace scene.game.ingame.world
{
	[System.Serializable, RequireComponent(typeof(NavMeshAgent))]
	public class NavMeshMover : MonoBehaviour
	{
		[System.Serializable]
		public class Data
		{
			[SerializeField]
			private float m_speed = 0.0f;
			public float Speed => m_speed;
		}

		[SerializeField]
		private Data m_data;

		[SerializeField]
		private NavMeshAgent m_navAgent = null;

		[SerializeField]
		private LineRenderer m_testLine;




		private Vector3 m_transformPosition = Vector3.zero;

		private Quaternion m_transformRotation = Quaternion.identity;

		private UnityAction<Vector3, Quaternion> m_updateTransformEvent;

		private Coroutine m_updateNavmeshCoroutine;

		private UnityAction m_moveCallback;

		private UnityAction m_stopCallback;



		public void Initialize(
			UnityAction<Vector3, Quaternion> updateTransformEvent,
			UnityAction moveCallback,
			UnityAction stopCallback)
		{
			m_updateTransformEvent = updateTransformEvent;
			m_moveCallback = moveCallback;
			m_stopCallback = stopCallback;

			m_navAgent.enabled = false;
		}

		public void Stop()
		{
			if (m_updateNavmeshCoroutine != null)
			{
				StopCoroutine(m_updateNavmeshCoroutine);
			}
		}

		public void Move(Vector3 position, UnityAction callback)
		{
			if (m_updateNavmeshCoroutine != null)
			{
				StopCoroutine(m_updateNavmeshCoroutine);
			}
			m_updateNavmeshCoroutine = StartCoroutine(MoveCoroutine(position, callback));
		}

		private IEnumerator MoveCoroutine(Vector3 position, UnityAction callback)
		{
			if (m_data.Speed <= 0.0f)
			{
				// 移動しない
				m_stopCallback();
				m_updateNavmeshCoroutine = null;
				yield break;
			}

			m_navAgent.enabled = true;
			yield return null;

			if (m_navAgent.SetDestination(position) == false)
			{
				// 目的地が正常に設定されていないので、移動しない
				m_stopCallback();
				m_updateNavmeshCoroutine = null;
				yield break;
			}
			while (m_navAgent.pathPending == true)
			{
				// 経路探索中
				yield return null;
			}
			
			Vector3[] navCornerPathes = m_navAgent.path.corners;
			m_navAgent.enabled = false;

			List<Vector3> navCornerPathList = new List<Vector3>();
			for (int i = 0; i < navCornerPathes.Length; ++i)
			{
				Vector3 nowPath = navCornerPathes[i];
				if (i >= navCornerPathes.Length - 1)
				{
					// 最後のパスはただ追加
					navCornerPathList.Add(nowPath);
					break;
				}
				Vector3 nextPath = navCornerPathes[i + 1];
				if (Mathf.Round(nowPath.y * 10) / 10.0f == Mathf.Round(nextPath.y * 10) / 10.0f)
				{
					// 平面上は追加
					navCornerPathList.Add(nowPath);
					continue;
				}

				int tryCount = 0;
				while (tryCount < 1000)
				{
					tryCount++;
					// 高さが違う２つのパスでは、
					Vector3 dir = (nextPath - nowPath).normalized;
					Vector3 checkPos = nowPath + dir;
					Ray ray = new Ray(checkPos + Vector3.up * 5.0f, Vector3.down);
					var hits = Physics.RaycastAll(ray, 10.0f);
					if (hits.Length <= 0)
					{
						break;
					}
					nowPath = hits[0].point;
					if (Mathf.Round(nowPath.y * 10) / 10.0f == Mathf.Round(checkPos.y * 10) / 10.0f)
					{
						continue;
					}
					navCornerPathList.Add(nowPath);
					if ((nextPath - nowPath).magnitude <= 1.0f)
					{
						break;
					}
				}
			}

			if (m_testLine != null)
			{
				// テスト用NavMeshライン描画
				m_testLine.positionCount = navCornerPathList.Count;
				m_testLine.SetPositions(navCornerPathes.ToArray());
			}

			m_moveCallback();

			// 目標地点へ移動
			//for (int i = 0; i < navCornerPathList.Count; ++i)
			//{
			//	Vector3 nowPos = m_transformPosition;
			//	Vector3 cornerPos = navCornerPathList[i];
			//	Vector3 dir = cornerPos - nowPos;
			//	float cornerLeapTime = 0.0f;
			//	float cornerLeapAddTime = 1.0f / (dir.magnitude / m_data.Speed);
			//	while (1.0f > cornerLeapTime)
			//	{
			//		SetPosition(Vector3.Lerp(nowPos, cornerPos, cornerLeapTime));
			//		var look = Quaternion.LookRotation((cornerPos - nowPos).normalized, Vector3.up);
			//		m_transformRotation = Quaternion.Lerp(m_transformRotation, look, 0.1f);
			//		m_updateTransformEvent(m_transformPosition, m_transformRotation);

			//		cornerLeapTime += cornerLeapAddTime * 0.05f;

			//		yield return new WaitForFixedUpdate();
			//	}

			//	SetPosition(cornerPos);
			//	m_updateTransformEvent(m_transformPosition, m_transformRotation);
			//}

			if (gameObject.name == "Player")
			{
				Debug.Log("count = " + navCornerPathList.Count);
				for (int i = 0; i < navCornerPathList.Count; ++i)
				{
					Debug.Log("path = " + navCornerPathList[i].ToString());
				}
			}
			for (int i = 0; i < navCornerPathList.Count; ++i)
			{
				Vector3 nowPath = navCornerPathList[i];
				if (i >= navCornerPathList.Count - 1)
				{
					SetPosition(nowPath);
					m_updateTransformEvent(m_transformPosition, m_transformRotation);
					break;
				}

				float nowDistance = 0.0f;
				Vector3 nextPath = navCornerPathList[i + 1];
				while (true)
				{
					Vector3 distance = (nextPath - nowPath);
					nowDistance += distance.magnitude;
					if (nowDistance >= m_data.Speed)
					{
						break;
					}

					i++;
					if (i >= navCornerPathList.Count - 1)
					{
						break;
					}
					nextPath = navCornerPathList[i + 1];
				}

				float nowTime = 0.0f;
				while (nowTime < m_data.Speed)
				{
					nowTime += Time.deltaTime;
					SetPosition(Vector3.Lerp(nowPath, nextPath, nowTime));
					var look = Quaternion.LookRotation((nextPath - nowPath).normalized, Vector3.up);
					m_transformRotation = Quaternion.Lerp(m_transformRotation, look, 0.1f);
					m_updateTransformEvent(m_transformPosition, m_transformRotation);

					yield return new WaitForFixedUpdate();
				}

				Debug.Log("i = " + i);
			}

			// 目的地点への移動が完了すると、すこし待機
			m_stopCallback();
			yield return new WaitForSeconds(UnityEngine.Random.Range(1.0f, 5.0f));

			if (callback != null)
			{
				callback();
			}
		}

		private void SetPosition(Vector3 pos)
		{
			m_transformPosition = pos;
			Ray ray = new Ray(m_transformPosition + Vector3.up * 5.0f, Vector3.down);
			var hits = Physics.RaycastAll(ray, 10.0f);
			if (hits.Length > 0)
			{
				for (int i = 0; i < hits.Length; ++i)
				{
					if (hits[i].collider.tag == "IgnoreRaycast")
					{
						continue;
					}
					m_transformPosition = hits[i].point;
					break;
				}
			}
		}
	}
}
