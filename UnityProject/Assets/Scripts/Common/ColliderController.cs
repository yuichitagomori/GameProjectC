using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderController : MonoBehaviour
{
	[SerializeField]
	private Collider m_collider;

	[SerializeField]
	private LayerMask m_mask;

	private UnityAction m_enter = null;
	private UnityAction m_stay = null;
	private UnityAction m_exit = null;


	public void Setup(
		UnityAction _enter,
		UnityAction _stay,
		UnityAction _exit)
	{
		m_enter = _enter;
		m_stay = _stay;
		m_exit = _exit;
	}

	public bool IsHitRay(Vector3 _pos, Vector3 _dir, float _length)
	{
		Ray ray = new Ray(_pos, _dir);

		//Rayを緑色の線で可視化する
		Debug.DrawRay(ray.origin, ray.direction * _length, Color.green, 5, false);

		if (Physics.Raycast(ray, _length) == true)
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// 当たり開始
	/// </summary>
	/// <param name="collision"></param>
	private void OnCollisionEnter(Collision collision)
	{
		if (m_enter != null)
		{
			m_enter();
		}
	}

	/// <summary>
	/// 当たり中
	/// </summary>
	/// <param name="collision"></param>
	private void OnCollisionStay(Collision collision)
	{
		if (m_stay != null)
		{
			m_stay();
		}
	}

	/// <summary>
	/// 当たり終了
	/// </summary>
	/// <param name="collision"></param>
	private void OnCollisionExit(Collision collision)
	{
		if (m_exit != null)
		{
			m_exit();
		}
	}

	/// <summary>
	/// 当たり開始
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerEnter(Collider other)
	{
		if (m_enter != null)
		{
			m_enter();
		}
	}

	/// <summary>
	/// 当たり中
	/// </summary>
	/// <param name="collision"></param>
	private void OnTriggerStay(Collider other)
	{
		if (m_stay != null)
		{
			m_stay();
		}
	}

	/// <summary>
	/// 当たり終了
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerExit(Collider other)
	{
		if (m_exit != null)
		{
			m_exit();
		}
	}
}
