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
		public enum ColliderFilter
		{
			Player,
			SearchIn,
			SearchOut,
		}

		[SerializeField]
		private string m_enterEventParam = "";

		[SerializeField]
		private string m_exitEventParam = "";

		[SerializeField]
		private ColliderFilter[] m_filterTypes;

		[SerializeField]
		private bool m_invalidation = true;



		private Collider m_collider = null;

		private UnityAction<string> m_enterCallback = null;

		private UnityAction<string> m_exitCallback = null;



		public void Initialize(
			string enterEventParam,
			string exitEventParam,
			UnityAction<string> enterCallback,
			UnityAction<string> exitCallback)
		{
			m_enterEventParam = enterEventParam;
			m_exitEventParam = exitEventParam;
			Initialize(enterCallback, exitCallback);
		}

		public void Initialize(
			UnityAction<string> enterCallback,
			UnityAction<string> exitCallback)
		{
			m_collider = GetComponents<Collider>().First(c => c.isTrigger == true);
			m_enterCallback = enterCallback;
			m_exitCallback = exitCallback;
		}

		private void OnTriggerEnter(Collider other)
		{
			ColliderEventCheck(other.name, m_enterEventParam, m_enterCallback);
		}

		private void OnTriggerExit(Collider other)
		{
			ColliderEventCheck(other.name, m_exitEventParam, m_exitCallback);
		}

		private void ColliderEventCheck(
			string otherName,
			string eventParam,
			UnityAction<string> callback)
		{
			if (callback == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(eventParam) == true)
			{
				return;
			}
			if (m_filterTypes.Length <= 0)
			{
				return;
			}
			for (int i = 0; i < m_filterTypes.Length; ++i)
			{
				string filterName = GetFilterName(m_filterTypes[i]);
				if (string.IsNullOrEmpty(filterName) == true)
				{
					continue;
				}
				if (string.Equals(otherName, filterName) == false)
				{
					continue;
				}

				if (m_invalidation == true)
				{
					m_collider.enabled = false;
				}

				callback(eventParam);
				return;
			}
		}

		private string GetFilterName(ColliderFilter type)
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
}