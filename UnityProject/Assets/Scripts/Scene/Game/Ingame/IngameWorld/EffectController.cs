using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.world
{
	[System.Serializable]
	public class EffectController : MonoBehaviour
	{
		public enum PlayEffectType
		{
			Freeze,
		}

		public enum LoopEffectType
		{
			Restaint,
		}

		[SerializeField]
		private GameObject[] m_originalPlayEffectObjectList;

		[SerializeField]
		private GameObject[] m_originalLoopEffectObjectList;



		private GameObject m_effectObject = null;



		public void Initialize()
		{
			for (int i = 0; i < m_originalPlayEffectObjectList.Length; ++i)
			{
				m_originalPlayEffectObjectList[i].SetActive(false);
			}
			for (int i = 0; i < m_originalLoopEffectObjectList.Length; ++i)
			{
				m_originalLoopEffectObjectList[i].SetActive(false);
			}
		}

		public void Play(PlayEffectType playEffectType, Vector3 position, UnityAction callback)
		{
			var originalObject = m_originalPlayEffectObjectList[(int)playEffectType];
			m_effectObject = GameObject.Instantiate(originalObject, transform);
			m_effectObject.SetActive(true);
			m_effectObject.transform.position = position;
			effect.PlayEffectBase effectBase = m_effectObject.GetComponent<effect.PlayEffectBase>();
			effectBase.Play(() =>
			{
				GameObject.Destroy(m_effectObject);
				if (callback != null)
				{
					callback();
				}
			});
		}

		public UnityAction PlayLoop(
			LoopEffectType loopEffectType,
			Vector3 position)
		{
			var originalObject = m_originalLoopEffectObjectList[(int)loopEffectType];
			m_effectObject = GameObject.Instantiate(originalObject, transform);
			m_effectObject.SetActive(true);
			m_effectObject.transform.position = position;
			effect.LoopEffectBase effectBase = m_effectObject.GetComponent<effect.LoopEffectBase>();
			effectBase.PlayIn(null);
			return () =>
			{
				effectBase.PlayOut(() =>
				{
					GameObject.Destroy(m_effectObject);
				});
			};
		}
	}
}