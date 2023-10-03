using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.outgame.window
{
    [System.Serializable]
    public class MainWindow : WindowBase
	{
		public class CharaActionButtonData
		{
			private ingame.world.ActionTargetBase.Category m_category;
			public ingame.world.ActionTargetBase.Category Category => m_category;

			private int m_controllId;
			public int ControllId => m_controllId;

			private CharaActionButtonElement.ActionType m_type;
			public CharaActionButtonElement.ActionType Type => m_type;

			public CharaActionButtonData(
				ingame.world.ActionTargetBase.Category category,
				int controllId,
				CharaActionButtonElement.ActionType type)
			{
				m_category = category;
				m_controllId = controllId;
				m_type = type;
			}
		}

		[SerializeField]
		private Common.AnimatorExpansion m_sequenceAnimation;



		private List<KeyCode> m_downKeyList = new List<KeyCode>();

		private CharaActionButtonData m_charaActionButtonData;

		private UnityAction<ingame.world.ActionTargetBase.Category, int> m_charaActionButtonEvent;

		private Coroutine m_updateCharaActionButtonCoroutine;

		private UnityAction<Vector2> m_cameraBeginMoveEvent;
		private UnityAction<Vector2> m_cameraMoveEvent;
		private UnityAction m_cameraEndMoveEvent;

		private UnityAction<KeyCode[]> m_inputEvent;

		private UnityAction<Vector2> m_clickEvent;

		private Vector2 m_beginPosition = Vector2.zero;


		public void Initialize(
			//UnityAction<ingame.world.ActionTargetBase.Category, int> charaActionButtonEvent,
			//UnityAction<Vector2> cameraBeginMoveEvent,
			//UnityAction<Vector2> cameraMoveEvent,
			//UnityAction cameraEndMoveEvent,
			UnityAction<KeyCode[]> inputEvent,
			//UnityAction<Vector2> clickEvent,
			//UnityAction<float> cameraZoomEvent,
			RectTransform windowArea,
			UnityAction holdCallback)
		{
			//m_cameraBeginMoveEvent = cameraBeginMoveEvent;
			//m_cameraMoveEvent = cameraMoveEvent;
			//m_cameraEndMoveEvent = cameraEndMoveEvent;

			m_inputEvent = inputEvent;

			//m_clickEvent = clickEvent;

			//m_hander.Initialize(new Handler.EventData(
			//	beginDragEvent: CameraBeginDragEvent,
			//	dragEvent: CameraDragEvent,
			//	endDragEvent: CameraEndDragEvent,
			//	clickEvent: ClickEvent));

			//m_charaActionButton.Initialize(OnCharaActionButtonPressed);
			//m_charaActionButton.Anime.Play("Default");
			//m_charaActionButtonEvent = charaActionButtonEvent;

			//m_cameraZoomSlider.onValueChanged.AddListener(cameraZoomEvent);
			//m_cameraZoomSlider.value = 0.5f;

			//m_wightParamView.Initialize();

			base.Initialize(windowArea, holdCallback);
		}

		public override void Go()
		{
			StartCoroutine(GoCoroutine());
		}

		private IEnumerator GoCoroutine()
		{
			while (true)
			{
				// PC設定
				if (GeneralRoot.Instance.IsPCPlatform() == true)
				{
					if (m_isActiveWindow == false || m_isSelectWindow == false)
					{
						yield return null;
						continue;
					}

					m_inputEvent(m_downKeyList.ToArray());
				}
				
				yield return null;
			}
		}

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

			var input = GeneralRoot.Instance.Input;
			UnityAction<KeyCode> setupKey = (key) =>
			{
				input.UpdateEvent(system.InputSystem.Type.Down, key, () =>
				{
					if (m_downKeyList.Contains(key) == false)
					{
						m_downKeyList.Add(key);
					}
				});
				input.UpdateEvent(system.InputSystem.Type.Up, key, () =>
				{
					if (m_downKeyList.Contains(key) == true)
					{
						m_downKeyList.Remove(key);
					}
				});
			};
			setupKey(KeyCode.W);
			setupKey(KeyCode.S);
			setupKey(KeyCode.A);
			setupKey(KeyCode.D);
			setupKey(KeyCode.Space);
			setupKey(KeyCode.Return);

			m_downKeyList.Clear();
		}

		private IEnumerator SetupEventCoroutine(string[] paramStrings, UnityAction callback)
		{
			switch (paramStrings[0])
			{
				case "SequenceAnime":
					{
						string animationName = paramStrings[1];
						bool isDone = false;
						m_sequenceAnimation.Play(animationName, () => { isDone = true; });
						while (!isDone) { yield return null; }

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

		//private void CameraBeginDragEvent(Vector2 position)
		//{
		//	if (m_isActiveWindow == false || m_isSelectWindow == false)
		//	{
		//		return;
		//	}
		//	m_cameraBeginMoveEvent(position);
		//}

		//private void CameraDragEvent(Vector2 position)
		//{
		//	if (m_isActiveWindow == false || m_isSelectWindow == false)
		//	{
		//		return;
		//	}
		//	m_cameraMoveEvent(position);
		//}

		//private void CameraEndDragEvent()
		//{
		//	if (m_isActiveWindow == false || m_isSelectWindow == false)
		//	{
		//		return;
		//	}
		//	m_cameraEndMoveEvent();
		//}

		//private void ClickEvent(Vector2 position)
		//{
		//	if (m_isActiveWindow == false || m_isSelectWindow == false)
		//	{
		//		return;
		//	}

		//	// レンダリング画像サイズに座標を修正
		//	// (RenderTextureを使用している場合はViewport座標に変換して使用する必要がある)
		//	position.x -= 1920.0f * 0.5f;
		//	position.y -= 1080.0f * 0.5f;
		//	position.x *= (1280.0f / 900.0f);
		//	position.y *= (720.0f / 506.25f);
		//	position.x += 1920.0f * 0.5f;
		//	position.y += 1080.0f * 0.5f;
		//	position.x -= m_windowTransform.localPosition.x * (1920.0f / 900.0f);
		//	position.y -= m_windowTransform.localPosition.y * (1080.0f / 506.25f);
		//	m_clickEvent(new Vector2(position.x / 1920.0f, position.y / 1080.0f));
		//}

        public void UpdateWeightParam(float value)
		{
			//m_wightParamView.UpdateWeightParam(value);
		}
    }
}
