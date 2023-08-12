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
        private Handler m_hander;

		[SerializeField]
		private CharaActionButtonElement m_charaActionButton;

		[SerializeField]
		private UnityEngine.UI.Slider m_cameraZoomSlider;

		[SerializeField]
		private UnityEngine.UI.Image m_weightGaugeImage;

		[SerializeField]
		private CommonUI.TextExpansion m_weightParamText;



		private List<KeyCode> m_downKeyList = new List<KeyCode>();

		private CharaActionButtonData m_charaActionButtonData = null;

		private UnityAction<ingame.world.ActionTargetBase.Category, int> m_charaActionButtonEvent = null;

		private Coroutine m_updateCharaActionButtonCoroutine = null;

		private UnityAction<Vector2> m_cameraBeginMoveEvent = null;
		private UnityAction<Vector2> m_cameraMoveEvent = null;
		private UnityAction m_cameraEndMoveEvent = null;

		private UnityAction<Vector2> m_charaBeginMoveEvent = null;
		private UnityAction<Vector2> m_charaMoveEvent = null;
		private UnityAction m_charaEndMoveEvent = null;

		private Vector2 m_beginPosition = Vector2.zero;

		private float m_weightParam;


		public void Initialize(
			UnityAction<ingame.world.ActionTargetBase.Category, int> charaActionButtonEvent,
			UnityAction<Vector2> cameraBeginMoveEvent,
			UnityAction<Vector2> cameraMoveEvent,
			UnityAction cameraEndMoveEvent,
			UnityAction<Vector2> charaBeginMoveEvent,
			UnityAction<Vector2> charaMoveEvent,
			UnityAction charaEndMoveEvent,
			UnityAction<float> cameraZoomEvent,
			UnityAction holdCallback)
		{

			m_cameraBeginMoveEvent = cameraBeginMoveEvent;
			m_cameraMoveEvent = cameraMoveEvent;
			m_cameraEndMoveEvent = cameraEndMoveEvent;

			m_charaBeginMoveEvent = charaBeginMoveEvent;
			m_charaMoveEvent = charaMoveEvent;
			m_charaEndMoveEvent = charaEndMoveEvent;

			m_hander.Initialize(new Handler.EventData(
				beginDragEvent: CameraBeginDragEvent,
				dragEvent: CameraDragEvent,
				endDragEvent: CameraEndDragEvent,
				clickEvent: null));

			m_charaActionButton.Initialize(OnCharaActionButtonPressed);
			m_charaActionButton.Anime.Play("Default");
			m_charaActionButtonEvent = charaActionButtonEvent;

			m_cameraZoomSlider.onValueChanged.AddListener(cameraZoomEvent);
			m_cameraZoomSlider.value = 0.5f;

			if (GeneralRoot.Instance.IsPCPlatform() == true)
			{
				// PCÝ’è
				var input = GeneralRoot.Instance.Input;
				input.UpdateEvent(system.InputSystem.Type.Down, KeyCode.W, () =>
				{
					if (m_downKeyList.Contains(KeyCode.W) == false)
					{
						m_downKeyList.Add(KeyCode.W);
					}
				});
				input.UpdateEvent(system.InputSystem.Type.Down, KeyCode.S, () =>
				{
					if (m_downKeyList.Contains(KeyCode.S) == false)
					{
						m_downKeyList.Add(KeyCode.S);
					}
				});
				input.UpdateEvent(system.InputSystem.Type.Down, KeyCode.A, () =>
				{
					if (m_downKeyList.Contains(KeyCode.A) == false)
					{
						m_downKeyList.Add(KeyCode.A);
					}
				});
				input.UpdateEvent(system.InputSystem.Type.Down, KeyCode.D, () =>
				{
					if (m_downKeyList.Contains(KeyCode.D) == false)
					{
						m_downKeyList.Add(KeyCode.D);
					}
				});
				input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.W, () =>
				{
					if (m_downKeyList.Contains(KeyCode.W) == true)
					{
						m_downKeyList.Remove(KeyCode.W);
					}
				});
				input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.S, () =>
				{
					if (m_downKeyList.Contains(KeyCode.S) == true)
					{
						m_downKeyList.Remove(KeyCode.S);
					}
				});
				input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.A, () =>
				{
					if (m_downKeyList.Contains(KeyCode.A) == true)
					{
						m_downKeyList.Remove(KeyCode.A);
					}
				});
				input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.D, () =>
				{
					if (m_downKeyList.Contains(KeyCode.D) == true)
					{
						m_downKeyList.Remove(KeyCode.D);
					}
				});
			}

			base.Initialize(holdCallback);
		}

		public override void Go()
		{
			StartCoroutine(GoCoroutine());
		}

		private IEnumerator GoCoroutine()
		{
			bool isBegin = false;
			while (true)
			{
				// PCÝ’è
				if (GeneralRoot.Instance.IsPCPlatform() == true)
				{
					if (m_isActiveWindow == false)
					{
						if (isBegin == true)
						{
							isBegin = false;
							m_charaEndMoveEvent();
						}
						yield return null;
						continue;
					}

					if (m_downKeyList.Count > 0)
					{
						if (isBegin == false)
						{
							isBegin = true;
							m_charaBeginMoveEvent(Vector2.zero);
						}
						Vector2 moveVector = Vector2.zero;
						if (m_downKeyList.Contains(KeyCode.W) == true)
						{
							moveVector += Vector2.up;
						}
						if (m_downKeyList.Contains(KeyCode.S) == true)
						{
							moveVector += Vector2.down;
						}
						if (m_downKeyList.Contains(KeyCode.A) == true)
						{
							moveVector += Vector2.left;
						}
						if (m_downKeyList.Contains(KeyCode.D) == true)
						{
							moveVector += Vector2.right;
						}
						m_charaMoveEvent(moveVector);
					}
					else
					{
						if (isBegin == true)
						{
							isBegin = false;
							m_charaEndMoveEvent();
						}
					}
				}
				
				yield return null;
			}
		}

		private void CameraBeginDragEvent(Vector2 position)
		{
			if (m_isActiveWindow == false)
			{
				return;
			}
			m_cameraBeginMoveEvent(position);
		}

		private void CameraDragEvent(Vector2 position)
		{
			if (m_isActiveWindow == false)
			{
				return;
			}
			m_cameraMoveEvent(position);
		}

		private void CameraEndDragEvent()
		{
			if (m_isActiveWindow == false)
			{
				return;
			}
			m_cameraEndMoveEvent();
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
					m_charaActionButtonData.Category != data.Category ||
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
				   m_charaActionButtonData.Category != data.Category ||
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
				m_charaActionButton.Anime.Play("Out", () => { isDone = true; });
				while (!isDone) { yield return null; }
			}

			m_charaActionButtonData = data;
			yield return null;

			if (isIn)
			{
				m_charaActionButton.Setup(m_charaActionButtonData.Type);
				bool isDone = false;
				m_charaActionButton.Anime.Play("In", () => { isDone = true; });
				while (!isDone) { yield return null; }
			}
		}

		private void OnCharaActionButtonPressed()
		{
			if (m_charaActionButtonData == null)
			{
				return;
			}
			m_charaActionButtonEvent(
				m_charaActionButtonData.Category,
				m_charaActionButtonData.ControllId);
		}

		private void UpdateWeightView(float value)
		{
			m_weightParam = value;
		}
	}
}
