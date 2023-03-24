﻿using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

namespace scene.game.ingame.world
{
	[System.Serializable]
	public class Enemy : MonoBehaviour
	{
		public enum NavmeshMoveType
		{
			Stop,		// 停止
			WalkNear,	// 近い場所だけを歩く
			WalkFar,    // 遠い場所も歩く
		}

		[SerializeField]
		private string m_actionEvent = "";

		[SerializeField]
		private FBXBase m_fbx = null;

		[SerializeField]
		private EventBase[] m_events = null;

		[SerializeField]
		private float m_navSpeed = 0.0f;

		[SerializeField]
		private NavMeshAgent m_navAgent = null;

		[SerializeField]
		private Common.LODEvent m_lodEvent = null;

		[SerializeField]
		private NavmeshMoveType m_navmeshMoveType = NavmeshMoveType.WalkFar;

		[SerializeField]
		private LineRenderer m_testLine;



		private int m_enemyId = -1;
		public int EnemyId => m_enemyId;

		private int m_controllId = -1;
		public int ControllId => m_controllId;



		private Transform m_transform;

		private bool m_enable = true;

		private Coroutine m_updateNavmeshCoroutine;

		private Vector3[] m_navmeshPointList;

		private Vector3 m_transformPosition = Vector3.zero;
		public Vector3 TransformPosition => m_transformPosition;

		private Quaternion m_transformRotation = Quaternion.identity;

		private float m_updateWaitTime = 0.0f;

		private float m_nowWaitTime = 0.0f;

		private UnityAction m_loopEffectOutEvent;


		public void Initialize(
			int controllId,
			int enemyId,
			Transform ingameCameraTransform,
			Vector3[] navmeshPointList,
			data.resource.EnemyColorResource.Data.ColorData colorData,
			UnityAction<string> enterCallback,
			UnityAction<string> exitCallback)
		{
			m_controllId = controllId;
			m_enemyId = enemyId;

			m_transform = base.transform;
			m_navmeshPointList = navmeshPointList;
			m_navAgent.enabled = false;

			if (m_fbx.Models.Length > 0)
			{
				for (int i = 0; i < m_fbx.Models.Length; ++i)
				{
					if (m_fbx.Models[i].Mesh == null)
					{
						continue;
					}
					//var material = Material.Instantiate(m_fbx.Mesh.materials[0]);
					//m_fbx.Mesh.materials[0] = material;
					var material = m_fbx.Models[i].Mesh.materials[0];

					material.SetColor("_Color1", colorData.Colors1);
					material.SetColor("_Color2", colorData.Colors2);

					material.SetFloat("_SequenceTime", 0.0f);
				}
			}

			if (m_events.Length >= 2)
			{
				string enterEvent = string.Format("SearchIn_{0}", m_controllId);
				m_events[0].Initialize(enterEvent, "", enterCallback, exitCallback);
				string exitEvent = string.Format("SearchOut_{0}", m_controllId);
				m_events[1].Initialize("", exitEvent, enterCallback, exitCallback);
			}

			m_transformPosition = m_transform.position;
			m_transformRotation = m_transform.rotation;
			int initializeNavmeshPointIndex = 0;
			if (m_navmeshMoveType != NavmeshMoveType.Stop)
			{
				initializeNavmeshPointIndex = UnityEngine.Random.Range(0, m_navmeshPointList.Length);
				Vector3 initializePos = m_navmeshPointList[initializeNavmeshPointIndex];
				SetPosition(initializePos);
				Vector3 randomDir = new Vector3(
					UnityEngine.Random.Range(-1.0f, 1.0f),
					0.0f,
					UnityEngine.Random.Range(-1.0f, 1.0f)).normalized;
				m_transformRotation = Quaternion.LookRotation(randomDir, Vector3.up);
				m_transform.SetPositionAndRotation(m_transformPosition, m_transformRotation);
			}
			m_updateNavmeshCoroutine = StartCoroutine(UpdateNavmeshCoroutine(initializeNavmeshPointIndex));

			var LODEventDatas = new Common.LODEvent.Data[]
			{
				new Common.LODEvent.Data(1, 0.0f, () => UpdateLOD(1)),
				new Common.LODEvent.Data(2, 50.0f, () => UpdateLOD(2)),
				new Common.LODEvent.Data(3, 200.0f, () => UpdateLOD(3)),
			};
			m_lodEvent.Initialize(
				LODEventDatas,
				ingameCameraTransform,
				1.0f);
		}

		public void SetEnable(bool value)
		{
			m_enable = value;
		}

		public void SetSequenceTime(float value)
		{
			for (int i = 0; i < m_fbx.Models.Length; ++i)
			{
				if (m_fbx.Models[i].Mesh == null)
				{
					continue;
				}
				var material = m_fbx.Models[i].Mesh.materials[0];
				material.SetFloat("_SequenceTime", value);
			}
		}

		private void FixedUpdate()
		{
			if (m_transform == null) return;

			if (m_enable == false)
			{
				// ここでは更新処理を止めなくてもよい（UpdateNavmeshCoroutine処理側で止めている）
			}

			m_nowWaitTime += Time.deltaTime;

			if (m_nowWaitTime >= m_updateWaitTime)
			{
				float animationUpdateTime = m_nowWaitTime;
				m_nowWaitTime = 0;

				m_transform.SetPositionAndRotation(m_transformPosition, m_transformRotation);

				if (m_fbx.Anime.IsPlaying() == false)
				{
					// Loop再生アニメ中でStopしている状態のみ、更新
					m_fbx.Anime.UpdateFrame(animationUpdateTime);
				}
			}
		}

		public void SearchIn()
		{
			Debug.Log("Enemy SearchIn");
		}

		public void SearchOut()
		{
			Debug.Log("Enemy SearchOut");
		}

		public void OnCharaActionButtonPressed(UnityAction callback)
		{
			StartCoroutine(OnCharaActionButtonPressedCoroutine(callback));
		}

		private IEnumerator OnCharaActionButtonPressedCoroutine(UnityAction callback)
		{
			bool isTarget = false;
			var searchTargetList = GeneralRoot.User.LocalSaveData.SearchTargetList;
			if (searchTargetList.Count > 0)
			{
				var searchTarget = searchTargetList.Find(d =>
					d.EnemyId == m_enemyId &&
					d.ControllId == m_controllId);
				if (searchTarget != null)
				{
					isTarget = true;
				}
			}

			string beforeAnimName = m_fbx.Anime.GetAnimationName();
			string actionEvent = "";
			if (isTarget == true)
			{
				// ムービー側で操作されることになるため、Navmesh更新を止める
				actionEvent = string.Format("Movie_{0}_{1}", 1, m_controllId);
				StopCoroutine(m_updateNavmeshCoroutine);
			}
			else if (string.IsNullOrEmpty(m_actionEvent) == false)
			{
				PlayLoopAnimation("ReactionYes");
				yield return new WaitForSeconds(2.0f);

				actionEvent = m_actionEvent;
				PlayLoopAnimation(beforeAnimName);
			}
			else
			{
				PlayLoopAnimation("ReactionNo");
				yield return new WaitForSeconds(2.0f);

				PlayLoopAnimation(beforeAnimName);
			}

			if (callback != null)
			{
				callback();
			}
		}

		public void LookTarget(Vector3 targetPosition, UnityAction callback)
		{
			StartCoroutine(LookTargetCoroutine(targetPosition, callback));
		}

		private IEnumerator LookTargetCoroutine(Vector3 targetPosition, UnityAction callback)
		{
			Vector3 targetPos = new Vector3(targetPosition.x, m_transformPosition.y, targetPosition.z);
			var dir = targetPos - m_transformPosition;
			var look = Quaternion.LookRotation(dir, Vector3.up);
			float time = 0.0f;
			while (time < 0.5f)
			{
				time += Time.deltaTime;
				float t = time / 0.5f;
				m_transformRotation = Quaternion.Lerp(m_transform.rotation, look, t);
				yield return null;
			}

			if (callback != null)
			{
				callback();
			}
		}

		public void PlayReaction(
			IngameWorld.ReactionType type,
			UnityAction loopEffectOutEvent,
			UnityAction callback)
		{
			StartCoroutine(PlayReactionCoroutine(type, loopEffectOutEvent, callback));
		}

		private IEnumerator PlayReactionCoroutine(
			IngameWorld.ReactionType type,
			UnityAction loopEffectOutEvent,
			UnityAction callback)
		{
			switch (type)
			{
				case IngameWorld.ReactionType.DelightIn:
					{
						bool isDone = false;
						PlayAnimation("ReactionYes", () => { isDone = true; });
						while (!isDone) { yield return null; }

						break;
					}
				case IngameWorld.ReactionType.DelightOut:
					{
						PlayLoopAnimation("Wait");

						break;
					}
				case IngameWorld.ReactionType.Restraint:
					{
						bool isDone = false;
						PlayAnimation("ReactionRestraint", () => { isDone = true; });
						while (!isDone) { yield return null; }

						PlayLoopAnimation("ReactionRestraintLoop");

						break;
					}
			}

			m_loopEffectOutEvent = loopEffectOutEvent;

			if (callback != null)
			{
				callback();
			}
		}

		public Transform GetHeadTransform()
		{
			return m_fbx.HeadTransform;
		}

		public string GetActionEvent()
		{
			if (string.IsNullOrEmpty(m_actionEvent) == false)
			{
				return m_actionEvent;
			}

			bool isTarget = false;
			var searchTargetList = GeneralRoot.User.LocalSaveData.SearchTargetList;
			if (searchTargetList.Count > 0)
			{
				var searchTarget = searchTargetList.Find(d =>
					d.EnemyId == m_enemyId &&
					d.ControllId == m_controllId);
				if (searchTarget != null)
				{
					isTarget = true;
				}
			}

			if (isTarget == true)
			{
				// ムービー再生指定
				return string.Format("Movie_{0}_{1}", 1, m_controllId);
			}

			return "";
		}

		private void PlayLoopAnimation(string name)
		{
			m_fbx.Anime.PlayLoop(name);
			m_fbx.Anime.Stop();
		}

		private void PlayAnimation(string name, UnityAction callback)
		{
			m_fbx.Anime.Play(name, callback);
		}

		private IEnumerator UpdateNavmeshCoroutine(int initializeNavmeshPointIndex)
		{
			// 目標地点更新
			int navPointIndex = initializeNavmeshPointIndex + 1;
			if (navPointIndex >= m_navmeshPointList.Length)
			{
				navPointIndex = 0;
			}

			if (m_navmeshPointList.Length <= 1)
			{
				// 目標地点無し
				PlayLoopAnimation("Wait");
				m_updateNavmeshCoroutine = null;
				yield break;
			}

			if (m_navSpeed <= 0.0f)
			{
				// 移動しない
				PlayLoopAnimation("Wait");
				m_updateNavmeshCoroutine = null;
				yield break;
			}

			if (m_navmeshMoveType == NavmeshMoveType.Stop)
			{
				// 停止
				PlayLoopAnimation("Wait");
				m_updateNavmeshCoroutine = null;
				yield break;
			}

			while (!m_enable) { yield return null; }

			m_navAgent.enabled = true;
			yield return null;
			m_navAgent.SetDestination(m_navmeshPointList[navPointIndex]);
			while (m_navAgent.pathPending == true)
			{
				// 経路探索中
				yield return null;
			}
			Vector3[] navCornerPathes = m_navAgent.path.corners;
			m_navAgent.enabled = false;

			//// 範囲を外れたポイントの補正（負荷高め）
			//for (int i = 0; i < navCornerPathes.Length; ++i)
			//{
			//	NavMeshHit hit;
			//	if (NavMesh.FindClosestEdge(navCornerPathes[i], out hit, NavMesh.AllAreas))
			//	{
			//		navCornerPathes[i] = hit.position;
			//	}
			//}

			if (m_testLine != null)
			{
				// テスト用NavMeshライン描画
				m_testLine.positionCount = navCornerPathes.Length;
				m_testLine.SetPositions(navCornerPathes);
			}

			if (m_fbx.Anime.GetAnimationName() != "Walk")
			{
				PlayLoopAnimation("Walk");
			}

			// 目標地点へ移動
			int cornerIndex = 0;
			while (cornerIndex < navCornerPathes.Length)
			{
				while (!m_enable) { yield return null; }

				Vector3 nowPos = m_transformPosition;
				Vector3 cornerPos = navCornerPathes[cornerIndex];
				Vector3 dir = cornerPos - nowPos;
				float distance = dir.magnitude;
				if (distance < 0.1f)
				{
					SetPosition(cornerPos);
					yield return null;
					cornerIndex++;
					continue;
				}
				float cornerLeapTime = 0.0f;
				float cornerLeapAddTime = 1.0f / (distance / m_navSpeed);
				while (1.0f > cornerLeapTime)
				{
					while (!m_enable) { yield return null; }

					SetPosition(Vector3.Lerp(nowPos, cornerPos, cornerLeapTime));
					var look = Quaternion.LookRotation(dir, Vector3.up);
					m_transformRotation = Quaternion.Lerp(m_transformRotation, look, 0.1f);

					cornerLeapTime += cornerLeapAddTime;

					yield return null;
				}

				SetPosition(cornerPos);
				m_nowWaitTime = m_updateWaitTime;	// コーナー到着時は強制更新
				yield return null;

				cornerIndex++;
			}

			// 目的地点への移動が完了すると、すこし待機
			PlayLoopAnimation("Wait");
			yield return new WaitForSeconds(UnityEngine.Random.Range(1.0f, 5.0f));

			yield return UpdateNavmeshCoroutine(navPointIndex);
		}

		private void UpdateLOD(int controllId)
		{
			switch (controllId)
			{
				case 1:
					{
						m_updateWaitTime = 0.01f;
						for (int i = 0; i < m_fbx.Models.Length; ++i)
						{
							m_fbx.Models[i].ModelObject.SetActive(i == 0);
						}
						break;
					}
				case 2:
					{
						m_updateWaitTime = 0.2f;
						for (int i = 0; i < m_fbx.Models.Length; ++i)
						{
							m_fbx.Models[i].ModelObject.SetActive(i == 1);
						}
						break;
					}
				case 3:
					{
						m_updateWaitTime = 0.5f;
						for (int i = 0; i < m_fbx.Models.Length; ++i)
						{
							m_fbx.Models[i].ModelObject.SetActive(i == 2);
						}
						break;
					}
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
