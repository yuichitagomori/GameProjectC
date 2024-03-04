using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene.game.outgame.window
{
    [System.Serializable]
    public class CameraWindow : WindowBase
    {
		[System.Serializable]
		public class Data
		{
			[SerializeField]
			private int m_controllId;
			public int ControllId => m_controllId;

			[SerializeField]
			private GameObject m_charaContentsPrefab;
			public GameObject CharaContentsPrefab => m_charaContentsPrefab;
		}

		[SerializeField]
		private Transform m_cameraContentsParent;

		[SerializeField]
		private Data[] m_cameraContentsDatas;



		private camera.CameraContentsController m_controller;

		public override void OnMovieStart(string[] paramStrings, UnityAction callback)
		{
			StartCoroutine(OnMovieStartCoroutine(paramStrings, callback));
		}

		private IEnumerator OnMovieStartCoroutine(string[] paramStrings, UnityAction callback)
		{
			switch (paramStrings[0])
			{
				case "Prefab":
					{
						int index = int.Parse(paramStrings[1]);
						var data = m_cameraContentsDatas[index];
						var contents = GameObject.Instantiate(data.CharaContentsPrefab);
						contents.transform.SetParent(m_cameraContentsParent);
						contents.transform.localPosition = Vector3.zero;
						contents.transform.localScale = Vector3.one;
						m_controller = contents.GetComponent<camera.CameraContentsController>();
						break;
					}
				case "AnimationName":
					{
						string animationName = paramStrings[1];
						bool isLoop = bool.Parse(paramStrings[2]);
						if (isLoop)
						{
							m_controller.PlayLoop(animationName);
						}
						else
						{
							bool isDone = false;
							m_controller.Play(animationName, () => { isDone = true; });
							while (!isDone) { yield return null; }
						}
						break;
					}
				case "TopSibling":
					{
						SetTopSibling();
						break;
					}
			}

			if (callback != null)
			{
				callback();
			}
		}
	}
}