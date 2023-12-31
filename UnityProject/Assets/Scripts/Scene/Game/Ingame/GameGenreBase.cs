using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace scene.game.ingame
{
	[System.Serializable]
	public abstract class GameGenreBase : SceneBase
	{
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
		private Material[] m_mapMaterials;

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



		protected State m_state;

		protected UnityAction<string> m_changeGameEvent;

		protected UnityAction<string, UnityAction> m_outgameSetupEvent;


		public abstract void Initialize();

		public void Setting(
			State state,
			UnityAction<string> changeGameEvent,
			UnityAction<string, UnityAction> outgameSetupEvent)
		{
			m_state = state;
			m_changeGameEvent = changeGameEvent;
			m_outgameSetupEvent = outgameSetupEvent;
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
			RenderSettings.ambientLight = m_ambient;
			RenderSettings.fog = true;
			RenderSettings.fogColor = m_fogColor;

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

		private void SetSequenceTime(float value)
		{
			m_skyboxMaterial.SetFloat("_SequenceTime", value);
			for (int i = 0; i < m_mapMaterials.Length; ++i)
			{
				m_mapMaterials[i].SetFloat("_SequenceTime", value);
			}
		}
	}
}
