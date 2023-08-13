using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Common
{
	[System.Serializable]
	public class LODEvent : MonoBehaviour
	{
		[System.Serializable]
		public class Data
		{
			[SerializeField]
			private int m_controllID = 0;
			public int ControllID => m_controllID;

			[SerializeField]
			private float m_cameraDistance = 0.0f;	// 昇順で設定する必要がある
			public float CameraDistance => m_cameraDistance;

			[SerializeField]
			private UnityAction m_eventAction = null;
			public UnityAction EventAction => m_eventAction;

			public Data(int controllID, float cameraDistance, UnityAction eventAction)
			{
				m_controllID = controllID;
				m_cameraDistance = cameraDistance;
				m_eventAction = eventAction;
			}
		}

		[SerializeField]
		private int m_nowControllID = -1;

		[SerializeField]
		private Data[] m_datas = null;



		private Transform m_transform = null;
		private Transform m_cameraTransform = null;

		public void Initialize(
			Data[] datas,
			Transform cameraTransform,
			float checkFrameSecond)
		{
			m_transform = base.transform;
			m_datas = datas;
			m_cameraTransform = cameraTransform;
			StartCoroutine(UpdateCoroutine(checkFrameSecond));
		}

		private IEnumerator UpdateCoroutine(float checkFrameSecond)
		{
			var wait = new WaitForSeconds(checkFrameSecond);

			while (true)
			{
				Vector3 dis = m_cameraTransform.position - m_transform.position;
				var data = GetData(dis.magnitude);
				if (m_nowControllID != data.ControllID)
				{
					m_nowControllID = data.ControllID;
					data.EventAction();
				}
				yield return wait;
			}
		}

		private Data GetData(float magnitude)
		{
			if (m_datas.Length <= 1)
			{
				return m_datas[0];
			}
			for (int i = 0; i < m_datas.Length - 1; ++i)
			{
				if (m_datas[i].CameraDistance <= magnitude &&
					m_datas[i + 1].CameraDistance > magnitude)
				{
					return m_datas[i];
				}
			}
			return m_datas[m_datas.Length - 1];
		}
	}
}