using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.world.effect
{
    public class FreezeEffect : EffectBase
    {
        [SerializeField]
        private SkinnedMeshRenderer m_mesh = null;

        public override void Initialize(
            UnityAction effectEvent,
            UnityAction callback)
		{
            m_fbx.Anime.Play("Action", callback);
            //StartCoroutine(PlayAction());
		}

        private IEnumerator PlayAction()
		{
            WaitForSeconds wait = new WaitForSeconds(0.01f);
            int index = 0;
            int indexMax = 3;
            while (true)
            {
                for (int i = 0; i < indexMax; ++i)
                {
                    float value = (i == index) ? 1.0f : 0.0f;
                    m_mesh.SetBlendShapeWeight(i, value);
                }
                index++;
                if (index >= indexMax)
				{
                    index = 0;
				}
                yield return wait;
            }
		}
    }
}