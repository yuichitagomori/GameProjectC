using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.world.effect
{
    public class RestraintEffect : LoopEffectBase
    {
        [SerializeField]
        protected FBXBase m_fbx = null;



        public override void PlayIn(UnityAction callback)
		{
            StartCoroutine(PlayInCoroutine(callback));
		}

        private IEnumerator PlayInCoroutine(UnityAction callback)
		{
            bool isDone = false;
            m_fbx.Anime.Play("In", () => { isDone = true; });
            while (!isDone) { yield return null; }

            m_fbx.Anime.PlayLoop("Loop");

            if (callback != null)
			{
                callback();
			}
		}

        public override void PlayOut(UnityAction callback)
        {
            StartCoroutine(PlayOutCoroutine(callback));
        }

        private IEnumerator PlayOutCoroutine(UnityAction callback)
        {
            bool isDone = false;
            m_fbx.Anime.Play("Out", () => { isDone = true; });
            while (!isDone) { yield return null; }

            if (callback != null)
            {
                callback();
            }
        }
    }
}