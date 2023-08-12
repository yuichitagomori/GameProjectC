using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

namespace scene.game.ingame.world
{
	[System.Serializable]
	public class NPC : ActionTargetBase
	{
		[SerializeField]
		private Transform m_faceChangerTransform = null;

		[SerializeField]
		private Material m_faceMaterial = null;

		[SerializeField]
		private float m_navSpeed = 0.0f;

		[SerializeField]
		private NavMeshAgent m_navAgent = null;

		[SerializeField]
		private Common.LODEvent m_lodEvent = null;

		[SerializeField]
		private LineRenderer m_testLine;



		private int m_npcId;
		public int NPCId => m_npcId;

		private int m_colorId;
		public int ColorId => m_colorId;

		private Transform m_transform;

		private Coroutine m_updateNavmeshCoroutine;

		private Vector3[] m_navmeshPointList;

		private Quaternion m_transformRotation = Quaternion.identity;

		private float m_updateWaitTime = 0.0f;

		private float m_nowWaitTime = 0.0f;

		private string m_defaultActionEventParam = "";

		private UnityAction m_loopEffectOutEvent;

		private Coroutine m_lockTargetCoroutine;



		public void Initialize(
			int npcId,
			int colorId,
			int controllId,
			NPCController.Data.Parameter param,
			Transform ingameCameraTransform,
			Vector3[] navmeshPointList,
			UnityAction<string> eventCallback)
		{
			m_controllId = controllId;

			m_npcId = npcId;
			m_colorId = colorId;

			m_transform = base.transform;

			m_navmeshPointList = navmeshPointList;
			m_navAgent.enabled = false;

			if (m_fbx.Models.Length > 0)
			{
				var colorResource = GeneralRoot.Resource.ColorResource;
				var colorResourceData = colorResource.Find(m_npcId);
				var colorData = colorResourceData.Find(colorId);
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
					material.SetFloat("_Fresnel", 0.0f);
				}
			}

			EventBase.Data[] eventDatas = new EventBase.Data[]
			{
				new EventBase.Data(
					EventBase.Data.Type.Enter,
					string.Format("SearchIn_{0}_{1}", (int)Category.NPC, m_controllId),
					new EventBase.Data.ColliderFilter[]{ EventBase.Data.ColliderFilter.SearchIn },
					false),
				new EventBase.Data(
					EventBase.Data.Type.Exit,
					string.Format("SearchOut_{0}_{1}", (int)Category.NPC, m_controllId),
					new EventBase.Data.ColliderFilter[]{ EventBase.Data.ColliderFilter.SearchOut },
					false),
			};
			m_event.Initialize(eventDatas, eventCallback);

			if (param != null)
			{
				m_transform.SetPositionAndRotation(param.TransformData.position, param.TransformData.rotation);
				m_defaultActionEventParam = param.ActionEventParam;
				PlayLoopAnimation("Wait");
			}
			else
			{
				int initializeNavmeshPointIndex = UnityEngine.Random.Range(0, m_navmeshPointList.Length);
				Vector3 initializePos = m_navmeshPointList[initializeNavmeshPointIndex];
				Vector3 randomDir = new Vector3(
					UnityEngine.Random.Range(-1.0f, 1.0f),
					0.0f,
					UnityEngine.Random.Range(-1.0f, 1.0f)).normalized;
				m_transform.SetPositionAndRotation(
					initializePos,
					Quaternion.LookRotation(randomDir, Vector3.up));
				m_updateNavmeshCoroutine = StartCoroutine(UpdateNavmeshCoroutine(initializeNavmeshPointIndex));
			}
			SetPosition(m_transform.position);
			m_transformRotation = m_transform.rotation;

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
			if (m_faceChangerTransform != null)
			{
				// enable = false 状態でも表情切り替えを行いたいので、手前で更新処理をはさむ
				int offsetUIndex = Mathf.RoundToInt(m_faceChangerTransform.localPosition.x * 100);
				int offsetVIndex = Mathf.RoundToInt(m_faceChangerTransform.localPosition.y * 100);
				float offsetU = 0.125f * offsetUIndex;
				float offsetV = 0.125f * offsetVIndex;
				m_faceMaterial.SetVector("_UVOffset", new Vector4(offsetU, offsetV, 0.0f, 0.0f));
			}

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

		public override void SearchIn(Vector3 playerPosition)
		{
			if (m_fbx.Models.Length > 0)
			{
				for (int i = 0; i < m_fbx.Models.Length; ++i)
				{
					if (m_fbx.Models[i].Mesh == null)
					{
						continue;
					}
					var material = m_fbx.Models[i].Mesh.materials[0];
					material.SetFloat("_Fresnel", 1.0f);
				}
			}
			m_enable = false;
			LookTarget(playerPosition, null);
		}

		public override void SearchOut()
		{
			if (m_fbx.Models.Length > 0)
			{
				for (int i = 0; i < m_fbx.Models.Length; ++i)
				{
					if (m_fbx.Models[i].Mesh == null)
					{
						continue;
					}
					var material = m_fbx.Models[i].Mesh.materials[0];
					material.SetFloat("_Fresnel", 0.0f);
				}
			}
			m_enable = true;
		}

		public override void OnCharaActionButtonPressed(UnityAction callback)
		{
			StartCoroutine(OnCharaActionButtonPressedCoroutine(callback));
		}

		private IEnumerator OnCharaActionButtonPressedCoroutine(UnityAction callback)
		{
			string beforeAnimName = m_fbx.Anime.GetAnimationName();
			if (string.IsNullOrEmpty(m_defaultActionEventParam) == false)
			{
				PlayLoopAnimation("ReactionYes");
				yield return new WaitForSeconds(2.0f);

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

		private void LookTarget(Vector3 targetPosition, UnityAction callback)
		{
			if (m_lockTargetCoroutine != null)
			{
				StopCoroutine(m_lockTargetCoroutine);
			}
			m_lockTargetCoroutine = StartCoroutine(LookTargetCoroutine(targetPosition, callback));
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

		public override void PlayReaction(
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

		public override string GetActionEventParam()
		{
			if (string.IsNullOrEmpty(m_defaultActionEventParam) == false)
			{
				return m_defaultActionEventParam;
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
