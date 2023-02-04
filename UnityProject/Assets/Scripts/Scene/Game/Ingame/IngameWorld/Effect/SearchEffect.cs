using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.world.effect
{
    public class SearchEffect : EffectBase
    {
        [SerializeField]
        private AnimatorExpansion m_anime = null;

        public override void Initialize(
            UnityAction effectEvent,
            UnityAction callback)
		{
            StartCoroutine(PlayAction(callback));
		}

        private IEnumerator PlayAction(UnityAction callback)
		{
            bool isDone = false;
            m_anime.Play("In", () => { isDone = true; });
            while (!isDone) { yield return null; }

            m_anime.PlayLoop("Loop");
            yield return new WaitForSeconds(1.0f);

            isDone = false;
            m_anime.Play("Out", () => { isDone = true; });
            while (!isDone) { yield return null; }

            if (callback != null)
            {
                callback();
            }
        }
    }
}