using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.world
{
	[System.Serializable]
	public class PlayerChara : MonoBehaviour
	{
		[SerializeField]
		private Rigidbody m_rigidbody;

		[SerializeField]
		private FBXBase m_fbx;



		private Transform m_transform = null;
		public new Transform transform => m_transform;

		private bool m_enable = true;

		private Transform m_cameraTransform = null;

		//private Vector3 m_moveVector = Vector3.zero;

		//private float m_moveSpeed = 0.0f;

		//private float m_moveSpeedMax = 0.0f;


		public void Initialize(Transform ingameCameraTransform)
		{
			m_transform = base.transform;
			m_cameraTransform = ingameCameraTransform;

			//StartCoroutine(UpdateCoroutine());
		}

		public void Move(Vector3 position)
		{
		}

		public void DragEvent(Vector2 direction)
		{
			//Vector3 euler = new Vector3(0.0f, m_cameraTransform.eulerAngles.y, 0.0f);
			//m_moveVector = (Quaternion.Euler(euler) * new Vector3(direction.x, 0.0f, direction.y)).normalized;
		}

		public void EndDragEvent()
		{
			//m_moveVector = Vector3.zero;
		}

		public void SetEnable(bool value)
		{
			m_enable = value;
		}

		public void SearchIn()
		{
		}

		public void SearchOut()
		{
		}

		public void UpdateParam()
		{
			//var localSaveData = GeneralRoot.User.LocalSaveData;
			//var customizeData = localSaveData.Customize;
			//var customizePartsMaster = GeneralRoot.Master.CustomizeParts;
			//var customizePartsEffectMaster = GeneralRoot.Master.CustomizePartsEffect;
			//m_moveSpeedMax = 5.0f;
			//for (int i = 0; i < customizeData.BoardPartsDatas.Length; ++i)
			//{
			//	var boardPartsData = customizeData.BoardPartsDatas[i];
			//	var userPartsData = localSaveData.UniqueItemList.Find(d => d.UniqueId == boardPartsData.UniqueId);
			//	var customizePartsMasterData = customizePartsMaster.Find(userPartsData.Id);
			//	var customizePartsEffectMasterData = customizePartsEffectMaster.Find(customizePartsMasterData.EffectId);
			//	switch (customizePartsEffectMasterData.Category)
			//	{
			//		case data.master.CustomizePartsEffect.Data.CaterogyType.SpeedUp:
			//			{
			//				m_moveSpeedMax += customizePartsEffectMasterData.Param * 2;
			//				break;
			//			}
			//	}
			//}
		}

		public void OnCharaActionButtonPressed(UnityAction callback)
		{
			//m_moveVector = Vector3.zero;

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

		public void PlayReaction(IngameWorld.ReactionType type, UnityAction callback)
		{
			StartCoroutine(PlayReactionCoroutine(type, callback));
		}

		private IEnumerator PlayReactionCoroutine(IngameWorld.ReactionType type, UnityAction callback)
		{
			switch (type)
			{
				case IngameWorld.ReactionType.DelightIn:
					{
						int doneCount = 0;
						LookTarget(m_cameraTransform.position, () => { doneCount++; });
						m_fbx.Anime.Play("ReactionDelight", () => { doneCount++; });
						while (doneCount < 2) { yield return null; }

						break;
					}
				case IngameWorld.ReactionType.DelightOut:
					{
						bool isDone = false;
						m_fbx.Anime.Play("ReactionDelight", () => { isDone = true; });
						while (!isDone) { yield return null; }

						m_fbx.Anime.PlayLoop("Wait");

						break;
					}
				case IngameWorld.ReactionType.Restraint:
					{
						bool isDone = false;
						m_fbx.Anime.Play("ReactionRestraint", () => { isDone = true; });
						while (!isDone) { yield return null; }

						m_fbx.Anime.PlayLoop("Wait");

						break;
					}
			}

			if (callback != null)
			{
				callback();
			}
		}

		private void FixedUpdate()
		{
			//// enable = false 状態でも表情切り替えを行いたいので、手前で更新処理をはさむ
			//int offsetUIndex = Mathf.RoundToInt(m_faceChangerTransform.localPosition.x * 100);
			//int offsetVIndex = Mathf.RoundToInt(m_faceChangerTransform.localPosition.y * 100);
			//float offsetU = 0.125f * offsetUIndex;
			//float offsetV = 0.125f * offsetVIndex;
			//m_faceMaterial.SetVector("_UVOffset", new Vector4(offsetU, offsetV, 0.0f, 0.0f));

			if (m_transform == null) return;

			//if (m_enable == false)
			//{
			//	m_rigidbody.velocity = Vector3.zero;
			//	//m_dustParticle.Stop();
			//	return;
			//}

			//if (m_moveVector == Vector3.zero)
			//{
			//	if (m_moveSpeed > 0.0f)
			//	{
			//		m_moveSpeed *= 0.9f;
			//		if (m_moveSpeed < 0.01f)
			//		{
			//			m_moveSpeed = 0.0f;
			//		}
			//		m_rigidbody.velocity *= m_moveSpeed;
			//	}
			//}
			//else
			//{
			//	float velocityY = 0.0f;
			//	m_moveSpeed += 0.05f;
			//	if (m_moveSpeed > 1.0f)
			//	{
			//		m_moveSpeed = 1.0f;
			//	}
			//	Vector3 moveVector = m_moveVector * m_moveSpeed * m_moveSpeedMax;
			//	m_rigidbody.velocity = new Vector3(moveVector.x, velocityY, moveVector.z);
			//}

			//float magnitude = m_rigidbody.velocity.magnitude;
			//if (magnitude > 10.0f &&
			//	m_dustParticle.isStopped == true)
			//{
			//	m_dustParticle.Play();
			//}
			//if (magnitude <= 10.0f &&
			//	m_dustParticle.isPlaying == true)
			//{
			//	m_dustParticle.Stop();
			//}
		}

		private IEnumerator UpdateCoroutine()
		{
			while (true)
			{
				if (m_enable == false)
				{
					yield return null;
					continue;
				}

				//if (m_moveVector == Vector3.zero)
				//{
				//	yield return null;
				//	continue;
				//}

				//var look = Quaternion.LookRotation(m_moveVector, Vector3.up);
				//Quaternion setRotate = Quaternion.Lerp(m_transform.rotation, look, 0.25f);
				//Quaternion setRotate = m_transform.rotation * Quaternion.AngleAxis(2.0f, Vector3.up);

				//Vector3 setPosition = m_transform.position;
				//Ray ray = new Ray(setPosition + Vector3.up * 5.0f, Vector3.down);
				//var hits = Physics.RaycastAll(ray, 10.0f);
				//if (hits.Length > 0)
				//{
				//	for (int i = 0; i < hits.Length; ++i)
				//	{
				//		if (hits[i].collider.tag == "IgnoreRaycast")
				//		{
				//			continue;
				//		}
				//		setPosition = hits[i].point;
				//		break;
				//	}
				//}
				//m_transform.SetPositionAndRotation(setPosition, setRotate);

				yield return null;
			}
		}
	}
}
