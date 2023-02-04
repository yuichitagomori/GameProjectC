using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.world
{
	[System.Serializable]
	public class ItemBase : MonoBehaviour
	{
		[SerializeField]
		private int m_id = -1;
		public int ID => m_id;

		[SerializeField]
		private FBXBase m_fbx = null;

		[SerializeField]
		private EventBase m_event = null;



		private Transform m_transform = null;
		public new Transform transform => m_transform;
		
		private UnityAction<string> m_eventCallback = null;


		public void Initialize(UnityAction<string> _eventCallback)
		{
			gameObject.SetActive(false);
			m_transform = base.transform;
			
			m_eventCallback = _eventCallback;
		}

		public IEnumerator SetActiveColoutine(bool _value)
		{
			if (_value == true)
			{
				gameObject.SetActive(true);
				bool isDone = false;
				m_fbx.Anime.Play("In", () => { isDone = true; });
				while (!isDone) { yield return null; }
				m_fbx.Anime.PlayLoop("Wait");

				m_event.Initialize(m_eventCallback, null);
			}
			else
			{
				bool isDone = false;
				m_fbx.Anime.Play("Out", () => { isDone = true; });
				while (!isDone) { yield return null; }
				gameObject.SetActive(false);
			}
		}
	}
}
