using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace scene
{
	/// <summary>
	/// シーン操作
	/// </summary>
	public class SceneController : MonoBehaviour
	{
		/// <summary>
		/// 遷移アニメーター
		/// </summary>
		[SerializeField]
		private AnimatorExpansion m_animator;



		/// <summary>
		/// シーンリスト
		/// </summary>
		private List<SceneBase> m_scenes = new List<SceneBase>();

		/// <summary>
		/// コルーチン
		/// </summary>
		private Coroutine m_coroutine = null;

		/// <summary>
		/// 初期化
		/// </summary>
		public void Initialize()
		{
			StartCoroutine(InitializeCoroutine());
		}

		/// <summary>
		/// 初期化コルーチン
		/// </summary>
		/// <returns></returns>
		private IEnumerator InitializeCoroutine()
		{
			yield return ChangeSceneCoroutine("Game", null, false);
			yield return RemoveSceneCoroutine("Initialize");
		}

		/// <summary>
		/// シーン切り替え
		/// </summary>
		/// <param name="added"></param>
		public void ChangeScene<T>(UnityAction<T> added = null) where T : SceneBase
		{
			ChangeScene<T>(typeof(T).Name, added);
		}

		/// <summary>
		/// シーン切り替え
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sceneName"></param>
		/// <param name="added"></param>
		public void ChangeScene<T>(string sceneName, UnityAction<T> added = null) where T : SceneBase
		{
			if (m_coroutine != null)
			{
				return;
			}

			m_coroutine = StartCoroutine(ChangeSceneCoroutine(
				sceneName: sceneName,
				added: (s) =>
				{
					if (added != null)
					{
						added((T)s);
					}
					m_coroutine = null;
				}));
		}

		/// <summary>
		/// シーン追加
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="added"></param>
		public void AddScene<T>(UnityAction<T> added = null) where T : SceneBase
		{
			AddScene<T>(typeof(T).Name, added);
		}

		/// <summary>
		/// シーン追加
		/// </summary>
		/// <param name="sceneName"></param>
		/// <param name="added"></param>
		public void AddScene<T>(
			string sceneName,
			UnityAction<T> added = null) where T : SceneBase
		{
			if (m_coroutine != null)
			{
				return;
			}

			m_coroutine = StartCoroutine(AddSceneColoutine(
				sceneName: sceneName,
				added: (s) =>
				{
					if (added != null)
					{
						added((T)s);
					}
					m_coroutine = null;
				}));
		}

		/// <summary>
		/// シーン削除
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="callback"></param>
		public void RemoveScene(SceneBase scene, UnityAction callback)
		{
			if (m_coroutine != null)
			{
				return;
			}

			m_coroutine = StartCoroutine(RemoveSceneColoutine(
				scene: scene,
				callback: () =>
				{
					m_coroutine = null;
					if (callback != null)
					{
						callback();
					}
				}));
		}

		/// <summary>
		/// シーン切り替え
		/// </summary>
		/// <param name="sceneName"></param>
		/// <param name="added"></param>
		/// <param name="isInSequenceAnimation"></param>
		/// <returns></returns>
		private IEnumerator ChangeSceneCoroutine(
			string sceneName,
			UnityAction<SceneBase> added = null,
			bool isInSequenceAnimation = true)
		{
			GeneralRoot.Instance.SetForeMostRayCast(true);

			bool isDone = false;

			if (isInSequenceAnimation == true)
			{
				//GeneralRoot.Instance.SoundSystem.PlaySE((int)SoundSystem.SEType.CLOSE_SCENE);
				//isDone = false;
				//m_animator.Play("In", () => { isDone = true; });
				//while (!isDone) { yield return null; }
			}

			SceneBase newScene = null;
			yield return LoadSceneColoutine(sceneName, (SceneBase _newScene) => { newScene = _newScene; });
			while (newScene == null) { yield return null; }

			SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

			// シーンを全削除
			for (int i = 0; i < m_scenes.Count; ++i)
			{
				isDone = false;
				m_scenes[i].Finish(() => { isDone = true; });
				while (!isDone) { yield return null; }
				yield return RemoveSceneCoroutine(m_scenes[i].name);
			}
			m_scenes.Clear();
			m_scenes.Add(newScene);

			if (added != null)
			{
				added(newScene);
			}
			
			newScene.Initialize(this);

			isDone = false;
			newScene.Ready(() => { isDone = true; });
			while (!isDone) { yield return null; }

			// Ready完了後、表示状態に
			if (newScene.TopCanvasGroup != null)
			{
				newScene.TopCanvasGroup.alpha = 1.0f;
				newScene.TopCanvasGroup.blocksRaycasts = true;
			}

			//GeneralRoot.Instance.SoundSystem.PlaySE((int)SoundSystem.SEType.OPEN_SCENE);
			//isDone = false;
			//m_animator.Play("Out", () => { isDone = true; });
			//while (!isDone) { yield return null; }

			GeneralRoot.Instance.SetForeMostRayCast(false);

			newScene.Go();
		}

		/// <summary>
		/// シーン追加
		/// </summary>
		/// <param name="sceneName"></param>
		/// <param name="added"></param>
		private IEnumerator AddSceneColoutine(
			string sceneName,
			UnityAction<SceneBase> added)
		{
			GeneralRoot.Instance.SetForeMostRayCast(true);

			bool isDone = false;

			SceneBase newScene = null;
			yield return LoadSceneColoutine(sceneName, (SceneBase s) => { newScene = s; });
			while (newScene == null) { yield return null; }

			m_scenes.Add(newScene);

			if (added != null)
			{
				added(newScene);
			}
			
			newScene.Initialize(this);

			isDone = false;
			newScene.Ready(() => { isDone = true; });
			while (!isDone) { yield return null; }

			// Ready完了後、表示状態に
			if (newScene.TopCanvasGroup != null)
			{
				newScene.TopCanvasGroup.alpha = 1.0f;
				newScene.TopCanvasGroup.blocksRaycasts = true;
			}

			GeneralRoot.Instance.SetForeMostRayCast(false);

			newScene.Go();
		}

		/// <summary>
		/// シーン削除
		/// </summary>
		/// <param name="scene"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		private IEnumerator RemoveSceneColoutine(SceneBase scene, UnityAction callback)
		{
			bool isDone = false;
			scene.Finish(() => { isDone = true; });
			while (!isDone) { yield return null; }

			m_scenes.Remove(scene);

			string sceneName = scene.SceneName;
			yield return RemoveSceneCoroutine(sceneName);

			if (callback != null)
			{
				callback();
			}
		}

		/// <summary>
		/// シーン追加
		/// </summary>
		/// <param name="sceneName"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		private IEnumerator LoadSceneColoutine(string sceneName, UnityAction<SceneBase> callback)
		{
			var async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

			while (!async.isDone)
			{
				yield return null;
			}

			Scene scene = SceneManager.GetSceneByName(sceneName);
			GameObject sceneRootObject = scene.GetRootGameObjects()[0];
			SceneBase sceneBase = sceneRootObject.GetComponent<SceneBase>();

			if (sceneBase.TopCanvasGroup != null)
			{
				sceneBase.TopCanvasGroup.alpha = 0.0f;
				sceneBase.TopCanvasGroup.blocksRaycasts = false;
			}

			if (callback != null)
			{
				callback(sceneBase);
			}
		}

		/// <summary>
		/// シーン削除
		/// </summary>
		/// <param name="sceneName"></param>
		/// <returns></returns>
		private IEnumerator RemoveSceneCoroutine(string sceneName)
		{
			yield return SceneManager.UnloadSceneAsync(sceneName);
		}
	}
}