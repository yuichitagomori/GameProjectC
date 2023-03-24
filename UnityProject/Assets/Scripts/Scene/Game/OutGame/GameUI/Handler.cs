using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace scene.game.outgame
{
	/// <summary>
	/// ハンドルイベント受け取りクラス
	/// </summary>
	public class Handler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
		public class EventData
		{
			/// <summary>
			/// ドラッグ時イベント
			/// </summary>
			private UnityAction<Vector2> m_dragEvent = null;
			public UnityAction<Vector2> DragEvent => m_dragEvent;

			/// <summary>
			/// ドラッグ終了時イベント
			/// </summary>
			private UnityAction m_endDragEvent = null;
			public UnityAction EndDragEvent => m_endDragEvent;

			/// <summary>
			/// クリック時イベント
			/// </summary>
			private UnityAction<Vector2> m_clickEvent = null;
			public UnityAction<Vector2> ClickEvent => m_clickEvent;

			public EventData(UnityAction<Vector2> dragEvent, UnityAction endDragEvent, UnityAction<Vector2> clickEvent)
			{
				m_dragEvent = dragEvent;
				m_endDragEvent = endDragEvent;
				m_clickEvent = clickEvent;
			}
		}

		private enum HanderType
		{
			BeginFixed,
			LengthFixed,
		}

		/// <summary>
		/// ハンドルタイプ
		/// </summary>
		[SerializeField]
		private HanderType m_type = HanderType.BeginFixed;

		[SerializeField]
		private GameObject m_beginObject = null;

		[SerializeField]
		private GameObject m_endObject = null;

		/// <summary>
		/// Begin時に原点から開始されるかどうか
		/// </summary>
		[SerializeField]
		private bool m_isBeginLocalPosition = false;

		/// <summary>
		/// キーボード入力
		/// </summary>
		[SerializeField]
		private bool m_isInputKey = false;

		/// <summary>
		/// イベントデータ
		/// </summary>
		private EventData m_eventData = null;

		/// <summary>
		/// 開始座標
		/// </summary>
		private Vector2 m_beginVector = Vector3.zero;

		/// <summary>
		/// 方向
		/// </summary>
		private Vector2 m_dragVector = Vector2.zero;

		/// <summary>
		/// 方向の長さ
		/// </summary>
		private float m_vectorMagnitude = 0.0f;



		private IEnumerator UpdateCoroutine()
		{
			if (m_eventData == null || m_eventData.DragEvent == null)
			{
				yield break;
			}

			const float keyDragLength = 100.0f;
			System.Collections.Generic.Dictionary<KeyCode, bool> IsKeyDown = new System.Collections.Generic.Dictionary<KeyCode, bool>()
			{
				{ KeyCode.W, false },
				{ KeyCode.S, false },
				{ KeyCode.A, false },
				{ KeyCode.D, false }
			};

			while (true)
			{
				if (m_isInputKey == true)
				{
					// 移動量測定
					if (Input.GetKeyDown(KeyCode.W) == true)
					{
						IsKeyDown[KeyCode.W] = true;
					}
					if (Input.GetKeyDown(KeyCode.S) == true)
					{
						IsKeyDown[KeyCode.S] = true;
					}
					if (Input.GetKeyDown(KeyCode.A) == true)
					{
						IsKeyDown[KeyCode.A] = true;
					}
					if (Input.GetKeyDown(KeyCode.D) == true)
					{
						IsKeyDown[KeyCode.D] = true;
					}

					if (Input.GetKeyUp(KeyCode.W) == true)
					{
						IsKeyDown[KeyCode.W] = false;
					}
					if (Input.GetKeyUp(KeyCode.S) == true)
					{
						IsKeyDown[KeyCode.S] = false;
					}
					if (Input.GetKeyUp(KeyCode.A) == true)
					{
						IsKeyDown[KeyCode.A] = false;
					}
					if (Input.GetKeyUp(KeyCode.D) == true)
					{
						IsKeyDown[KeyCode.D] = false;
					}

					Vector2 drag = Vector2.zero;
					if (IsKeyDown[KeyCode.W] == true)
					{
						drag += new Vector2(0.0f, 1.0f);
					}
					if (IsKeyDown[KeyCode.S] == true)
					{
						drag += new Vector2(0.0f, -1.0f);
					}
					if (IsKeyDown[KeyCode.A] == true)
					{
						drag += new Vector2(-1.0f, 0.0f);
					}
					if (IsKeyDown[KeyCode.D] == true)
					{
						drag += new Vector2(1.0f, 0.0f);
					}

					if (drag != Vector2.zero)
					{
						m_dragVector = drag.normalized * keyDragLength;
						m_vectorMagnitude = keyDragLength;
					}
					else
					{
						if (m_eventData != null && m_eventData.EndDragEvent != null &&
							m_dragVector != Vector2.zero)
						{
							m_eventData.EndDragEvent.Invoke();
						}

						m_dragVector = Vector2.zero;
						m_vectorMagnitude = 0.0f;
					}
				}

				if (m_vectorMagnitude >= 1.0f)
				{
					//Debug.Log("m_dragVector = " + m_dragVector);
					m_eventData.DragEvent.Invoke(m_dragVector);

					if (m_beginObject != null)
					{
						m_beginObject.transform.position = m_beginVector;
					}
					if (m_endObject != null)
					{
						m_endObject.transform.position = m_beginVector + m_dragVector;
					}
				}

				yield return null;
			}
		}

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="eventData"></param>
		public void Initialize(EventData eventData)
		{
			m_eventData = eventData;
			m_beginVector = Vector3.zero;
			m_dragVector = Vector2.zero;
			m_vectorMagnitude = 0.0f;

			if (m_beginObject != null)
			{
				m_beginObject.SetActive(false);
			}
			if (m_endObject != null)
			{
				m_endObject.SetActive(false);
			}

			StartCoroutine(UpdateCoroutine());
		}

		/// <summary>
		/// ドラックが開始したとき呼ばれる
		/// </summary>
		/// <param name="e"></param>
		public void OnBeginDrag(PointerEventData e)
		{
			//Debug.Log("OnBeginDrag e.position = " + e.position);
			if (m_isBeginLocalPosition == true)
			{
				m_beginVector = transform.position;
			}
			else
			{
				m_beginVector = e.position;
			}
			m_vectorMagnitude = 0.0f;

			if (m_beginObject != null)
			{
				m_beginObject.SetActive(true);
			}
			if (m_endObject != null)
			{
				m_endObject.SetActive(true);
			}
		}

		/// <summary>
		/// ドラック中に呼ばれる
		/// </summary>
		/// <param name="e"></param>
		public void OnDrag(PointerEventData e)
		{
			//Debug.Log("OnDrag e.position = " + e.position);
			m_dragVector = e.position - m_beginVector;
			m_vectorMagnitude = m_dragVector.magnitude;
			if (m_type == HanderType.LengthFixed && m_vectorMagnitude > 50.0f)
			{
				m_beginVector = e.position - m_dragVector.normalized * 50.0f;
			}
		}

		/// <summary>
		/// ドラックが終了したとき呼ばれる
		/// </summary>
		/// <param name="e"></param>
		public void OnEndDrag(PointerEventData e)
		{
			//Debug.Log("OnEndDrag");
			m_dragVector = Vector2.zero;
			m_vectorMagnitude = 0.0f;

			if (m_eventData == null || m_eventData.EndDragEvent == null)
			{
				return;
			}

			m_eventData.EndDragEvent.Invoke();

			if (m_beginObject != null)
			{
				m_beginObject.SetActive(false);
			}
			if (m_endObject != null)
			{
				m_endObject.SetActive(false);
			}
		}

		/// <summary>
		/// 押した時に呼ばれる
		/// </summary>
		/// <param name="e"></param>
		public void OnPointerDown(PointerEventData e)
		{
			//Debug.Log("OnPointerDown");
			if (m_isBeginLocalPosition == true)
			{
				m_beginVector = transform.position;
			}
			else
			{
				m_beginVector = e.position;
			}
			m_vectorMagnitude = 0.0f;
		}

		/// <summary>
		/// 離した時に呼ばれる
		/// </summary>
		/// <param name="e"></param>
		public void OnPointerUp(PointerEventData e)
		{
			//Debug.Log("OnPointerUp");
			m_beginVector = Vector2.zero;
			m_vectorMagnitude = 0.0f;

			if (m_eventData == null || m_eventData.EndDragEvent == null)
			{
				return;
			}

			m_eventData.EndDragEvent.Invoke();
		}

		/// <summary>
		/// クリックした時に呼ばれる
		/// </summary>
		/// <param name="e"></param>
		public void OnPointerClick(PointerEventData e)
		{
			if (m_eventData == null || m_eventData.EndDragEvent == null)
			{
				return;
			}

			m_eventData.ClickEvent.Invoke(e.position);
		}
	}
}