using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

namespace scene.game.outgame
{
	[System.Serializable]
	public class GameUI : MonoBehaviour
	{
		public class CharaActionButtonData
		{
			private int m_enemyId;
			public int EnemyId => m_enemyId;

			private int m_controllId;
			public int ControllId => m_controllId;

			private Transform m_target;
			public Transform Target => m_target;

			private outgame.CharaActionButtonElement.ActionType m_type;
			public outgame.CharaActionButtonElement.ActionType Type => m_type;

			public CharaActionButtonData(
				int enemyId,
				int controllId,
				Transform target,
				outgame.CharaActionButtonElement.ActionType type)
			{
				m_enemyId = enemyId;
				m_controllId = controllId;
				m_target = target;
				m_type = type;
			}
		}

		public struct SearchTargetIconColor
		{
			public int m_enemyId;
			public data.resource.EnemyColorResource.Data.ColorData m_colorData;
		}

		[System.Serializable]
		public class SearchTargetIconTexture
		{
			public int m_enemyId;
			public Sprite m_iconSprite;
			public Texture m_ruleTexture1;
			public Texture m_ruleTexture2;
			public Texture m_disableRuleTexture;
		}

		[SerializeField]
		private CanvasGroup m_canvasGroup = null;

		[SerializeField]
		private GameObject m_mainObject = null;

		[SerializeField]
		private GameObject m_appObject = null;

		[Header("Main")]

		[SerializeField]
		private outgame.Handler m_cameraHandler = null;

		[SerializeField]
		private outgame.Handler m_playerHandler = null;

		[SerializeField]
		private GameObject m_playerHandlerActiveObject = null;

		[SerializeField]
		private Text m_mapNameText = null;

		[SerializeField]
		private AnimatorExpansion m_mapNameAnime = null;

		[SerializeField]
		private List<SearchTargetIconTexture> m_searchTargetIconTextureList = null;

		[SerializeField]
		private AnimatorExpansion m_searchFrameAnime = null;

		[SerializeField]
		private Image m_searchTargetImage = null;

		[SerializeField]
		private Image[] m_searchFrameImages = null;

		[SerializeField]
		private Sprite[] m_searchFrameOnSprites = null;

		[SerializeField]
		private Sprite[] m_searchFrameOffSprites = null;



		[SerializeField]
		private RectTransform m_charaActionButtonRect = null;

		[SerializeField]
		private outgame.CharaActionButtonElement m_charaActionButtonElement = null;

		[SerializeField]
		private RectTransform m_convertUIPositionHelperRect;

		[Header("App")]

		[SerializeField]
		private List<CommonUI.ButtonExpansion> m_appIconButtonList = null;



		private outgame.Handler.EventData m_cameraHandlerEventData = null;

		private outgame.Handler.EventData m_playerHandlerEventData = null;

		private CharaActionButtonData m_charaActionButtonData = null;

		private UnityAction<int> m_onCharaActionButtonEvent = null;

		private Coroutine m_updateCharaActionButtonCoroutine = null;


		public void Initialize(
			outgame.Handler.EventData cameraHandlerEventData,
			outgame.Handler.EventData playerHandlerEventData,
			Camera ingameCamera,
			UnityAction<int> onCharaActionButtonEvent)
		{
			m_cameraHandlerEventData = cameraHandlerEventData;
			m_cameraHandler.Initialize(new outgame.Handler.EventData(
				OnCameraHandlerDrag,
				OnCameraHandlerEndDrag,
				OnCameraHandlerClick));
			m_playerHandlerEventData = playerHandlerEventData;
			m_playerHandler.Initialize(new outgame.Handler.EventData(
				OnPlayerHandlerDrag,
				OnPlayerHandlerEndDrag,
				OnPlayerHandlerClick));
			m_playerHandlerActiveObject.SetActive(false);

			m_mainObject.SetActive(true);
			m_appObject.SetActive(false);

			m_charaActionButtonElement.Initialize(OnCharaActionButtonPressed);
			m_charaActionButtonElement.Anime.Play("Default");

			m_onCharaActionButtonEvent = onCharaActionButtonEvent;

			//for (int i = 0; i < m_appIconButtonList.Count; ++i)
			//{
			//	int index = i;
			//	m_appIconButtonList[i].SetupClickEvent(() =>
			//	{
			//		onAppIconButtonEvent(index);
			//	});
			//}

			StartCoroutine(UpdateCoroutine(ingameCamera));
		}

		private IEnumerator UpdateCoroutine(Camera ingameCamera)
		{
			var charaActionButtonElementsTransform = m_charaActionButtonElement.GetComponent<RectTransform>();
			Vector2 convertUIPositionPar = new Vector2(
				m_convertUIPositionHelperRect.rect.width / ingameCamera.pixelWidth,
				m_convertUIPositionHelperRect.rect.height / ingameCamera.pixelHeight);
			m_convertUIPositionHelperRect.anchorMin = Vector2.zero;
			m_convertUIPositionHelperRect.anchorMax = Vector2.zero;

			while (true)
			{
				if (m_charaActionButtonData == null)
				{
					yield return null;
					continue;
				}

				var targetPosition = m_charaActionButtonData.Target.position;
				var screenPos = ingameCamera.WorldToScreenPoint(targetPosition);
				Vector2 positionInImage = new Vector2(
					screenPos.x * convertUIPositionPar.x,
					screenPos.y * convertUIPositionPar.y);
				m_convertUIPositionHelperRect.anchoredPosition = positionInImage;
				Vector2 buttonPos = m_convertUIPositionHelperRect.position;
				charaActionButtonElementsTransform.position = buttonPos;
				yield return null;
			}
		}

		private void OnCameraHandlerDrag(Vector2 v)
		{
			if (m_cameraHandlerEventData == null || m_cameraHandlerEventData.DragEvent == null)
			{
				return;
			}

			m_cameraHandlerEventData.DragEvent(v);
		}

		private void OnCameraHandlerEndDrag()
		{
			if (m_cameraHandlerEventData == null || m_cameraHandlerEventData.EndDragEvent == null)
			{
				return;
			}

			m_cameraHandlerEventData.EndDragEvent();
		}

		private void OnCameraHandlerClick(Vector2 v)
		{
			if (m_cameraHandlerEventData == null || m_cameraHandlerEventData.ClickEvent == null)
			{
				return;
			}

			m_cameraHandlerEventData.ClickEvent(v);
		}

		private void OnPlayerHandlerDrag(Vector2 v)
		{
			if (m_playerHandlerEventData == null || m_playerHandlerEventData.DragEvent == null)
			{
				return;
			}

			m_playerHandlerEventData.DragEvent(v);

			m_playerHandlerActiveObject.SetActive(true);
			float eulerZ = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg - 90.0f;
			m_playerHandlerActiveObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, eulerZ);
		}

		private void OnPlayerHandlerEndDrag()
		{
			if (m_playerHandlerEventData == null || m_playerHandlerEventData.EndDragEvent == null)
			{
				return;
			}

			m_playerHandlerEventData.EndDragEvent();

			m_playerHandlerActiveObject.SetActive(false);
		}

		private void OnPlayerHandlerClick(Vector2 v)
		{
			if (m_playerHandlerEventData == null || m_playerHandlerEventData.ClickEvent == null)
			{
				return;
			}

			m_playerHandlerEventData.ClickEvent(v);
		}

		private IEnumerator UpdateModeNoneCoroutine()
		{
			m_mainObject.SetActive(true);
			m_appObject.SetActive(false);

			bool isDone = false;
			m_mapNameAnime.Play("In", () => { isDone = true; });
			while (!isDone) { yield return null; }
		}

		private IEnumerator UpdateModeAppCoroutine()
		{
			m_mainObject.SetActive(false);
			m_appObject.SetActive(true);

			m_mapNameText.text = "";

			bool isDone = false;
			m_mapNameAnime.Play("In", () => { isDone = true; });
			while (!isDone) { yield return null; }
		}

		public void SetVisible(bool value)
		{
			if (value == true)
			{
				m_canvasGroup.alpha = 1.0f;
			}
			else
			{
				m_canvasGroup.alpha = 0.0f;
			}
		}

		public void UpdateMapName(string mapName)
		{
			//m_mapNameText.text = string.Format("MapId:{0}", m_mapId);
		}

		public void UpdateSearchTarget(SearchTargetIconColor colorData, UnityAction callback)
		{
			StartCoroutine(UpdateSearchTargetCoroutine(colorData, callback));
		}

		private IEnumerator UpdateSearchTargetCoroutine(SearchTargetIconColor colorData, UnityAction callback)
		{
			if (colorData.m_enemyId < 0)
			{
				m_searchTargetImage.gameObject.SetActive(false);
				m_searchFrameImages[0].sprite = m_searchFrameOffSprites[0];
				m_searchFrameImages[1].sprite = m_searchFrameOffSprites[1];
				m_searchFrameAnime.Stop();
				if (callback != null)
				{
					callback();
				}
				yield break;
			}

			m_searchFrameImages[0].sprite = m_searchFrameOnSprites[0];
			m_searchFrameImages[1].sprite = m_searchFrameOnSprites[1];

			var textureData = m_searchTargetIconTextureList.Find(d => d.m_enemyId == colorData.m_enemyId);
			if (textureData == null)
			{
				if (callback != null)
				{
					callback();
				}
				yield break;
			}
			m_searchTargetImage.gameObject.SetActive(true);
			m_searchTargetImage.sprite = textureData.m_iconSprite;
			var material = m_searchTargetImage.material;
			material.SetTexture("_ColorRule1", textureData.m_ruleTexture1);
			material.SetColor("_Color1", colorData.m_colorData.Colors1);
			material.SetTexture("_ColorRule2", textureData.m_ruleTexture2);
			material.SetColor("_Color2", colorData.m_colorData.Colors2);
			material.SetTexture("_DisableRule", textureData.m_disableRuleTexture);

			bool isDone = false;
			m_searchFrameAnime.Play("LoopStart", () => { isDone = true; });
			while (!isDone) { yield return null; }
			m_searchFrameAnime.PlayLoop("Loop");

			if (callback != null)
			{
				callback();
			}
		}

		public void UpdateCharaActionButton(CharaActionButtonData data)
		{
			if (m_updateCharaActionButtonCoroutine != null)
			{
				StopCoroutine(m_updateCharaActionButtonCoroutine);
			}
			m_updateCharaActionButtonCoroutine = StartCoroutine(UpdateCharaActionButtonCoroutine(data));
		}

		private IEnumerator UpdateCharaActionButtonCoroutine(CharaActionButtonData data)
		{
			if (m_charaActionButtonData == null && data == null)
			{
				yield break;
			}

			bool isOut = false;
			bool isIn = false;
			if (data != null)
			{
				if (m_charaActionButtonData == null)
				{
					isIn = true;
				}
				else if (
					m_charaActionButtonData.EnemyId != data.EnemyId ||
					m_charaActionButtonData.ControllId != data.ControllId)
				{
					isOut = true;
					isIn = true;
				}
			}
			else if (m_charaActionButtonData != null)
			{
				if (data == null)
				{
					isOut = true;
				}
				else if (
				   m_charaActionButtonData.EnemyId != data.EnemyId ||
				   m_charaActionButtonData.ControllId != data.ControllId)
				{
					isOut = true;
					isIn = true;
				}
			}

			m_charaActionButtonData = null;
			yield return null;

			if (isOut)
			{
				bool isDone = false;
				m_charaActionButtonElement.Anime.Play("Out", () => { isDone = true; });
				while (!isDone) { yield return null; }
			}

			m_charaActionButtonData = data;
			yield return null;

			if (isIn)
			{
				m_charaActionButtonElement.Setup(m_charaActionButtonData.Type);
				bool isDone = false;
				m_charaActionButtonElement.Anime.Play("In", () => { isDone = true; });
				while (!isDone) { yield return null; }
			}
		}

		private void OnCharaActionButtonPressed()
		{
			if (m_charaActionButtonData == null)
			{
				return;
			}
			m_onCharaActionButtonEvent(m_charaActionButtonData.ControllId);
		}
	}
}
