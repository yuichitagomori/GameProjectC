using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

namespace scene.game.ingame
{
	public class StageScene : SceneBase
	{
		[System.Serializable]
		public class PositionData
		{
			public Vector3 m_position = Vector3.zero;
			public Vector3 m_euler = Vector3.zero;
		}

		[System.Serializable]
		public class WorldData
		{
			[SerializeField]
			private Material m_skyboxMaterial = null;
			//public Material SkyboxMaterial => m_skyboxMaterial;

			[SerializeField]
			private Texture[] m_skyboxTextures = null;
			public Texture[] SkyboxTextures => m_skyboxTextures;

			[SerializeField]
			private float m_skyboxTextureChangeTime = 0.0f;
			public float SkyboxTextureChangeTime => m_skyboxTextureChangeTime;

			[SerializeField]
			private Light m_directionLight = null;

			[SerializeField]
			private Color m_ambient = Color.white;

			[SerializeField]
			private Color m_fogColor = Color.white;

			[SerializeField]
			private world.MapCollider m_mapCollider = null;

			[SerializeField]
			private world.NavMeshController m_navMeshController = null;

			[SerializeField]
			private Material[] m_mapMaterials = null;
			//public Material[] MapMaterials => m_mapMaterials;

			[SerializeField]
			private world.NPCController m_npcController = null;

			[SerializeField]
			private world.ObjectController m_objectController = null;

			[SerializeField]
			private world.ItemBase[] m_itemList = null;

			[SerializeField]
			private EventBase[] m_eventList = null;

			[SerializeField]
			private PositionData[] m_positionDataList = null;



			public void Initialize(
				Transform ingameCameraTransform,
				UnityAction<string> eventCallback)
			{
				RenderSettings.skybox = m_skyboxMaterial;
				RenderSettings.sun = m_directionLight;  // この処理はなくても大丈夫そう
				RenderSettings.ambientLight = m_ambient;
				RenderSettings.fog = true;
				RenderSettings.fogColor = m_fogColor;

				m_npcController.Initialize(
					ingameCameraTransform,
					m_navMeshController.GetNavMeshBasePoints(),
					eventCallback);
				m_objectController.Initialize(eventCallback);

				for (int i = 0; i < m_itemList.Length; ++i)
				{
					m_itemList[i].Initialize(eventCallback);
				}
				for (int i = 0; i < m_eventList.Length; ++i)
				{
					m_eventList[i].Initialize(eventCallback);
				}
			}

			public void SetSequenceTime(float value)
			{
				m_skyboxMaterial.SetFloat("_SequenceTime", value);
				for (int i = 0; i < m_mapMaterials.Length; ++i)
				{
					m_mapMaterials[i].SetFloat("_SequenceTime", value);
				}
				m_npcController.SetSequenceTime(value);
			}

			public void SetSkyboxTexture(Texture texture)
			{
				m_skyboxMaterial.SetTexture("_Texture", texture);
			}

			public PositionData GetPositionData(int index)
			{
				return m_positionDataList[index];
			}

			public world.NPC GetNPC(int controllId)
			{
				return m_npcController.GetNPC(controllId);
			}

			public void DeleteNPC(int controllId)
			{
				m_npcController.DeleteNPC(controllId);
			}

			public world.ObjectBase GetObject(int controllId)
			{
				return m_objectController.GetObject(controllId);
			}

			public world.ItemBase GetItem(int id)
			{
				return m_itemList.Where(d => d.ID == id).First();
			}

			public void OnMovieStart(string[] paramStrings, UnityAction callback)
			{
				switch (paramStrings[0])
				{
					case "AddNPC":
						{
							int characterId = int.Parse(paramStrings[1]);
							int colorId = int.Parse(paramStrings[2]);
							int count = int.Parse(paramStrings[3]);
							m_npcController.AddNPC(characterId, colorId, count, callback);
							break;
						}
				}
			}
		}

		[SerializeField]
		private WorldData m_worldData;



		private Transform m_ingameCameraTransform;
		private UnityAction<string> m_eventCallback;

		public void Initialize(
			Transform ingameCameraTransform,
			UnityAction<string> eventCallback)
		{
			m_ingameCameraTransform = ingameCameraTransform;
			m_eventCallback = eventCallback;
		}

		public override void Ready(UnityAction callback)
		{
			m_worldData.SetSequenceTime(0.0f);
			m_worldData.SetSkyboxTexture(m_worldData.SkyboxTextures[0]);

			m_worldData.Initialize(
				m_ingameCameraTransform,
				m_eventCallback);

			callback();
		}

		public override void Go()
		{
			StartCoroutine(UpdateSkyboxCoroutine());
		}

		public IEnumerator ChangeMapOutCoroutine(float time)
		{
			var curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

			float nowTime = 0.0f;
			while (nowTime < time)
			{
				nowTime += Time.deltaTime;
				if (nowTime > time)
				{
					nowTime = time;
				}

				float t = nowTime / time;
				float value = curve.Evaluate(1.0f - t);
				m_worldData.SetSequenceTime(value);

				yield return null;
			}
			m_worldData.SetSequenceTime(0.0f);
		}

		public IEnumerator ChangeMapInCoroutine(float time)
		{
			var curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

			float nowTime = 0.0f;
			while (nowTime < time)
			{
				nowTime += Time.deltaTime;
				if (nowTime > time)
				{
					nowTime = time;
				}

				float t = nowTime / time;
				float value = curve.Evaluate(t);
				m_worldData.SetSequenceTime(value);

				yield return null;
			}
			m_worldData.SetSequenceTime(1.0f);
		}

		public PositionData GetPositionData(int index)
		{
			return m_worldData.GetPositionData(index);
		}

		public world.NPC GetNPC(int controllId)
		{
			return m_worldData.GetNPC(controllId);
		}

		public void DeleteNPC(int controllId)
		{
			m_worldData.DeleteNPC(controllId);
		}

		public world.ObjectBase GetObject(int controllId)
		{
			return m_worldData.GetObject(controllId);
		}

		public world.ItemBase GetItem(int id)
		{
			return m_worldData.GetItem(id);
		}

		private IEnumerator UpdateSkyboxCoroutine()
		{
			float changeTime = m_worldData.SkyboxTextureChangeTime;
			WaitForSeconds wait = new WaitForSeconds(changeTime);
			int index = 0;

			while (true)
			{
				m_worldData.SetSkyboxTexture(m_worldData.SkyboxTextures[index]);
				index++;
				if (index >= m_worldData.SkyboxTextures.Length)
				{
					index = 0;
				}
				yield return wait;
			}
		}

		public void OnMovieStart(string[] paramStrings, UnityAction callback)
		{
			switch (paramStrings[0])
			{
				default:
					{
						m_worldData.OnMovieStart(paramStrings, callback);
						break;
					}
			}
		}
	}
}