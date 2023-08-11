using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace system
{
	public class InputSystem : MonoBehaviour
	{
		public enum Type
		{
			Press,
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
			for (int i = 0; i < m_inputEventList.Count; ++i)
			{
				var eventData = m_inputEventList[i];
				bool isFire = false;
				switch (eventData.PressType)
				{
					case Type.Press:
						{
							isFire = Input.GetKey(eventData.Key);
							break;
						}
					case Type.Down:
						{
							isFire = Input.GetKeyDown(eventData.Key);
							break;
						}
					case Type.Up:
						{
							isFire = Input.GetKeyUp(eventData.Key);
							break;
						}
				}
				if (isFire == true)
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
	}
}
