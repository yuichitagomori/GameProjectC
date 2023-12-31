using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace scene.game
{
	public class MovieController : MonoBehaviour
	{
		private UnityAction<string, UnityAction> m_playMovieEvent;



		public void Initialize(UnityAction<string, UnityAction> playMovieEvent)
		{
			m_playMovieEvent = playMovieEvent;
		}

		public void Play(int controllId, UnityAction callback)
		{
			StartCoroutine(PlayCoroutine(controllId, callback));
		}

		private IEnumerator PlayCoroutine(
			int controllId,
			UnityAction callback)
		{
			var masterData = GeneralRoot.Master.MovieData.Find(controllId);
			if (masterData == null)
			{
				if (callback != null)
				{
					callback();
				}
				yield break;
			}

			if (masterData.DisableInput == true)
			{
				GeneralRoot.Instance.SetForeMostRayCast(true);
			}

			bool isDone = false;
			for (int i = 0; i < masterData.ParamStrings.Length; ++i)
			{
				isDone = false;
				string param = masterData.ParamStrings[i];
				m_playMovieEvent(param, () => { isDone = true; });
				while (!isDone) { yield return null; }
			}

			if (masterData.DisableInput == true)
			{
				GeneralRoot.Instance.SetForeMostRayCast(false);
			}

			if (callback != null)
			{
				callback();
			}
		}
	}
}
