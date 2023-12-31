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

		private List<KeyCode> m_moveButtonPressKeys = new List<KeyCode>();
		private Vector2 m_moveVector = Vector2.zero;

		private UnityAction<int> m_commonWindowPlayMovieEvent;
		private UnityAction m_mainWindowPowerButtonEvent;
		private UnityAction<KeyCode[]> m_mainWindowInputEvent;
		private UnityAction<WindowBase.Type[], int> m_updateWindowIcon;

		public void Initialize(
			UnityAction<int> commonWindowPlayMovieEvent,
			UnityAction mainWindowPowerButtonEvent,
			UnityAction<KeyCode[]> mainWindowInputEvent,
			UnityAction<WindowBase.Type[], int> updateWindowIcon)
		{
			m_commonWindowPlayMovieEvent = commonWindowPlayMovieEvent;
			m_mainWindowPowerButtonEvent = mainWindowPowerButtonEvent;
			m_mainWindowInputEvent = mainWindowInputEvent;
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

					if ((m_moveVector.x < 0 && addVector.x > 0) || (m_moveVector.x > 0 && addVector.x < 0))
					{
						m_moveVector.x = 0.0f;
					}
					if ((m_moveVector.y < 0 && addVector.y > 0) || (m_moveVector.y > 0 && addVector.y < 0))
					{
						m_moveVector.y = 0.0f;
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
			if (IsEnableChangeWindow() == false)
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
			SelectWindow(nextWindowIndex, null);
		}

		private void OnWindowChangeLeftButtonPressed()
		{
			if (IsEnableChangeWindow() == false)
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
			SelectWindow(nextWindowIndex, null);
		}

		/// <summary>
		/// ウィンドウ切り替え可能な状況かどうかを取得
		/// </summary>
		/// <returns></returns>
		private bool IsEnableChangeWindow()
		{
			if (m_windows.Count(d => d.IsActiveWindow == true) <= 1)
			{
				return false;
			}

			if (m_windows[m_selectWindowIndex].WindowType == WindowBase.Type.Common)
			{
				// 汎用ダイアログ展開中で返事待ち
				return false;
			}
			return true;
		}

		private IEnumerator AddWindowCoroutine(WindowBase.Type windowType, UnityAction<WindowBase> settingEvent)
		{
			var windowPrefab = m_windowPrefabList[(int)windowType];
			var window = GameObject.Instantiate(windowPrefab);
			window.transform.SetParent(m_windowParent);
			window.transform.localPosition = new Vector3(0.0f, -64.0f, 0.0f);
			window.transform.localScale = Vector3.one;
			var windowBase = window.GetComponent<WindowBase>();
			m_windows.Add(windowBase);
			windowBase.Initialize(
				windowArea: m_windowParent.GetComponent<RectTransform>(),
				holdCallback: null);
			UpdateWindowIcon();

			if (settingEvent != null)
			{
				settingEvent(windowBase);
			}

			windowBase.Go();
			yield return windowBase.AddWindow();
			yield return SelectWindowCoroutine(m_windows.Count - 1, null);
		}

		private void RemoveWindow(int index, UnityAction callback)
		{
			StartCoroutine(RemoveWindowCoroutine(index, callback));
		}

		private IEnumerator RemoveWindowCoroutine(int index, UnityAction callback)
		{
			var window = m_windows[index];
			if (index == m_selectWindowIndex)
			{
				yield return SelectWindowCoroutine(-1, null);
			}
			yield return window.RemoveWindow();
			GameObject.Destroy(window.gameObject);
			m_windows.RemoveAt(index);

			if (m_windows.Count > 0)
			{
				if (m_windows.Count > index)
				{
					yield return SelectWindowCoroutine(index, null);
				}
				else
				{
					yield return SelectWindowCoroutine(index - 1, null);
				}
			}
			UpdateWindowIcon();

			if (callback != null)
			{
				callback();
			}
		}

		private IEnumerator MoveWindowCoroutine(int index, float time, Vector2 pos)
		{
			var curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
			var window = m_windows[index];
			Vector2 beforePos = window.transform.localPosition;
			float nowTime = 0.0f;
			while (nowTime < time)
			{
				float value = curve.Evaluate(nowTime);
				Vector2 nowPos = (pos - beforePos) * value + beforePos;
				window.transform.localPosition = nowPos;

				nowTime += Time.deltaTime;

				yield return null;
			}
			window.transform.localPosition = pos;
		}

		public void SelectWindow(int index, UnityAction callback)
		{
			StartCoroutine(SelectWindowCoroutine(index, callback));
		}

		private IEnumerator SelectWindowCoroutine(int index, UnityAction callback)
		{
			int doneCount = 0;
			for (int i = 0; i < m_windows.Count; ++i)
			{
				m_windows[i].SetSelect(i == index, m_windows.Count, () => { doneCount++; });
			}
			while (doneCount < m_windows.Count) { yield return null; }
			m_selectWindowIndex = index;
			UpdateWindowIcon();

			if (callback != null)
			{
				callback();
			}
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
						yield return SelectWindowCoroutine(index, null);
						break;
					}
				case "AddTitleWindow":
					{
						bool isDone = false;
						yield return AddWindowCoroutine(WindowBase.Type.Title, (windowBase) =>
						{
							((TitleWindow)windowBase).Setting(
								onButtonPressEvent: () => { isDone = true; });
						});
						while (!isDone) { yield return null; }
						break;
					}
				case "AddMainWindow":
					{
						yield return AddWindowCoroutine(WindowBase.Type.Main, (windowBase) =>
						{
							((MainWindow)windowBase).Setting(
								powerButtonEvent: m_mainWindowPowerButtonEvent,
								inputEvent: m_mainWindowInputEvent);
						});
						break;
					}
				case "AddMessageWindow":
					{
						yield return AddWindowCoroutine(WindowBase.Type.Message, null);
						break;
					}
				case "AddDateTimeWindow":
					{
						yield return AddWindowCoroutine(WindowBase.Type.DateTime, null);
						break;
					}
				case "AddCheckSheetWindow":
					{
						yield return AddWindowCoroutine(WindowBase.Type.CheckSheet, (windowBase) =>
						{
							var local = GeneralRoot.User.LocalSaveData;
							int userCheckSheetId = local.CheckSheetId;
							int userAccuracyLevel = local.AccuracyLevel;
							int[] userOccurredBugIds = local.OccurredBugIds;
							((CheckSheetWindow)windowBase).Setting(
								userCheckSheetId,
								userAccuracyLevel,
								userOccurredBugIds);
						});
						break;
					}
				case "AddCharaWindow":
					{
						yield return AddWindowCoroutine(WindowBase.Type.Chara, null);
						break;
					}
				case "AddCommonWindow":
					{
						yield return AddWindowCoroutine(WindowBase.Type.Common, (windowBase) =>
						{
							int messageId = int.Parse(paramStrings[1]);
							string message = "デバッグ結果の報告の為、アップロードしますが\nよろしいですか？";
							int yesPlayMovieId = int.Parse(paramStrings[2]);
							int noPlayMovieId = int.Parse(paramStrings[3]);
							((CommonWindow)windowBase).Setting(
								message,
								() => { if (yesPlayMovieId >= 0) m_commonWindowPlayMovieEvent(yesPlayMovieId); },
								() => { if (noPlayMovieId >= 0) m_commonWindowPlayMovieEvent(noPlayMovieId); });
						});
						break;
					}
				case "PlayMessageWindow":
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
				case "PlayDateTimeWindow":
					{
						var windowBase = m_windows.Find(d => d.WindowType == WindowBase.Type.DateTime);
						if (windowBase != null)
						{
							bool isDone = false;
							int controllId = int.Parse(paramStrings[1]);
							((DateTimeWindow)windowBase).Play(controllId, () => { isDone = true; });
							while (!isDone) { yield return null; }
						}
						break;
					}
				case "PlayCharaWindow":
					{
						var windowBase = m_windows.Find(d => d.WindowType == WindowBase.Type.Chara);
						if (windowBase != null)
						{
							bool isDone = false;
							int controllId = int.Parse(paramStrings[1]);
							((CharaWindow)windowBase).Play(controllId, () => { isDone = true; });
							while (!isDone) { yield return null; }
						}
						break;
					}
				case "RemoveWindow":
					{
						int index = int.Parse(paramStrings[1]);
						bool isDone = false;
						RemoveWindow(index, () => { isDone = true; });
						while (!isDone) { yield return null; }
						break;
					}
				case "MoveWindow":
					{
						int index = int.Parse(paramStrings[1]);
						float time = float.Parse(paramStrings[2]);
						Vector2 pos = new Vector2(float.Parse(paramStrings[3]), float.Parse(paramStrings[4]));
						yield return MoveWindowCoroutine(index, time, pos);
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
			string[] paramStrings = param.Split(',');
			WindowBase window = null;
			switch (paramStrings[0])
			{
				case "Main":
					{
						window = m_windows.Find(d => d.WindowType == WindowBase.Type.Main);
						break;
					}
				case "Chara":
					{
						window = m_windows.Find(d => d.WindowType == WindowBase.Type.Chara);
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
