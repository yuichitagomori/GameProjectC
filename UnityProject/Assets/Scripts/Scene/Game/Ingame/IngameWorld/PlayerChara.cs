using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.world
{
	[System.Serializable]
	public class PlayerChara : MonoBehaviour
	{
		public enum ReactionType
		{
			Delight,	// 喜ぶ
			Restraint,	// 拘束
		}

		[SerializeField]
		private Rigidbody m_rigidbody = null;

		[SerializeField]
		private FBXBase m_fbx = null;

		[SerializeField]
		private Material m_faceMaterial = null;

		[SerializeField]
		private Texture[] m_faceTextureList = null;

		[SerializeField]
		private ParticleSystem m_dustParticle = null;



		private Transform m_transform = null;
		public new Transform transform => m_transform;

		private bool m_enable = true;

		private Transform m_cameraTransform = null;

		private Vector3 m_moveVector = Vector3.zero;

		private float m_moveSpeed = 0.0f;


		public void Initialize()
		{
			m_transform = base.transform;

			var camera = GeneralRoot.Instance.GetIngame2Camera();
			m_cameraTransform = camera.transform;

			m_fbx.Anime.PlayLoop("Wait");
			m_dustParticle.Stop();

			StartCoroutine(UpdateCoroutine());
		}

		public void DragEvent(Vector2 dragVector)
		{
			Vector3 euler = new Vector3(0.0f, m_cameraTransform.eulerAngles.y, 0.0f);
			m_moveVector = (Quaternion.Euler(euler) * new Vector3(dragVector.x, 0.0f, dragVector.y)).normalized;

			if (m_fbx.Anime.GetAnimationName() == "Wait")
			{
				m_fbx.Anime.PlayLoop("Walk");
				m_dustParticle.Play();
			}
		}

		public void EndDragEvent()
		{
			m_moveVector = Vector3.zero;

			if (m_fbx.Anime.GetAnimationName() == "Walk")
			{
				m_fbx.Anime.PlayLoop("Wait");
				m_dustParticle.Stop();
			};
		}

		public void SetEnable(bool value)
		{
			m_enable = value;
		}

		public void OnCharaActionButtonPressed(UnityAction callback)
		{
			m_moveVector = Vector3.zero;

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

		public void PlayReaction(ReactionType type, UnityAction callback)
		{
			StartCoroutine(PlayReactionCoroutine(type, callback));
		}

		private IEnumerator PlayReactionCoroutine(ReactionType type, UnityAction callback)
		{
			switch (type)
			{
				case ReactionType.Delight:
					{
						int doneCount = 0;
						LookTarget(m_cameraTransform.position, () => { doneCount++; });
						m_fbx.Anime.Play("ReactionDelight", () => { doneCount++; });
						while (doneCount < 3) { yield return null; }

						m_fbx.Anime.PlayLoop("Wait");

						break;
					}
				case ReactionType.Restraint:
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
			if (m_transform == null) return;

			if (m_enable == false)
			{
				m_rigidbody.velocity = Vector3.zero;
				return;
			}

			if (m_moveVector == Vector3.zero)
			{
				if (m_moveSpeed > 0.0f)
				{
					m_moveSpeed *= 0.9f;
					if (m_moveSpeed < 0.01f)
					{
						m_moveSpeed = 0.0f;
					}
					m_rigidbody.velocity *= m_moveSpeed;
				}
			}
			else
			{
				float velocityY = 0.0f;
				m_moveSpeed += 0.05f;
				if (m_moveSpeed > 1.0f)
				{
					m_moveSpeed = 1.0f;
				}
				Vector3 moveVector = m_moveVector * 20.0f * m_moveSpeed;
				m_rigidbody.velocity = new Vector3(moveVector.x, velocityY, moveVector.z);
			}
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

				if (m_moveVector == Vector3.zero)
				{
					yield return null;
					continue;
				}

				var look = Quaternion.LookRotation(m_moveVector, Vector3.up);
				Quaternion setRotate = Quaternion.Lerp(m_transform.rotation, look, 0.25f);

				Vector3 setPosition = m_transform.position;
				Ray ray = new Ray(setPosition + Vector3.up * 5.0f, Vector3.down);
				var hits = Physics.RaycastAll(ray, 10.0f);
				if (hits.Length > 0)
				{
					for (int i = 0; i < hits.Length; ++i)
					{
						if (hits[i].collider.tag == "IgnoreRaycast")
						{
							continue;
						}
						setPosition = hits[i].point;
						break;
					}
				}
				m_transform.SetPositionAndRotation(setPosition, setRotate);

				yield return null;
			}
		}
	}
}
