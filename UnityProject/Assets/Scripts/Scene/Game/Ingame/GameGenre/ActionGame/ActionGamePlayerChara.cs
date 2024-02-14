using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
		private SphereCollider m_collider;

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

		private bool m_isEnable = true;
		private bool m_isAir = false;
		private bool m_isJump = false;
		private UnityAction<int> m_bugCallback;


		public void Initialize(UnityAction<int> bugCallback)
		{
			m_transform = base.transform;
			m_isEnable = true;
			m_isAir = false;
			m_isJump = false;
			m_bugCallback = bugCallback;

			var local = GeneralRoot.User.LocalSaveData;
			if (local.OccurredBugId == (int)data.master.CheckSheetBugData.BugType.Animation)
			{
				m_fbx.Anime.Play("Jump");
			}
			else
			{
				m_bugCallback(3);
			}

			SetSequenceTime(0);

			StartCoroutine(UpdateCoroutine());
		}

		private IEnumerator UpdateCoroutine()
		{
			var wait = new WaitForFixedUpdate();
			var faceMaterial = m_materials[1];
			var local = GeneralRoot.User.LocalSaveData;

			while (true)
			{
				if (m_isEnable == false)
				{
					yield return wait;
					continue;
				}

				if (m_transform.position.y < -100.0f)
				{
					SetupEnable(false);
					yield return wait;
					continue;
				}

				//bool isAirBefore = m_isAir;
				m_isAir = true;
				Vector3 rayPosition = m_collider.transform.position + Vector3.up * 0.5f;
				Ray ray = new Ray(rayPosition, Vector3.down);
				var hits = Physics.SphereCastAll(ray, m_collider.radius - 0.01f, 1.0f);
				if (hits.Length > 0)
				{
					for (int i = 0; i < hits.Length; ++i)
					{
						if (hits[i].collider.tag == "IgnoreRaycast")
						{
							continue;
						}

						m_isAir = false;
						break;
					}
				}
				if (m_rigidbody.velocity.y < 0.0f)
				{
					// 落下中のみチェック
					if (m_isAir == false)
					{
						// 着地しているか埋まっているので、着地させジャンプ状態解除
						m_isJump = false;
					}
				}

				// 重力を自前で対応
				m_rigidbody.velocity += new Vector3(0.0f, -m_gravity, 0.0f);

				var direction = new Vector3(m_rigidbody.velocity.x, 0.0f, m_rigidbody.velocity.z);
				var power = direction.magnitude;
				if (power >= 1.0f)
				{
					if (m_fbx.Anime.GetAnimationName() != "Walk" && m_isJump == false)
					{
						if (local.OccurredBugId == (int)data.master.CheckSheetBugData.BugType.Animation)
						{
							m_fbx.Anime.PlayLoop("Walk");
						}
						else
						{
							m_bugCallback(3);
						}
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
						if (local.OccurredBugId == (int)data.master.CheckSheetBugData.BugType.Animation)
						{
							m_fbx.Anime.PlayLoop("Wait");
						}
						else
						{
							m_bugCallback(3);
						}
					}
					if (m_moveDustParticle.isPlaying == true)
					{
						m_moveDustParticle.Stop();
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

			SetupEnable(true);

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
			SetupEnable(false);

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
			if (m_isEnable == false)
			{
				return;
			}

			float movePower = (m_isAir) ? m_movePower * 0.5f : m_movePower;
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
			if (m_isEnable == false)
			{
				return;
			}

			if (m_isAir == true || m_isJump == true)
			{
				return;
			}

			Vector3 add = Vector3.up * m_jumpPower;
			m_rigidbody.velocity += add;
			if (m_fbx.Anime.GetAnimationName() != "Jump")
			{
				var local = GeneralRoot.User.LocalSaveData;
				if (local.OccurredBugId == (int)data.master.CheckSheetBugData.BugType.Animation)
				{
					m_fbx.Anime.Play("Jump");
				}
				else
				{
					m_bugCallback(3);
				}
			}
			if (m_moveDustParticle.isPlaying == true)
			{
				m_moveDustParticle.Stop();
			}
			m_isJump = true;
		}

		public void Transfer()
		{
			var local = GeneralRoot.User.LocalSaveData;
			if (local.OccurredBugId == (int)data.master.CheckSheetBugData.BugType.Animation)
			{
				m_fbx.Anime.Play("Jump", time: 1.0f);
			}
			else
			{
				m_bugCallback(3);
			}
		}

		private void SetupEnable(bool value)
		{
			if (value == true)
			{
				m_isEnable = true;
			}
			else
			{
				m_isEnable = false;
				m_rigidbody.velocity = Vector3.zero;
				if (m_moveDustParticle.isPlaying == true)
				{
					m_moveDustParticle.Stop();
				}
				if (m_fbx.Anime.GetAnimationName() != "Wait")
				{
					var local = GeneralRoot.User.LocalSaveData;
					if (local.OccurredBugId == (int)data.master.CheckSheetBugData.BugType.Animation)
					{
						m_fbx.Anime.PlayLoop("Wait");
					}
					else
					{
						m_bugCallback(3);
					}
				}
			}
		}
	}
}
