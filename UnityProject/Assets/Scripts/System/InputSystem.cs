using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace system
{
	public class InputSystem : MonoBehaviour
	{
		public enum Type
		{
			//Press,
			Down,
			Up
		}

		private class Data
		{
			private Type m_pressType;
			public Type PressType => m_pressType;

			private KeyCode m_key;
			public KeyCode Key => m_key;

			private UnityAction m_event;
			public UnityAction Event => m_event;

			public Data(Type pressType, KeyCode key, UnityAction e)
			{
				m_pressType = pressType;
				m_key = key;
				m_event = e;
			}

			public void UpdateEvent(UnityAction e)
			{
				m_event = e;
			}
		}





		/// <summary>
		/// レイキャストアクティブ時である為、無効状態かどうか
		/// </summary>
		private bool m_isRayCastActive;

		/// <summary>
		/// キー入力イベントリスト
		/// </summary>
		private List<Data> m_inputEventList = new List<Data>();

		public void Initialize()
		{
			// マルチタッチ無効
			Input.multiTouchEnabled = false;
		}

		private void Update()
		{
			if (m_isRayCastActive == true)
			{
				return;
			}

			for (int i = 0; i < m_inputEventList.Count; ++i)
			{
				var eventData = m_inputEventList[i];
				bool isFire = GetButton(eventData.PressType, eventData.Key);
				if (isFire == true && eventData.Event != null)
				{
					eventData.Event();
				}
			}
		}

		public void UpdateEvent(Type pressType, KeyCode key, UnityAction e)
		{
			var find = m_inputEventList.Find(d => d.PressType == pressType && d.Key == key);
			if (find != null)
			{
				find.UpdateEvent(e);
			}
			else
			{
				m_inputEventList.Add(new Data(pressType, key, e));
			}
		}

		public void ClearEvent()
		{
			m_inputEventList.Clear();
		}

		public Vector3 GetMousePosition()
		{
			return Input.mousePosition;
		}

		public bool IsMouseDown()
		{
			return Input.GetMouseButton(0);
		}

		public void SetRayCastActive(bool value)
		{
			m_isRayCastActive = value;
		}

		private bool GetButton(Type type, KeyCode code)
		{
			var gamepad = Gamepad.current;
			if (gamepad != null)
			{
				// ゲームパッド入力
				UnityEngine.InputSystem.Controls.ButtonControl control = null;
				switch (code)
				{
					case KeyCode.W:
						{
							control = gamepad.leftStick.up;
							break;
						}
					case KeyCode.S:
						{
							control = gamepad.leftStick.down;
							break;
						}
					case KeyCode.A:
						{
							control = gamepad.leftStick.left;
							break;
						}
					case KeyCode.D:
						{
							control = gamepad.leftStick.right;
							break;
						}
					case KeyCode.UpArrow:
						{
							control = gamepad.dpad.up;
							break;
						}
					case KeyCode.DownArrow:
						{
							control = gamepad.dpad.down;
							break;
						}
					case KeyCode.LeftArrow:
						{
							control = gamepad.dpad.left;
							break;
						}
					case KeyCode.RightArrow:
						{
							control = gamepad.dpad.right;
							break;
						}
					case KeyCode.Q:
						{
							control = gamepad.leftTrigger;
							break;
						}
					case KeyCode.E:
						{
							control = gamepad.rightTrigger;
							break;
						}
					case KeyCode.Space:	// 南、A
						{
							control = gamepad.buttonSouth;
							break;
						}
					case KeyCode.Z:	// 北、Y
						{
							control = gamepad.buttonNorth;
							break;
						}
					case KeyCode.X:	// 西、X
						{
							control = gamepad.buttonWest;
							break;
						}
					case KeyCode.C:	// 東、B
						{
							control = gamepad.buttonEast;
							break;
						}
				}

				if (control != null)
				{
					switch (type)
					{
						case Type.Down:
							{
								return control.wasPressedThisFrame;
							}
						case Type.Up:
							{
								return control.wasReleasedThisFrame;
							}
					}
				}
			}

			var keyboard = Keyboard.current;
			if (keyboard != null)
			{
				// キーボード入力
				UnityEngine.InputSystem.Controls.ButtonControl control = null;
				switch (code)
				{
					case KeyCode.W:
						{
							control = keyboard.wKey;
							break;
						}
					case KeyCode.S:
						{
							control = keyboard.sKey;
							break;
						}
					case KeyCode.A:
						{
							control = keyboard.aKey;
							break;
						}
					case KeyCode.D:
						{
							control = keyboard.dKey;
							break;
						}
					case KeyCode.UpArrow:
						{
							control = keyboard.upArrowKey;
							break;
						}
					case KeyCode.DownArrow:
						{
							control = keyboard.downArrowKey;
							break;
						}
					case KeyCode.LeftArrow:
						{
							control = keyboard.leftArrowKey;
							break;
						}
					case KeyCode.RightArrow:
						{
							control = keyboard.rightArrowKey;
							break;
						}
					case KeyCode.Q:
						{
							control = keyboard.qKey;
							break;
						}
					case KeyCode.E:
						{
							control = keyboard.eKey;
							break;
						}
					case KeyCode.Space:
						{
							control = keyboard.spaceKey;
							break;
						}
					case KeyCode.Z:
						{
							control = keyboard.zKey;
							break;
						}
					case KeyCode.X:
						{
							control = keyboard.xKey;
							break;
						}
					case KeyCode.C:
						{
							control = keyboard.cKey;
							break;
						}
				}

				if (control != null)
				{
					switch (type)
					{
						case Type.Down:
							{
								return control.wasPressedThisFrame;
							}
						case Type.Up:
							{
								return control.wasReleasedThisFrame;
							}
					}
				}
			}

			return false;
		}
	}
}
