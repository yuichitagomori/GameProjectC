using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.world.effect
{
    public class DeleteEffect : PlayEffectBase
    {
        [SerializeField]
        private GameObject m_object;



        public override void Play(UnityAction callback)
		{
            StartCoroutine(PlayCoroutine(callback));
        }

        private IEnumerator PlayCoroutine(UnityAction callback)
		{
            m_object.SetActive(true);

            while (true)
			{
                if (m_object.activeInHierarchy == false)
				{
                    break;
				}
                yield return null;
			}

            if (callback != null)
            {
                callback();
            }
        }
    }
}