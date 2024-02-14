using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game
{
	[System.Serializable]
	public class Ingame : MonoBehaviour
	{
		[SerializeField]
		private Material m_backgroundMaterial;

		[SerializeField]
		private Transform m_displayCharaTransform;

		[SerializeField]
		private Common.AnimatorExpansion m_displayCharaAnimation;

		[SerializeField]
		private Transform m_displayCharaFaceTransform;

		[SerializeField]
		private Material m_displayCharaFaceMaterial;

		[SerializeField]
		private Transform m_displayCharaEmotionTransform;

		[SerializeField]
		private Material m_displayCharaEmotionMaterial;





		private UnityAction<string, ingame.GameGenreBase, UnityAction<ingame.GameGenreBase>> m_loadGameEvent;

		private ingame.GameGenreBase m_gameGenre;

		private UnityAction<string, UnityAction> m_outgameSetupEvent;

		private float m_backgroundScrollSpeed;

		private float m_backgroundScrollValue;



		public void Initialize(
			UnityAction<string, ingame.GameGenreBase, UnityAction<ingame.GameGenreBase>> loadGameEvent,
			UnityAction<string, UnityAction> outgameSetupEvent)
		{
			m_loadGameEvent = loadGameEvent;
			m_outgameSetupEvent = outgameSetupEvent;

			m_backgroundMaterial.SetFloat("_Scroll", 0.0f);
			m_backgroundMaterial.SetFloat("_Distortion", 0.0f);
			m_backgroundScrollSpeed = 4.0f;
			m_backgroundScrollValue = 0.0f;
			StartCoroutine(UpdateCoroutine());

			m_displayCharaAnimation.Play("Default");
		}

		public void Go()
		{
		}

		private IEnumerator UpdateCoroutine()
		{
			var wait = new WaitForFixedUpdate();

			while (true)
			{
				m_backgroundScrollValue += Time.deltaTime * m_backgroundScrollSpeed;
				m_backgroundScrollValue = m_backgroundScrollValue % 1.0f;
				m_backgroundMaterial.SetFloat("_Scroll", m_backgroundScrollValue);

				int faceOffsetUIndex = Mathf.RoundToInt(m_displayCharaFaceTransform.localPosition.x * 100);
				int faceOffsetVIndex = Mathf.RoundToInt(m_displayCharaFaceTransform.localPosition.y * 100);
				float faceOffsetU = -0.25f * faceOffsetUIndex;
				float faceOffsetV = 0.125f * faceOffsetVIndex;
				m_displayCharaFaceMaterial.SetVector("_UVOffset", new Vector4(faceOffsetU, faceOffsetV, 0.0f, 0.0f));

				int emotionOffsetUIndex = Mathf.RoundToInt(m_displayCharaEmotionTransform.localPosition.x * 100);
				int emotionOffsetVIndex = Mathf.RoundToInt(m_displayCharaEmotionTransform.localPosition.y * 100);
				float emotionOffsetU = -0.125f * emotionOffsetUIndex;
				float emotionOffsetV = 0.125f * emotionOffsetVIndex;
				m_displayCharaEmotionMaterial.SetVector("_UVOffset", new Vector4(emotionOffsetU, emotionOffsetV, 0.0f, 0.0f));

				yield return wait;
			}
		}

		public void OnMovieStart(string[] paramStrings, UnityAction callback)
		{
			switch (paramStrings[0])
			{
				case "BackgroundFast":
					{
						float time = float.Parse(paramStrings[1]);
						SetupBackground(true, time, callback);
						break;
					}
				case "BackgroundSlow":
					{
						float time = float.Parse(paramStrings[1]);
						SetupBackground(false, time, callback);
						break;
					}
				case "StartGame":
					{
						string sceneName = paramStrings[1];
						StartGame(sceneName, ingame.GameGenreBase.State.None, callback);
						break;
					}
				case "PlayDisplayChara":
					{
						string animationName = paramStrings[1];
						bool isLoop = bool.Parse(paramStrings[2]);
						StartCoroutine(PlayDisplayCharaCoroutine(animationName, isLoop, callback));
						break;
					}
				case "MoveDisplayChara":
					{
						float x = float.Parse(paramStrings[1]);
						float y = float.Parse(paramStrings[2]);
						float z = float.Parse(paramStrings[3]);
						float rotX = float.Parse(paramStrings[4]);
						float rotY = float.Parse(paramStrings[5]);
						float rotZ = float.Parse(paramStrings[6]);
						float time = float.Parse(paramStrings[7]);
						StartCoroutine(MoveDisplayCharaCoroutine(
							new Vector3(x, y, z),
							Quaternion.Euler(rotX, rotY, rotZ),
							time,
							callback));
						break;
					}
				default:
					{
						break;
					}
			}
		}

		private void StartGame(
			string sceneName,
			ingame.GameGenreBase.State state,
			UnityAction callback)
		{
			m_loadGameEvent(sceneName, m_gameGenre, (ingame.GameGenreBase newGameGenre) =>
			{
				m_gameGenre = newGameGenre;
				m_gameGenre.Setting(
					sceneName: sceneName,
					changeGameEvent: (sceneName, state) =>
					{
						StartGame(sceneName, state, null);
					},
					outgameSetupEvent: m_outgameSetupEvent,
					state: state);
				if (callback != null)
				{
					callback();
				}
			});
		}

		public void ResetGame()
		{
			StartGame(m_gameGenre.name, ingame.GameGenreBase.State.None, null);
		}

		public void OnInputEvent(KeyCode[] pressKeys)
		{
			if (m_gameGenre == null)
			{
				return;
			}
			m_gameGenre.OnInputEvent(pressKeys);
		}
		private void SetupBackground(bool isFast, float time, UnityAction callback)
		{
			StartCoroutine(SetupBackgroundCoroutine(isFast, time, callback));
		}

		private IEnumerator SetupBackgroundCoroutine(bool isFast, float time, UnityAction callback)
		{
			float nowTime = 0.0f;
			while (nowTime < time)
			{
				nowTime += Time.deltaTime;
				if (nowTime > time)
				{
					nowTime = time;
				}
				m_backgroundScrollSpeed = (isFast) ?
					3.0f * (nowTime / time) + 1.0f :
					3.0f * (1.0f - (nowTime / time)) + 1.0f;
				float distortionValue = (isFast) ?
					0.5f * (1.0f - (nowTime / time)) :
					0.5f * (nowTime / time);
				m_backgroundMaterial.SetFloat("_Distortion", distortionValue);

				yield return null;
			}

			if (callback != null)
			{
				callback();
			}

		}

		private IEnumerator PlayDisplayCharaCoroutine(string name, bool isLoop, UnityAction callback)
		{
			if (isLoop == true)
			{
				m_displayCharaAnimation.PlayLoop(name);
			}
			else
			{
				bool isDone = false;
				m_displayCharaAnimation.Play(name, () => { isDone = true; });
				while (!isDone) { yield return null; }
			}

			if (callback != null)
			{
				callback();
			}
		}

		private IEnumerator MoveDisplayCharaCoroutine(Vector3 pos, Quaternion rot, float time, UnityAction callback)
		{
			yield return CommonMath.EaseInOutTransform(
				m_displayCharaTransform,
				m_displayCharaTransform.localPosition,
				m_displayCharaTransform.localRotation,
				pos,
				rot,
				time,
				callback);
		}
	}
}