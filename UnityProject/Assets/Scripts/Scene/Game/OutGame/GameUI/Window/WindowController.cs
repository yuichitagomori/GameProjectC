using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.outgame.window
{
	public class WindowController : MonoBehaviour
	{
		[SerializeField]
		private List<GameObject> m_windowPrefabList;

		[SerializeField]
		private Transform m_windowParent;



		private List<WindowBase> m_windows = new List<WindowBase>();

		private int m_selectWindowIndex = -1;

		private UnityAction<WindowBase.Type[], int> m_updateWindowIcon;

		private List<KeyCode> m_moveButtonPressKeys = new List<KeyCode>();
		private Vector2 m_moveVector = Vector2.zero;

		//private UnityAction<ingame.world.ActionTargetBase.Category, int> m_charaActionButtonEvent;
		//private UnityAction<Vector2> m_cameraBeginMoveEvent;
		//private UnityAction<Vector2> m_cameraMoveEvent;
		//private UnityAction m_cameraEndMoveEvent;
		private UnityAction<KeyCode[]> m_inputEvent;
		//private UnityAction<Vector2> m_clickEvent;
		//private UnityAction<float> m_cameraZoomEvent;

		public void Initialize(
			UnityAction<KeyCode[]> inputEvent,
			UnityAction<WindowBase.Type[], int> updateWindowIcon)
		{
			//m_charaActionButtonEvent = charaActionButtonEvent;
			//m_cameraBeginMoveEvent = cameraBeginMoveEvent;
			//m_cameraMoveEvent = cameraMoveEvent;
			//m_cameraEndMoveEvent = cameraEndMoveEvent;
			m_inputEvent = inputEvent;
			//m_clickEvent = clickEvent;
			//m_cameraZoomEvent = cameraZoomEvent;

			m_updateWindowIcon = updateWindowIcon;



			if (GeneralRoot.Instance.IsPCPlatform() == false)
			{
				return;
			}

			var input = GeneralRoot.Instance.Input;
			input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.E, () =>
			{
				OnWindowChangeRightButtonPressed();
			});
			input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.Q, () =>
			{
				OnWindowChangeLeftButtonPressed();
			});
			input.UpdateEvent(system.InputSystem.Type.Down, KeyCode.UpArrow, () =>
			{
				OnWindowMoveButtonPressed(KeyCode.UpArrow);
			});
			input.UpdateEvent(system.InputSystem.Type.Down, KeyCode.DownArrow, () =>
			{
				OnWindowMoveButtonPressed(KeyCode.DownArrow);
			});
			input.UpdateEvent(system.InputSystem.Type.Down, KeyCode.LeftArrow, () =>
			{
				OnWindowMoveButtonPressed(KeyCode.LeftArrow);
			});
			input.UpdateEvent(system.InputSystem.Type.Down, KeyCode.RightArrow, () =>
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

		public void Go()
		{
			StartCoroutine(GoCoroutine());
		}

		private IEnumerator GoCoroutine()
		{
			while (true)
			{
				if (m_windows.Count(d => d.IsActiveWindow == true) <= 0)
				{
					yield return null;
					continue;
				}
				if (m_selectWindowIndex <= -1)
				{
					yield return null;
					continue;
				}
				//Debug.Log("m_moveButtonPressKeys.Count = " + m_moveButtonPressKeys.Count);
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

					m_moveVector += addVector.normalized * 5.0f;
					if (m_moveVector.magnitude > 25.0f)
					{
						m_moveVector = m_moveVector.normalized * 25.0f;
					}
				}
				else
				{
					m_moveVector *= 0.5f;
					if (m_moveVector.magnitude < 0.1f)
					{
						m_moveVector = Vector2.zero;
					}
				}
				if (m_moveVector != Vector2.zero)
				{
					m_windows[m_selectWindowIndex].OnMove(m_moveVector);
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

			int nextWindowIndex = m_selectWindowIndex;
			while (true)
			{
				nextWindowIndex++;
				if (nextWindowIndex >= m_windows.Count)
				{
					nextWindowIndex = 0;
				}
				if (m_windows[nextWindowIndex].IsActiveWindow == true)
				{
					break;
				}
			}
			SelectWindow(nextWindowIndex);
		}

		private void OnWindowChangeLeftButtonPressed()
		{
			if (m_windows.Count(d => d.IsActiveWindow == true) <= 1)
			{
				return;
			}

			int nextWindowIndex = m_selectWindowIndex;
			while (true)
			{
				nextWindowIndex--;
				if (nextWindowIndex < 0)
				{
					nextWindowIndex = m_windows.Count - 1;
				}
				if (m_windows[nextWindowIndex].IsActiveWindow == true)
				{
					break;
				}
			}
			SelectWindow(nextWindowIndex);
		}

		//public void AddWindow(WindowBase.Type windowType)
		//{
		//	StartCoroutine(AddWindowCoroutine(windowType));
		//}

		private IEnumerator AddWindowCoroutine(WindowBase.Type windowType)
		{
			var windowPrefab = m_windowPrefabList[(int)windowType];
			var window = GameObject.Instantiate(windowPrefab);
			window.transform.SetParent(m_windowParent);
			window.transform.localPosition = Vector3.zero;
			window.transform.localScale = Vector3.one;
			var windowBase = window.GetComponent<WindowBase>();
			m_windows.Add(windowBase);
			int index = m_windows.Count - 1;
			switch (windowBase.WindowType)
			{
				case WindowBase.Type.Main:
					{
						((MainWindow)windowBase).Initialize(
							inputEvent: m_inputEvent,
							windowArea: m_windowParent.GetComponent<RectTransform>(),
							holdCallback: () =>
							{
								SelectWindow(index);
							});
						break;
					}
				case WindowBase.Type.Message:
					{
						((MessageWindow)windowBase).Initialize(
							area: m_windowParent.GetComponent<RectTransform>(),
							holdCallback: () =>
							{
								SelectWindow(index);
							});
						break;
					}
				case WindowBase.Type.DateTime:
					{
						((DateTimeWindow)windowBase).Initialize(
							area: m_windowParent.GetComponent<RectTransform>(),
							holdCallback: () =>
							{
								SelectWindow(index);
							});
						break;
					}
				case WindowBase.Type.CheckSheet:
					{
						((CheckSheetWindow)windowBase).Initialize(
							area: m_windowParent.GetComponent<RectTransform>(),
							holdCallback: () =>
							{
								SelectWindow(index);
							});
						break;
					}
			}
			windowBase.Go();
			yield return windowBase.AddWindow();
			UpdateWindowIcon();
		}

		//public void RemoveWindow(int index)
		//{
		//	StartCoroutine(RemoveWindowCoroutine(index));
		//}

		private IEnumerator RemoveWindowCoroutine(int index)
		{
			var window = m_windows[index];
			yield return window.RemoveWindow();
			GameObject.Destroy(window.gameObject);
			m_windows.RemoveAt(index);

			if (m_windows.Count > index)
			{
				SelectWindow(index);
			}
			else if (m_windows.Count > 0)
			{
				SelectWindow(index - 1);
			}
			else
			{
				SelectWindow(-1);
			}
			UpdateWindowIcon();
		}

		public void SelectWindow(int index)
		{
			for (int i = 0; i < m_windows.Count; ++i)
			{
				m_windows[i].SetSelect(i == index, m_windows.Count);
			}
			m_selectWindowIndex = index;
			UpdateWindowIcon();
		}

		private void UpdateWindowIcon()
		{
			List<WindowBase.Type> m_windowTypes = new List<WindowBase.Type>();
			for (int i = 0; i < m_windows.Count; ++i)
			{
				if (m_windows[i].IsActiveWindow == true)
				{
					m_windowTypes.Add(m_windows[i].WindowType);
				}
			}
			m_updateWindowIcon(m_windowTypes.ToArray(), m_selectWindowIndex);
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
				case "SelectWindow":
					{
						int index = int.Parse(paramStrings[1]);
						SelectWindow(index);
						break;
					}
				case "AddMainWindow":
					{
						yield return AddWindowCoroutine(WindowBase.Type.Main);
						break;
					}
				case "AddMessageWindow":
					{
						yield return AddWindowCoroutine(WindowBase.Type.Message);
						break;
					}
				case "AddDateTimeWindow":
					{
						yield return AddWindowCoroutine(WindowBase.Type.DateTime);

						var windowBase = m_windows[m_windows.Count - 1];
						string dateTimeString = paramStrings[1];
						yield return ((DateTimeWindow)windowBase).PlayCoroutine(dateTimeString);
						break;
					}
				case "AddCheckSheetWindow":
					{
						yield return AddWindowCoroutine(WindowBase.Type.CheckSheet);

						var windowBase = m_windows[m_windows.Count - 1];
						int dateId = int.Parse(paramStrings[1]);
						((CheckSheetWindow)windowBase).Setting(dateId);
						break;
					}
				case "AddMessage":
					{
						var windowBase = m_windows.Find(d => d.WindowType == WindowBase.Type.Message);
						if (windowBase != null)
						{
							bool isMine = bool.Parse(paramStrings[1]);
							int messageId = int.Parse(paramStrings[2]);
							((MessageWindow)windowBase).AddMessage(isMine, messageId);
						}

						break;
					}
				case "RemoveWindow":
					{
						int index = int.Parse(paramStrings[1]);
						yield return RemoveWindowCoroutine(index);
						break;
					}
				default:
					{
						Debug.LogError("StageScene.cs OnMovieStart ErrorCommand " + paramStrings[0]);
						if (callback != null)
						{
							callback();
						}
						break;
					}
			}

			if (callback != null)
			{
				callback();
			}
		}

		public void SetupEvent(string param, UnityAction callback)
		{
			Debug.Log("WindowController param = " + param);
			string[] paramStrings = param.Split(',');
			WindowBase window = null;
			switch (paramStrings[0])
			{
				case "Main":
					{
						window = m_windows.Find(d => d.WindowType == WindowBase.Type.Main);
						break;
					}
			}

			if (window == null)
			{
				Debug.LogError("Windowに追加される前にSetupEventが実行されている");
				return;
			}
			paramStrings = paramStrings.Skip(1).ToArray();
			window.SetupEvent(paramStrings, callback);
		}
	}
}
