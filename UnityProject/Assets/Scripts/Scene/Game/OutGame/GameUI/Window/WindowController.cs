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

		private UnityAction<int> m_commonWindowPlayMovieEvent;
		private UnityAction m_feedbackButtonEvent;
		private UnityAction m_feedbackWindowPowerButtonEvent;
		private UnityAction m_feedbackWindowRecreateButtonEvent;
		private UnityAction m_feedbackWindowReleaseButtonEvent;
		private UnityAction m_feedbackWindowCanselButtonEvent;
		private UnityAction<KeyCode[]> m_mainWindowInputEvent;

		public void Initialize(
			UnityAction<int> commonWindowPlayMovieEvent,
			UnityAction feedbackButtonEvent,
			UnityAction powerButtonEvent,
			UnityAction recreateButtonEvent,
			UnityAction releaseButtonEvent,
			UnityAction canselButtonEvent,
			UnityAction<KeyCode[]> mainWindowInputEvent)
		{
			m_commonWindowPlayMovieEvent = commonWindowPlayMovieEvent;
			m_feedbackButtonEvent = feedbackButtonEvent;
			m_feedbackWindowPowerButtonEvent = powerButtonEvent;
			m_feedbackWindowRecreateButtonEvent = recreateButtonEvent;
			m_feedbackWindowReleaseButtonEvent = releaseButtonEvent;
			m_feedbackWindowCanselButtonEvent = canselButtonEvent;
			m_mainWindowInputEvent = mainWindowInputEvent;
		}

		public void Go()
		{
		}

		private IEnumerator AddWindowCoroutine(WindowBase.Type windowType, Vector3 pos, UnityAction<WindowBase> settingEvent)
		{
			var windowPrefab = m_windowPrefabList[(int)windowType];
			var window = GameObject.Instantiate(windowPrefab);
			window.transform.SetParent(m_windowParent);
			window.transform.localPosition = pos;
			window.transform.localScale = Vector3.one;
			var windowBase = window.GetComponent<WindowBase>();
			int beforePriority = -1;
			if (m_windows.Count > 0)
			{
				beforePriority = m_windows[m_windows.Count - 1].Priority;
			}
			m_windows.Add(windowBase);
			windowBase.Initialize(SetTopSibling);
			if (windowBase.Priority > beforePriority)
			{
				// 以前のウィンドウより優先度の高いウィンドウが展開した場合は、
				// 優先度の低いウィンドウのキーイベントを削除に無効に
				ResetInputKeyEvent();
			}
			windowBase.SetupInputKeyEvent();

			if (settingEvent != null)
			{
				settingEvent(windowBase);
			}

			yield return windowBase.AddWindow();
			windowBase.Go();
		}

		private void RemoveWindow(int index, UnityAction callback)
		{
			StartCoroutine(RemoveWindowCoroutine(index, callback));
		}

		private IEnumerator RemoveWindowCoroutine(int index, UnityAction callback)
		{
			var window = m_windows[index];
			int nowPriority = window.Priority;
			yield return window.RemoveWindow();
			GameObject.Destroy(window.gameObject);
			m_windows.RemoveAt(index);

			var nowPriorityWindows = m_windows.FindAll(d => d.Priority == nowPriority);
			if (nowPriorityWindows.Count <= 0)
			{
				// 閉じたウィンドウの優先度と同じ優先度のウィンドウが存在しない場合は、
				// １つ優先度の低いウィンドウのキーイベントを設定する
				ResetInputKeyEvent();
				var prePriorityWindows = m_windows.FindAll(d => d.Priority == nowPriority - 1);
				for (int i = 0; i < prePriorityWindows.Count; ++i)
				{
					prePriorityWindows[i].SetupInputKeyEvent();
				}
			}

			if (callback != null)
			{
				callback();
			}
		}

		private IEnumerator MoveWindowCoroutine(int index, float time, Vector2 pos)
		{
			var window = m_windows[index];
			var windowTransform = window.transform;
			yield return CommonMath.EaseInOutTransform(
				windowTransform,
				windowTransform.localPosition,
				windowTransform.localRotation,
				pos,
				windowTransform.localRotation,
				time,
				null);
		}

		public void OnMovieStart(string[] paramStrings, UnityAction callback)
		{
			StartCoroutine(OnMovieStartCoroutine(paramStrings, callback));
		}

		private IEnumerator OnMovieStartCoroutine(string[] paramStrings, UnityAction callback)
		{
			switch (paramStrings[0])
			{
				case "AddTitleWindow":
					{
						bool isDone = false;
						int x = int.Parse(paramStrings[1]);
						int y = int.Parse(paramStrings[2]);
						yield return AddWindowCoroutine(WindowBase.Type.Title, new Vector3(x, y, 0), (windowBase) =>
						{
							((TitleWindow)windowBase).Setting(
								onButtonPressEvent: () => { isDone = true; });
						});
						while (!isDone) { yield return null; }
						break;
					}
				case "AddMainWindow":
					{
						int x = int.Parse(paramStrings[1]);
						int y = int.Parse(paramStrings[2]);
						Game.GenreType genreType = (Game.GenreType)System.Enum.Parse(typeof(Game.GenreType), paramStrings[3]);
						yield return AddWindowCoroutine(WindowBase.Type.Main, new Vector3(x, y, 0), (windowBase) =>
						{
							((MainWindow)windowBase).Setting(
								genreType: genreType,
								inputEvent: m_mainWindowInputEvent);
						});
						break;
					}
				case "AddMessageWindow":
					{
						int x = int.Parse(paramStrings[1]);
						int y = int.Parse(paramStrings[2]);
						yield return AddWindowCoroutine(WindowBase.Type.Message, new Vector3(x, y, 0), null);
						break;
					}
				case "AddDateTimeWindow":
					{
						int x = int.Parse(paramStrings[1]);
						int y = int.Parse(paramStrings[2]);
						yield return AddWindowCoroutine(WindowBase.Type.DateTime, new Vector3(x, y, 0), null);
						break;
					}
				case "AddCheckSheetWindow":
					{
						int x = int.Parse(paramStrings[1]);
						int y = int.Parse(paramStrings[2]);
						yield return AddWindowCoroutine(WindowBase.Type.CheckSheet, new Vector3(x, y, 0), (windowBase) =>
						{
							((CheckSheetWindow)windowBase).Setting();
						});
						break;
					}
				case "AddCameraWindow":
					{
						int x = int.Parse(paramStrings[1]);
						int y = int.Parse(paramStrings[2]);
						yield return AddWindowCoroutine(WindowBase.Type.Camera, new Vector3(x, y, 0), null);
						break;
					}
				case "AddCommonWindow":
					{
						int x = int.Parse(paramStrings[1]);
						int y = int.Parse(paramStrings[2]);
						int messageId = int.Parse(paramStrings[3]);
						int yesPlayMovieId = int.Parse(paramStrings[4]);
						int noPlayMovieId = int.Parse(paramStrings[5]);
						yield return AddWindowCoroutine(WindowBase.Type.Common, new Vector3(x, y, 0), (windowBase) =>
						{
							((CommonWindow)windowBase).Setting(
								CommonUI.LocalizeText.GetString(messageId),
								() => { if (yesPlayMovieId >= 0) m_commonWindowPlayMovieEvent(yesPlayMovieId); },
								() => { if (noPlayMovieId >= 0) m_commonWindowPlayMovieEvent(noPlayMovieId); }); ;
						});
						break;
					}
				case "AddFeedbackWindow":
					{
						int x = int.Parse(paramStrings[1]);
						int y = int.Parse(paramStrings[2]);
						yield return AddWindowCoroutine(WindowBase.Type.Feedback, new Vector3(x, y, 0), (windowBase) =>
						{
							((FeedbackWindow)windowBase).Setting(
								powerButtonEvent: m_feedbackWindowPowerButtonEvent,
								recreateButtonEvent: m_feedbackWindowRecreateButtonEvent,
								releaseButtonEvent: m_feedbackWindowReleaseButtonEvent,
								canselButtonEvent: m_feedbackWindowCanselButtonEvent);
						});
						break;
					}
				case "AddImageWindow":
					{
						int x = int.Parse(paramStrings[1]);
						int y = int.Parse(paramStrings[2]);
						int id = int.Parse(paramStrings[3]);
						yield return AddWindowCoroutine(WindowBase.Type.Image, new Vector3(x, y, 0), (windowBase) =>
						{
							((ImageWindow)windowBase).Setting(id);
						});
						break;
					}
				case "AddResultWindow":
					{
						int x = int.Parse(paramStrings[1]);
						int y = int.Parse(paramStrings[2]);
						yield return AddWindowCoroutine(WindowBase.Type.Result, new Vector3(x, y, 0), (windowBase) =>
						{
							((ResultWindow)windowBase).Setting();
						});
						break;
					}
				case "PlayWindow":
					{
						WindowBase.Type windowType = (WindowBase.Type)System.Enum.Parse(typeof(WindowBase.Type), paramStrings[1]);
						var windowBase = m_windows.Find(d => d.WindowType == windowType);
						if (windowBase != null)
						{
							bool isDone = false;
							string[] parameters = paramStrings.Skip(2).ToArray();
							windowBase.OnMovieStart(parameters, () => { isDone = true; });
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

		//public void SetupEvent(string param, UnityAction callback)
		//{
		//	string[] paramStrings = param.Split(',');
		//	WindowBase window = null;
		//	switch (paramStrings[0])
		//	{
		//		case "Main":
		//			{
		//				window = m_windows.Find(d => d.WindowType == WindowBase.Type.Main);
		//				break;
		//			}
		//		case "Chara":
		//			{
		//				window = m_windows.Find(d => d.WindowType == WindowBase.Type.Camera);
		//				break;
		//			}
		//	}

		//	if (window == null)
		//	{
		//		Debug.LogError("Windowに追加される前にSetupEventが実行されている");
		//		return;
		//	}
		//	paramStrings = paramStrings.Skip(1).ToArray();
		//	window.SetupEvent(paramStrings, callback);
		//}

		private void SetTopSibling(Transform windowTransform)
		{
			windowTransform.SetSiblingIndex(m_windows.Count - 1);
		}

		private void ResetInputKeyEvent()
		{
			GeneralRoot.Input.ClearEvent();
			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.X, () =>
			{
				m_feedbackButtonEvent();
			});
		}
	}
}
