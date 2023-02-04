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
		private Collider m_collider = null;

		[SerializeField]
		private Rigidbody m_rigidbody = null;

		[SerializeField]
		private FBXBase m_fbx = null;

		[SerializeField]
		private Material m_faceMaterial = null;

		[SerializeField]
		private Texture[] m_faceTextureList = null;



		private Transform m_transform = null;
		public new Transform transform => m_transform;

		private bool m_enable = false;
		public bool Enable
		{
			set
			{
				//m_collider.enabled = value;
				m_enable = value;
			}
		}

		private Transform m_cameraTransform = null;

		private Vector3 m_moveVector = Vector3.zero;



		public void Initialize()
		{
			m_fbx.Anime.PlayLoop("Wait");
			m_transform = base.transform;

			var camera = GeneralRoot.Instance.GetIngame2Camera();
			m_cameraTransform = camera.transform;

			StartCoroutine(UpdateCoroutine());
		}

		public void DragEvent(Vector2 dragVector)
		{
			Vector3 euler = new Vector3(0.0f, m_cameraTransform.eulerAngles.y, 0.0f);
			m_moveVector = Quaternion.Euler(euler) * new Vector3(dragVector.x, 0.0f, dragVector.y);

			if (m_fbx.Anime.GetAnimationName() == "Wait")
			{
				m_fbx.Anime.PlayLoop("Walk");
			}
		}

		public void EndDragEvent()
		{
			m_moveVector = Vector3.zero;

			if (m_fbx.Anime.GetAnimationName() == "Walk")
			{
				m_fbx.Anime.PlayLoop("Wait");
			};
		}

		public void PlayReaction(UnityAction callback)
		{
			StartCoroutine(PlayReactionCoroutine(callback));
		}

		private IEnumerator PlayReactionCoroutine(UnityAction callback)
		{
			float nowTime = 0.0f;
			float maxTime = 0.5f;
			Quaternion beforeRotate = m_transform.rotation;
			Vector3 lookVector = m_cameraTransform.position - m_transform.position;
			Quaternion afterRotate = Quaternion.LookRotation(lookVector, Vector3.up);
			while (nowTime < maxTime)
			{
				nowTime += Time.deltaTime;

				float t = nowTime / maxTime;
				Quaternion setRotate = Quaternion.Lerp(beforeRotate, afterRotate, t);
				Vector3 setPosition = m_transform.position;
				m_transform.SetPositionAndRotation(setPosition, setRotate);

				yield return null;
			}

			bool isDone = false;
			m_fbx.Anime.Play("ReactionDelight", () => { isDone = true; });
			while (!isDone) { yield return null; }

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
				m_rigidbody.velocity = Vector3.zero;
			}
			else
			{
				float velocityY = -6.0f;
				Vector3 moveVector = m_moveVector.normalized * 15.0f;
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
				//Ray ray = new Ray(setPosition + Vector3.up, Vector3.down);
				//RaycastHit hit;
				//if (Physics.Raycast(ray, out hit, 2.0f))
				//{
				//	setPosition = hit.point;
				//}
				m_transform.SetPositionAndRotation(setPosition, setRotate);

				yield return null;
			}
		}
	}
}
