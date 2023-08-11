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

			public enum ColliderFilter
			{
				Player,
				SearchIn,
				SearchOut,
			}

			[SerializeField]
			private Type m_eventType = Type.Enter;
			public Type EventType => m_eventType;

			[SerializeField]
			private string m_eventParam = "";
			public string EventParam => m_eventParam;

			[SerializeField]
			private ColliderFilter[] m_filterTypes;
			public ColliderFilter[] FilterTypes => m_filterTypes;

			[SerializeField]
			private bool m_invalidation = true;
			public bool Invalidation => m_invalidation;

			private Data() { }
			public Data(
				Type eventType,
				string eventParam,
				ColliderFilter[] filterTypes,
				bool invalidation)
			{
				m_eventType = eventType;
				m_eventParam = eventParam;
				m_filterTypes = filterTypes;
				m_invalidation = invalidation;
			}

			public string GetFilterName(Data.ColliderFilter type)
			{
				switch (type)
				{
					case ColliderFilter.Player:
						{
							return "PlayerChara";
						}
					case ColliderFilter.SearchIn:
						{
							return "SearchIn";
						}
					case ColliderFilter.SearchOut:
						{
							return "SearchOut";
						}
					default:
						{
							return "";
						}
				}
			}
		}



		private Data[] m_datas = null;

		public UnityAction<string> m_callback;

		private Collider m_collider = null;



		public void Initialize(UnityAction<string> callback)
		{
			Initialize(null, callback);
		}

		public void Initialize(Data[] datas, UnityAction<string> callback)
		{
			m_datas = datas;
			m_callback = callback;
			m_collider = GetComponents<Collider>().FirstOrDefault(c => c.isTrigger == true);
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
			if (m_callback == null || m_datas == null)
			{
				return;
			}

			if (m_datas.Length <= 0)
			{
				return;
			}

			var checkDatas = m_datas.Where(d => d.EventType == eventType).ToArray();

			for (int i = 0; i < checkDatas.Length; ++i)
			{
				var data = checkDatas[i];
				if (string.IsNullOrEmpty(data.EventParam) == true)
				{
					continue;
				}
				if (data.FilterTypes.Length <= 0)
				{
					return;
				}
				for (int j = 0; j < data.FilterTypes.Length; ++j)
				{
					string filterName = data.GetFilterName(data.FilterTypes[j]);
					if (string.IsNullOrEmpty(filterName) == true)
					{
						continue;
					}
					if (string.Equals(otherName, filterName) == false)
					{
						continue;
					}

					if (data.Invalidation == true)
					{
						m_collider.enabled = false;
					}

					m_callback(data.EventParam);
					continue;
				}
			}
		}
	}
}