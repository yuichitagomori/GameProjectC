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
			private world.EnemyController m_enemyController = null;

			[SerializeField]
			private world.ObjectBase[] m_objectList = null;

			[SerializeField]
			private world.ItemBase[] m_itemList = null;

			[SerializeField]
			private EventBase[] m_eventList = null;

			[SerializeField]
			private PositionData[] m_positionDataList = null;



			public IEnumerator InitializeCoroutine(
				Transform playerTransform,
				EnemyDataAsset enemyDataAsset,
				UnityAction<string> enterCallback,
				UnityAction<string> exitCallback)
			{
				RenderSettings.skybox = m_skyboxMaterial;
				RenderSettings.sun = m_directionLight;  // この処理はなくても大丈夫そう
				RenderSettings.ambientLight = m_ambient;
				RenderSettings.fog = true;
				RenderSettings.fogColor = m_fogColor;

				yield return m_enemyController.InitializeCoroutine(
					playerTransform,
					m_navMeshController.GetNavMeshBasePoints(),
					enemyDataAsset,
					enterCallback,
					exitCallback);

				for (int i = 0; i < m_objectList.Length; ++i)
				{
					m_objectList[i].Initialize(
						enterCallback,
						exitCallback);
				}
				for (int i = 0; i < m_itemList.Length; ++i)
				{
					m_itemList[i].Initialize(enterCallback);
				}
				for (int i = 0; i < m_eventList.Length; ++i)
				{
					m_eventList[i].Initialize(enterCallback, exitCallback);
				}
			}

			public void SetSequenceTime(float value)
			{
				m_skyboxMaterial.SetFloat("_SequenceTime", value);
				for (int i = 0; i < m_mapMaterials.Length; ++i)
				{
					m_mapMaterials[i].SetFloat("_SequenceTime", value);
				}
				m_enemyController.SetSequenceTime(value);
			}

			public void SetSkyboxTexture(Texture texture)
			{
				m_skyboxMaterial.SetTexture("_Texture", texture);
			}

			public PositionData GetPositionData(int index)
			{
				return m_positionDataList[index];
			}

			public world.Enemy GetEnemy(int controllId)
			{
				return m_enemyController.GetEnemy(controllId);
			}

			public world.ItemBase GetItem(int id)
			{
				return m_itemList.Where(d => d.ID == id).First();
			}
		}

		[SerializeField]
		private WorldData m_worldData = null;


		private Transform m_playerTransform = null;
		private EnemyDataAsset m_enemyAssetData = null;
		private UnityAction<string> m_enterEvent = null;
		private UnityAction<string> m_exitEvent = null;

		public override void Ready(UnityAction callback)
		{
			StartCoroutine(ReadyCoroutine(callback));
		}

		private IEnumerator ReadyCoroutine(UnityAction callback)
		{
			m_worldData.SetSequenceTime(0.0f);
			m_worldData.SetSkyboxTexture(m_worldData.SkyboxTextures[0]);

			yield return m_worldData.InitializeCoroutine(
				m_playerTransform,
				m_enemyAssetData,
				m_enterEvent,
				m_exitEvent);

			callback();
		}

		public override void Go()
		{
			StartCoroutine(UpdateSkyboxCoroutine());
		}

		public void Initialize(
			Transform playerTransform,
			EnemyDataAsset enemyAssetData,
			UnityAction<string> enterEvent,
			UnityAction<string> exitEvent)
		{
			m_playerTransform = playerTransform;
			m_enemyAssetData = enemyAssetData;
			m_enterEvent = enterEvent;
			m_exitEvent = exitEvent;
		}

		public IEnumerator ChangeMapOutCoroutine()
		{
			var curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

			float nowTime = 0.0f;
			float maxTime = 0.5f;
			while (nowTime < maxTime)
			{
				nowTime += Time.deltaTime;
				if (nowTime > maxTime)
				{
					nowTime = maxTime;
				}

				float t = nowTime / maxTime;
				float value = curve.Evaluate(1.0f - t);
				m_worldData.SetSequenceTime(value);

				yield return null;
			}
			m_worldData.SetSequenceTime(0.0f);
		}

		public IEnumerator ChangeMapInCoroutine()
		{
			var curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

			float nowTime = 0.0f;
			float maxTime = 0.5f;
			while (nowTime < maxTime)
			{
				nowTime += Time.deltaTime;
				if (nowTime > maxTime)
				{
					nowTime = maxTime;
				}

				float t = nowTime / maxTime;
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

		public world.Enemy GetEnemy(int controllId)
		{
			return m_worldData.GetEnemy(controllId);
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
	}
}