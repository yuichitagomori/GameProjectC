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

		public override void SetupEvent(string[] paramStrings, UnityAction callback)
		{
			StartCoroutine(SetupEventCoroutine(paramStrings, callback));
		}

		protected override void SetupInputKeyEvent()
		{
			if (GeneralRoot.Instance.IsPCPlatform() == false)
			{
				return;
			}

			for (int i = 0; i < k_useKeys.Length; ++i)
			{
				var key = k_useKeys[i];
				GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Down, key, null);
				GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, key, null);
			}
		}

		private IEnumerator SetupEventCoroutine(string[] paramStrings, UnityAction callback)
		{
			switch (paramStrings[0])
			{
				case "Play":
					{
						int controllId = int.Parse(paramStrings[1]);
						yield return PlayCoroutine(controllId, null);

						break;
					}
				default:
					{
						break;
					}
			}

			if (callback != null)
			{
				callback();
			}
		}

		public void Play(int controllId, UnityAction callback)
		{
			StartCoroutine(PlayCoroutine(controllId, callback));
		}

		private IEnumerator PlayCoroutine(int controllId, UnityAction callback)
		{
			var masterData = GeneralRoot.Master.CharaWindowMovieData.Find(controllId);
			if (masterData == null)
			{
				yield break;
			}

			int paramStringsIndex = 0;
			while (paramStringsIndex < masterData.ParamStrings.Length)
			{
				string[] paramStrings = masterData.ParamStrings[paramStringsIndex].Split(',');
				switch (paramStrings[0])
				{
					case "Prefab":
						{
							var data = m_cameraContentsDatas.FirstOrDefault(d => d.ControllId == controllId);
							var contentsPrefab = data.CharaContentsPrefab;
							var contents = GameObject.Instantiate(contentsPrefab);
							contents.transform.SetParent(m_cameraContentsParent);
							contents.transform.localPosition = Vector3.zero;
							contents.transform.localScale = Vector3.one;
							m_controller = contents.GetComponent<camera.CameraContentsController>();
							m_controller.Play("Default", null);
							break;
						}
					case "WindowAnimationName":
						{
							string animationType = paramStrings[1];
							string animationName = paramStrings[2];
							if (animationType == "Play")
							{
								bool isDone = false;
								m_windowAnime.Play(animationName, () => { isDone = true; });
								while (!isDone) { yield return null; }
							}
							else if (animationType == "PlayLoop")
							{
								m_windowAnime.PlayLoop(animationName);
							}
							break;
						}
					case "AnimationName":
						{
							string animationType = paramStrings[1];
							string animationName = paramStrings[2];
							if (animationType == "Play")
							{
								bool isDone = false;
								m_controller.Play(animationName, () => { isDone = true; });
								while (!isDone) { yield return null; }
							}
							else if (animationType == "PlayLoop")
							{
								m_controller.PlayLoop(animationName);
							}
							break;
						}
					case "WaitTime":
						{
							float time = float.Parse(paramStrings[1]);
							yield return new WaitForSeconds(time);
							break;
						}
				}
				paramStringsIndex++;
			}

			if (callback != null)
			{
				callback();
			}
		}
	}
}