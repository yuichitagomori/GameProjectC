using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.actiongame
{
	[System.Serializable]
	public class ActionGamePlayerChara : MonoBehaviour
	{
		[SerializeField]
		private Rigidbody m_rigidbody;

		[SerializeField]
		private FBXBase m_fbx;

		[SerializeField]
		private Material[] m_materials;

		[SerializeField]
		private Transform m_faceChangerTransform;

		[SerializeField]
		private ParticleSystem m_moveDustParticle;

		[SerializeField]
		private float m_movePower;

		[SerializeField]
		private float m_speedMax;

		[SerializeField]
		private float m_jumpPower;

		[SerializeField]
		private float m_gravity;



		private Transform m_transform = null;
		public new Transform transform => m_transform;

		[SerializeField]
		private bool m_isJump;


		public void Initialize()
		{
			m_transform = base.transform;
			m_isJump = false;

			m_fbx.Anime.PlayLoop("Wait");

			SetSequenceTime(0);

			StartCoroutine(UpdateCoroutine());
		}

		private IEnumerator UpdateCoroutine()
		{
			var wait = new WaitForFixedUpdate();
			var faceMaterial = m_materials[1];

			while (true)
			{
				// 重力を自前で対応
				m_rigidbody.velocity += new Vector3(0.0f, -m_gravity, 0.0f);

				var direction = new Vector3(m_rigidbody.velocity.x, 0.0f, m_rigidbody.velocity.z);
				var power = direction.magnitude;
				if (power >= 1.0f)
				{
					if (m_fbx.Anime.GetAnimationName() != "Walk" && m_isJump == false)
					{
						m_fbx.Anime.PlayLoop("Walk");
					}
					if (m_moveDustParticle.isStopped == true && m_isJump == false)
					{
						m_moveDustParticle.Play();
					}
				}
				else
				{
					if (m_fbx.Anime.GetAnimationName() != "Wait" && m_isJump == false)
					{
						m_fbx.Anime.PlayLoop("Wait");
					}
					if (m_moveDustParticle.isPlaying == true)
					{
						m_moveDustParticle.Stop();
					}
				}

				Ray ray = new Ray(m_transform.position + Vector3.up * 10.0f, Vector3.down);
				var hits = Physics.RaycastAll(ray, 20.0f);
				if (hits.Length > 0)
				{
					for (int i = 0; i < hits.Length; ++i)
					{
						if (hits[i].collider.tag == "IgnoreRaycast")
						{
							continue;
						}
						if (hits[i].point.y >= m_transform.position.y - 0.01f)
						{
							// 着地しているか埋まっているので、着地させジャンプ状態解除
							m_transform.position = hits[i].point;
							//m_rigidbody.velocity = new Vector3(m_rigidbody.velocity.x, 0.0f, m_rigidbody.velocity.z);
							m_isJump = false;
							break;
						}
					}
				}

				// enable = false 状態でも表情切り替えを行いたいので、手前で更新処理をはさむ
				int offsetUIndex = Mathf.RoundToInt(m_faceChangerTransform.localPosition.x * 100);
				int offsetVIndex = Mathf.RoundToInt(m_faceChangerTransform.localPosition.y * 100);
				float offsetU = 0.125f * offsetUIndex;
				float offsetV = 0.125f * offsetVIndex;
				faceMaterial.SetVector("_UVOffset", new Vector4(offsetU, offsetV, 0.0f, 0.0f));

				yield return wait;
			}
		}

		public void SequenceIn(float sequenceTime, UnityAction callback)
		{
			StartCoroutine(SequenceInCoroutine(sequenceTime, callback));
		}

		private IEnumerator SequenceInCoroutine(float sequenceTime, UnityAction callback)
		{
			float time = 0.0f;
			while (time < sequenceTime)
			{
				float value = (time / sequenceTime);
				SetSequenceTime(value);
				time += Time.deltaTime;

				yield return null;
			}
			SetSequenceTime(1.0f);

			if (callback != null)
			{
				callback();
			}
		}

		public void SequenceOut(float sequenceTime, UnityAction callback)
		{
			StartCoroutine(SequenceOutCoroutine(sequenceTime, callback));
		}

		private IEnumerator SequenceOutCoroutine(float sequenceTime, UnityAction callback)
		{
			float time = 0.0f;
			while (time < sequenceTime)
			{
				float value = 1.0f - (time / sequenceTime);
				SetSequenceTime(value);
				time += Time.deltaTime;

				yield return null;
			}
			SetSequenceTime(0.0f);

			if (callback != null)
			{
				callback();
			}
		}

		private void SetSequenceTime(float value)
		{
			for (int i = 0; i < m_materials.Length; ++i)
			{
				m_materials[i].SetFloat("_SequenceTime", value);
			}
		}

		public void Move(Vector3 direction)
		{
			float movePower = (m_isJump) ? m_movePower * 0.5f : m_movePower;
			Vector3 add = direction * movePower;
			m_rigidbody.velocity += add;
			Vector3 move = new Vector3(m_rigidbody.velocity.x, 0.0f, m_rigidbody.velocity.z);
			if (move.magnitude > m_speedMax)
			{
				move = move.normalized * m_speedMax;
				m_rigidbody.velocity = new Vector3(move.x, m_rigidbody.velocity.y, move.z);
			}

			var look = Quaternion.LookRotation(direction.normalized, Vector3.up);
			m_transform.rotation = Quaternion.Lerp(m_transform.rotation, look, 0.2f);
		}

		public void Jump()
		{
			if (m_isJump == true)
			{
				return;
			}

			Vector3 add = Vector3.up * m_jumpPower;
			m_rigidbody.velocity += add;
			if (m_fbx.Anime.GetAnimationName() != "Jump")
			{
				m_fbx.Anime.Play("Jump");
			}
			if (m_moveDustParticle.isPlaying == true)
			{
				m_moveDustParticle.Stop();
			}
			m_isJump = true;
		}
	}
}
