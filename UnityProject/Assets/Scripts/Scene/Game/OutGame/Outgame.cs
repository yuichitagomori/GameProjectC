using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

namespace scene.game
{
	[System.Serializable]
	public class Outgame : MonoBehaviour
	{
		public enum Target
		{
			None,
			Game,
			Movie,
		}

		[SerializeField]
		private Material m_renderPassMaterial = null;

		[SerializeField]
		private outgame.GameUI m_gameUI = null;

		[SerializeField]
		private outgame.MovieUI m_movieUI = null;

		private UniversalAdditionalCameraData m_rendererData = null;



		public void Initialize(
			outgame.Handler.EventData cameraHandlerEventData,
			outgame.Handler.EventData playerHandlerEventData,
			UnityAction<int> onCharaActionButtonEvent)
		{
			//m_renderPassMaterial.SetFloat("_FogValue", 0.0f);
			//m_renderPassMaterial.SetFloat("_NoiseValue", 1.0f);
			//m_renderPassMaterial.SetFloat("_Lerp", 1.0f);
			var camera = GeneralRoot.Instance.GetIngame2Camera();
			//m_rendererData = camera.GetComponent<UniversalAdditionalCameraData>();
			//m_rendererData.SetRenderer(1);

			m_gameUI.Initialize(
				cameraHandlerEventData,
				playerHandlerEventData,
				onCharaActionButtonEvent);
		}

		//public IEnumerator GoCoroutine()
		//{
		//	AnimationCurve curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

		//	float nowFogValue = 0.0f;
		//	float nextFogValue = 1.0f;
		//	float time = 1.0f;
		//	float nowTime = 0.0f;
		//	while (nowTime < time)
		//	{
		//		float t = nowTime / time;

		//		float setFogValue = nowFogValue + (nextFogValue - nowFogValue) * curve.Evaluate(t);
		//		m_renderPassMaterial.SetFloat("_FogValue", setFogValue);

		//		nowTime += Time.deltaTime;
		//		yield return null;
		//	}
		//	m_renderPassMaterial.SetFloat("_FogValue", nextFogValue);

		//	yield return PlayMapInCoroutine();
		//}

		public void SetVisible(Target visibleTarget)
		{
			m_gameUI.SetVisible(visibleTarget == Target.Game);
			m_movieUI.SetVisible(visibleTarget == Target.Movie);
		}

		public void UpdateSearchTarget(outgame.GameUI.SearchTargetIconColor colorData, UnityAction callback)
		{
			m_gameUI.UpdateSearchTarget(colorData, callback);
		}

		public void UpdateCharaActionButton(outgame.GameUI.CharaActionButtonData data)
		{
			m_gameUI.UpdateCharaActionButton(data);
		}

		//public IEnumerator PlayMapInCoroutine()
		//{
		//	float nowNoiseValue = 1.0f;
		//	float nextNoiseValue = 0.0f;
		//	float time = 3.0f;
		//	float nowTime = 0.0f;
		//	while (nowTime < time)
		//	{
		//		float t = nowTime / time;
				
		//		float setNoiseValue = nowNoiseValue + (nextNoiseValue - nowNoiseValue) * t;
		//		m_renderPassMaterial.SetFloat("_NoiseValue", setNoiseValue);

		//		nowTime += Time.deltaTime;
		//		yield return null;
		//	}
		//	m_renderPassMaterial.SetFloat("_NoiseValue", nextNoiseValue);

		//	m_rendererData.SetRenderer(-1);
		//}

		//public IEnumerator PlayMapOutCoroutine()
		//{
		//	m_rendererData.SetRenderer(1);

		//	float nowNoiseValue = 0.0f;
		//	float nextNoiseValue = 1.0f;
		//	float time = 2.0f;
		//	float nowTime = 0.0f;
		//	while (nowTime < time)
		//	{
		//		float t = nowTime / time;

		//		float setNoiseValue = nowNoiseValue + (nextNoiseValue - nowNoiseValue) * t;
		//		m_renderPassMaterial.SetFloat("_NoiseValue", setNoiseValue);

		//		nowTime += Time.deltaTime;
		//		yield return null;
		//	}
		//	m_renderPassMaterial.SetFloat("_NoiseValue", nextNoiseValue);
		//}
	}
}
