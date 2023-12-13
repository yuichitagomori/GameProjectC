using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene.game.outgame.window.chara
{
    [System.Serializable]
    public class CharaContentsController : MonoBehaviour
    {
		[SerializeField]
		private Common.AnimatorExpansion m_charaAnime;



		public void Play(string animationName, UnityAction callback)
		{
			StartCoroutine(PlayCoroutine(animationName, callback));
		}

		private IEnumerator PlayCoroutine(string animationName, UnityAction callback)
		{
			bool isDone = false;
			m_charaAnime.Play(animationName, () => { isDone = true; });
			while (!isDone) { yield return null; }

			if (callback != null)
			{
				callback();
			}
		}

		public void PlayLoop(string animationName)
		{
			m_charaAnime.PlayLoop(animationName);
		}

		public string GetAnimationName()
		{
			return m_charaAnime.GetAnimationName();
		}
	}
}