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

		public void Play(int movieDataId, UnityAction callback)
		{
			StartCoroutine(PlayCoroutine(movieDataId, callback));
		}

		private IEnumerator PlayCoroutine(
			int movieDataId,
			UnityAction callback)
		{
			var masterData = GeneralRoot.Master.MovieListData.Find(movieDataId);
			if (masterData == null)
			{
				if (callback != null)
				{
					callback();
				}
				yield break;
			}

			bool isDone = false;
			for (int i = 0; i < masterData.Datas.Length; ++i)
			{
				isDone = false;
				m_playMovieEvent(masterData.Datas[i].Param, () => { isDone = true; });
				while (!isDone) { yield return null; }
			}

			if (callback != null)
			{
				callback();
			}
		}
	}
}
