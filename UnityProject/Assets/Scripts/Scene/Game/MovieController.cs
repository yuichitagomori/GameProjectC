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
		[System.Serializable]
		public class MovieData
		{
			[SerializeField]
			private int m_controllId;
			public int ControllId => m_controllId;

			[SerializeField]
			private string[] m_paramStrings;
			public string[] ParamStrings => m_paramStrings;
		}

		[SerializeField]
		private MovieData[] m_movieDatas;


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
			var data = m_movieDatas.FirstOrDefault(d => d.ControllId == controllId);
			if (data == null)
			{
				if (callback != null)
				{
					callback();
				}
				yield break;
			}

			bool isDone = false;
			for (int i = 0; i < data.ParamStrings.Length; ++i)
			{
				isDone = false;
				string param = data.ParamStrings[i];
				Debug.Log("MovieController param = " + param);
				m_playMovieEvent(param, () => { isDone = true; });
				while (!isDone) { yield return null; }
			}

			if (callback != null)
			{
				callback();
			}
		}
	}
}
