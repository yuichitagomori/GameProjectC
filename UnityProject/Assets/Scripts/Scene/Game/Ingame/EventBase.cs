using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame
{
	[System.Serializable]
	public class EventBase : MonoBehaviour
	{
		[System.Serializable]
		public class Data
		{
			public enum Type
			{
				Enter,
				Exit,
			}

			[SerializeField]
			private Type m_eventType = Type.Enter;
			public Type EventType => m_eventType;

			[SerializeField]
			private string m_targetName;
			public string TargetName => m_targetName;

			[SerializeField]
			private string[] m_eventParams;
			public string[] EventParams => m_eventParams;

			[SerializeField]
			private bool m_invalidation = true;
			public bool Invalidation => m_invalidation;

			private Data() { }
		}

		[SerializeField]
		private Data[] m_datas = null;

		[SerializeField]
		private Collider m_collider = null;



		public UnityAction<string[]> m_callback;


		public void Initialize(UnityAction<string[]> callback)
		{
			m_callback = callback;
		}

		private void OnTriggerEnter(Collider other)
		{
			ColliderEventCheck(Data.Type.Enter, other.name);
		}

		private void OnTriggerExit(Collider other)
		{
			ColliderEventCheck(Data.Type.Exit, other.name);
		}

		private void ColliderEventCheck(Data.Type eventType, string otherName)
		{
			if (m_callback == null)
			{
				return;
			}

			if (m_datas.Length <= 0)
			{
				return;
			}

			var checkDatas = m_datas
				.Where(d => d.EventType == eventType)
				.Where(d => !string.IsNullOrEmpty(d.TargetName) ? d.TargetName == otherName : true)
				.ToArray();
			for (int i = 0; i < checkDatas.Length; ++i)
			{
				var data = checkDatas[i];
				if (data.EventParams.Length <= 0)
				{
					continue;
				}
				if (data.Invalidation == true)
				{
					m_collider.enabled = false;
				}

				m_callback(data.EventParams);
			}
		}
	}
}