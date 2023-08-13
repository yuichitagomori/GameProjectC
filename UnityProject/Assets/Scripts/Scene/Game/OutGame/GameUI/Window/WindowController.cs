using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.outgame.window
{
	public class WindowController : MonoBehaviour
	{
		private enum Type
		{
			Main,
			Resource,
			Message,
		}

		[SerializeField]
		private List<WindowBase> m_windows = null;



		private Type m_activeWindowType = Type.Main;

		private List<KeyCode> m_moveButtonPressKeys = new List<KeyCode>();
		private Vector2 m_moveVector = Vector2.zero;



		public void Initialize(
			UnityAction<ingame.world.ActionTargetBase.Category, int> charaActionButtonEvent,
			UnityAction<Vector2> cameraBeginMoveEvent,
			UnityAction<Vector2> cameraMoveEvent,
			UnityAction cameraEndMoveEvent,
			UnityAction<Vector2> charaBeginMoveEvent,
			UnityAction<Vector2> charaMoveEvent,
			UnityAction charaEndMoveEvent,
			UnityAction<float> cameraZoomEvent)
		{

			if (GeneralRoot.Instance.IsPCPlatform() == true)
			{
				// PC設定
				var input = GeneralRoot.Instance.Input;
				input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.E, () =>
				{
					OnWindowChangeRightButtonPressed();
				});
				input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.Q, () =>
				{
					OnWindowChangeLeftButtonPressed();
				});
				input.UpdateEvent(system.InputSystem.Type.Press, KeyCode.UpArrow, () =>
				{
					OnWindowMoveButtonPressed(KeyCode.UpArrow);
				});
				input.UpdateEvent(system.InputSystem.Type.Press, KeyCode.DownArrow, () =>
				{
					OnWindowMoveButtonPressed(KeyCode.DownArrow);
				});
				input.UpdateEvent(system.InputSystem.Type.Press, KeyCode.LeftArrow, () =>
				{
					OnWindowMoveButtonPressed(KeyCode.LeftArrow);
				});
				input.UpdateEvent(system.InputSystem.Type.Press, KeyCode.RightArrow, () =>
				{
					OnWindowMoveButtonPressed(KeyCode.RightArrow);
				});
				input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.UpArrow, () =>
				{
					OnWindowMoveButtonReleased(KeyCode.UpArrow);
				});
				input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.DownArrow, () =>
				{
					OnWindowMoveButtonReleased(KeyCode.DownArrow);
				});
				input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.LeftArrow, () =>
				{
					OnWindowMoveButtonReleased(KeyCode.LeftArrow);
				});
				input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.RightArrow, () =>
				{
					OnWindowMoveButtonReleased(KeyCode.RightArrow);
				});
			}

			for (int i = 0; i < m_windows.Count; ++i)
			{
				if (i == (int)Type.Main)
				{
					((MainWindow)m_windows[i]).Initialize(
						charaActionButtonEvent: charaActionButtonEvent,
						cameraBeginMoveEvent: cameraBeginMoveEvent,
						cameraMoveEvent: cameraMoveEvent,
						cameraEndMoveEvent: cameraEndMoveEvent,
						charaBeginMoveEvent: charaBeginMoveEvent,
						charaMoveEvent: charaMoveEvent,
						charaEndMoveEvent: charaEndMoveEvent,
						cameraZoomEvent: cameraZoomEvent,
						holdCallback: () =>
						{
							ActiveWindow(Type.Main);
						});
				}
				else if (i == (int)Type.Resource)
				{
					((ResourceWindow)m_windows[i]).Initialize(
						holdCallback: () =>
						{
							ActiveWindow(Type.Resource);
						});
				}
				else if (i == (int)Type.Message)
				{
					((MessageWindow)m_windows[i]).Initialize(
						holdCallback: () =>
						{
							ActiveWindow(Type.Message);
						});
				}
			}
		}

		public void Go()
		{
			StartCoroutine(GoCoroutine());
		}

		private IEnumerator GoCoroutine()
		{
			for (int i = 0; i < m_windows.Count; ++i)
			{
				m_windows[i].Go();
				m_windows[i].SetSelect(false, m_windows.Count);
			}

			while (true)
			{
				if (m_moveButtonPressKeys.Count > 0)
				{
					Vector2 addVector = Vector2.zero;
					for (int i = 0; i < m_moveButtonPressKeys.Count; ++i)
					{
						switch (m_moveButtonPressKeys[i])
						{
							case KeyCode.UpArrow:
								{
									addVector += Vector2.up;
									break;
								}
							case KeyCode.DownArrow:
								{
									addVector += Vector2.down;
									break;
								}
							case KeyCode.LeftArrow:
								{
									addVector += Vector2.left;
									break;
								}
							case KeyCode.RightArrow:
								{
									addVector += Vector2.right;
									break;
								}
						}
					}

					m_moveVector += addVector.normalized * 2.0f;
					if (m_moveVector.magnitude > 10.0f)
					{
						m_moveVector = m_moveVector.normalized * 10.0f;
					}
				}
				else
				{
					m_moveVector *= 0.8f;
					if (m_moveVector.magnitude < 0.1f)
					{
						m_moveVector = Vector2.zero;
					}
				}
				if (m_moveVector != Vector2.zero)
				{
					m_windows[(int)m_activeWindowType].Move(m_moveVector);
				}
				yield return null;
			}
		}

		private void OnWindowChangeRightButtonPressed()
		{
			if (m_windows.Count(d => d.IsActiveWindow == true) <= 1)
			{
				return;
			}

			Type nextWindowType = m_activeWindowType;
			while (true)
			{
				nextWindowType++;
				if ((int)nextWindowType >= m_windows.Count)
				{
					nextWindowType = 0;
				}
				if (m_windows[(int)nextWindowType].IsActiveWindow == true)
				{
					break;
				}
			}
			ActiveWindow(nextWindowType);
		}

		private void OnWindowChangeLeftButtonPressed()
		{
			if (m_windows.Count(d => d.IsActiveWindow == true) <= 1)
			{
				return;
			}

			Type nextWindowType = m_activeWindowType;
			while (true)
			{
				nextWindowType--;
				if ((int)nextWindowType < 0)
				{
					nextWindowType = (Type)(m_windows.Count - 1);
				}
				if (m_windows[(int)nextWindowType].IsActiveWindow == true)
				{
					break;
				}
			}
			ActiveWindow(nextWindowType);
		}

		private void ActiveWindow(Type windowType)
		{
			StartCoroutine(ActiveWindowCoroutine(windowType));
		}

		private IEnumerator ActiveWindowCoroutine(Type windowType)
		{
			for (int i = 0; i < m_windows.Count; ++i)
			{
				if (m_windows[i].IsActiveWindow == false)
				{
					if (i == (int)windowType)
					{
						yield return m_windows[i].AddWindow();
					}
					else
					{
						continue;
					}
				}
				m_windows[i].SetSelect(i == (int)windowType, m_windows.Count);
			}
			m_activeWindowType = windowType;
		}

		private void OnWindowMoveButtonPressed(KeyCode key)
		{
			if (m_moveButtonPressKeys.Contains(key) == false)
			{
				m_moveButtonPressKeys.Add(key);
			}
		}

		private void OnWindowMoveButtonReleased(KeyCode key)
		{
			if (m_moveButtonPressKeys.Contains(key) == true)
			{
				m_moveButtonPressKeys.Remove(key);
			}
		}

		public void OnMovieStart(string[] paramStrings, UnityAction callback)
		{
			StartCoroutine(OnMovieStartCoroutine(paramStrings, callback));
		}

		private IEnumerator OnMovieStartCoroutine(string[] paramStrings, UnityAction callback)
		{
			switch (paramStrings[0])
			{
				case "ActiveMainWindow":
					{
						yield return ActiveWindowCoroutine(Type.Main);
						break;
					}
				case "ActiveResourceWindow":
					{
						yield return ActiveWindowCoroutine(Type.Resource);
						break;
					}
				case "ActiveMessageWindow":
					{
						yield return ActiveWindowCoroutine(Type.Message);
						break;
					}
				case "AddMessage":
					{
						yield return ActiveWindowCoroutine(Type.Message);

						bool isMine = bool.Parse(paramStrings[1]);
						int messageId = int.Parse(paramStrings[2]);
						((MessageWindow)m_windows[(int)Type.Message]).AddMessage(isMine, messageId);

						break;
					}
				case "AddResource":
					{
						yield return ActiveWindowCoroutine(Type.Resource);

						int npcId = int.Parse(paramStrings[1]);
						int colorId = int.Parse(paramStrings[2]);
						int npcCount = int.Parse(paramStrings[3]);
						((ResourceWindow)m_windows[(int)Type.Resource]).AddResource(
							npcId,
							colorId,
							npcCount);

						break;
					}
				case "":
					{
						break;
					}
			}

			if (callback != null)
			{
				callback();
			}
		}

        public void UpdateMainWindow(
            window.MainWindow.CharaActionButtonData actionButtonData,
            float weightParam)
        {
            ((window.MainWindow)m_windows[0]).UpdateCharaActionButton(actionButtonData);
            ((window.MainWindow)m_windows[0]).UpdateWeightParam(weightParam);
        }
    }
}
