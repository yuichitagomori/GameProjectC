using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.world
{
	[System.Serializable]
	public class EffectController : MonoBehaviour
	{
		public enum Type
		{
			Freeze,
			Search
		}

		[SerializeField]
		private GameObject[] m_originalEffectObjectList;


		private UnityAction m_effectEvent = null;

		private GameObject m_effectObject = null;



		public void Initialize(UnityAction effectEvent)
		{
			m_effectEvent = effectEvent;
			for (int i = 0; i < m_originalEffectObjectList.Length; ++i)
			{
				m_originalEffectObjectList[i].SetActive(false);
			}
		}

		public void Play(Type type, Vector3 position, UnityAction callback)
		{
			var originalObject = m_originalEffectObjectList[(int)type];
			m_effectObject = GameObject.Instantiate(originalObject, transform);
			m_effectObject.SetActive(true);
			m_effectObject.transform.position = position;
			effect.EffectBase effectBase = m_effectObject.GetComponent<effect.EffectBase>();
			effectBase.Initialize(m_effectEvent, () =>
			{
				GameObject.Destroy(m_effectObject);
				if (callback != null)
				{
					callback();
				}
			});
		}

	}
}