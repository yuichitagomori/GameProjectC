using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame
{
	[System.Serializable]
	public abstract class GameGenreBase : SceneBase
	{
		protected string SequenceAnimeStringFormat = "Outgame,PlayWindow,Main,SequenceAnime,{0}";
		protected string CameraReactionStringFormat = "Outgame,PlayWindow,Camera,AnimationName,{0},{1}";

		public enum State
		{
			None,
			Title,
			Game,
			Result,
		}



		[SerializeField]
		private Material m_skyboxMaterial;

		[SerializeField]
		private Texture[] m_skyboxTextures;

		[SerializeField]
		private float m_skyboxTextureChangeTime = 0.0f;

		[SerializeField]
		private Light m_directionLight;

		[SerializeField]
		private Color m_ambient = Color.white;

		[SerializeField]
		private Color m_fogColor = Color.white;

		[SerializeField]
		private system.PosproController.Data m_posproData;

		/// <summary>
		/// インゲーム用カメラ
		/// </summary>
		[SerializeField]
		protected Transform m_cameraTransform;

		/// <summary>
		/// インゲーム用カメラトランスフォーム
		/// </summary>
		[SerializeField]
		protected Transform m_cameraParentTransform;

		[SerializeField]
		protected Transform[] m_cameraAngles;



		private string m_sceneName;
		protected string SceneName => m_sceneName;

		private UnityAction<string, State, string> m_changeGameEvent;
		protected UnityAction<string, State, string> ChangeGameEvent => m_changeGameEvent;

		private UnityAction<string, UnityAction> m_playMovieEvent;
		protected UnityAction<string, UnityAction> PlayMovieEvent => m_playMovieEvent;



		protected State m_state;

		protected string m_initializeParam = "";



		public abstract void Initialize();

		public void Setting(
			string sceneName,
			UnityAction<string, State, string> changeGameEvent,
			UnityAction<string, UnityAction> playMovieEvent,
			State state,
			string initializeParam)
		{
			m_sceneName = sceneName;
			m_changeGameEvent = changeGameEvent;
			m_playMovieEvent = playMovieEvent;
			m_state = state;
			m_initializeParam = initializeParam;
		}

		public override void Ready(UnityAction callback)
		{
			Initialize();
			SetupEnvironment();

			callback();
		}

		private void SetupEnvironment()
		{
			RenderSettings.skybox = m_skyboxMaterial;
			RenderSettings.sun = m_directionLight;  // この処理はなくても大丈夫そう
			//RenderSettings.ambientLight = m_ambient;
			//RenderSettings.fog = (m_fogColor.a >= 1.0f);
			//RenderSettings.fogColor = m_fogColor;

			GeneralRoot.Pospro.Setting(m_posproData);

			StartCoroutine(UpdateSkyboxCoroutine());
		}

		public abstract void OnInputEvent(KeyCode[] pressKeys);

		private IEnumerator UpdateSkyboxCoroutine()
		{
			if (m_skyboxTextures.Length <= 0 || m_skyboxTextureChangeTime <= 0.0f)
			{
				yield break;
			}

			WaitForSeconds wait = new WaitForSeconds(m_skyboxTextureChangeTime);
			int index = 0;

			while (true)
			{
				m_skyboxMaterial.SetTexture("_Texture", m_skyboxTextures[index]);
				index++;
				if (index >= m_skyboxTextures.Length)
				{
					index = 0;
				}
				yield return wait;
			}
		}

		protected void SetSkyboxSequenceTime(float value)
		{
			m_directionLight.intensity = 2.0f * value;
			m_skyboxMaterial.SetFloat("_SequenceTime", value);
		}
	}
}
