using System.Collections;
using System.Collections.Generic;
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
		private Transform m_faceChangerTransform;

		[SerializeField]
		private Material m_faceMaterial;

		[SerializeField]
		private NavMeshMover m_mover;

		[SerializeField]
		private Common.LODEvent m_lodEvent;



		private int m_npcId;
		public int NPCId => m_npcId;

		private int m_colorId;
		public int ColorId => m_colorId;

		private Transform m_transform;

		private float m_updateWaitTime = 0.0f;

		private float m_nowWaitTime = 0.0f;

		private string m_defaultActionEventParam = "";

		private UnityAction m_loopEffectOutEvent;

		private Coroutine m_lockTargetCoroutine;

		private Vector3[] m_navmeshPointList;



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

			m_mover.Initialize(
				m_transform.SetPositionAndRotation,
				() => { PlayLoopAnimation("Walk"); },
				() => { PlayLoopAnimation("Wait"); });

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

			//EventBase.Data[] eventDatas = new EventBase.Data[]
			//{
			//	new EventBase.Data(
			//		EventBase.Data.Type.Enter,
			//		string.Format("SearchIn_{0}_{1}", (int)Category.NPC, m_controllId),
			//		new EventBase.Data.ColliderFilter[]{ EventBase.Data.ColliderFilter.SearchIn },
			//		false),
			//	new EventBase.Data(
			//		EventBase.Data.Type.Exit,
			//		string.Format("SearchOut_{0}_{1}", (int)Category.NPC, m_controllId),
			//		new EventBase.Data.ColliderFilter[]{ EventBase.Data.ColliderFilter.SearchOut },
			//		false),
			//};
			//m_event.Initialize(eventDatas, eventCallback);

			if (param != null)
			{
				m_defaultActionEventParam = param.ActionEventParam;
			}

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

			StartCoroutine(MoveCoroutine());
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

		public IEnumerator MoveCoroutine()
		{
			while (true)
			{
				int index = UnityEngine.Random.Range(0, m_navmeshPointList.Length);
				Vector3 setPosition = m_navmeshPointList[index];
				bool isDone = false;
				m_mover.Move(setPosition, () => { isDone = true; });
				while (!isDone) { yield return null; }
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
			Vector3 targetPos = new Vector3(targetPosition.x, m_transform.position.y, targetPosition.z);
			var dir = targetPos - m_transform.position;
			var look = Quaternion.LookRotation(dir, Vector3.up);
			float time = 0.0f;
			while (time < 0.5f)
			{
				time += Time.deltaTime;
				float t = time / 0.5f;
				m_transform.rotation = Quaternion.Lerp(m_transform.rotation, look, t);
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

		
	}
}
