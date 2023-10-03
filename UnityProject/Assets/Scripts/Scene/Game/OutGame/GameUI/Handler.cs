using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace scene.game.outgame
{
	/// <summary>
	/// ハンドルイベント受け取りクラス
	/// </summary>
	[System.Serializable]
	public class Handler :
		MonoBehaviour,
		IBeginDragHandler,
		IDragHandler,
		IEndDragHandler,
		IPointerDownHandler,
		IPointerUpHandler,
		IPointerClickHandler
	{
		public class EventData
		{
			/// <summary>
			/// ドラッグ開始時イベント
			/// </summary>
			private UnityAction<Vector2> m_beginDragEvent = null;
			public UnityAction<Vector2> BeginDragEvent => m_beginDragEvent;

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

			///// <summary>
			///// マウスが乗った時のイベント
			///// </summary>
			//private UnityAction<Vector2> m_mouseEnterEvent = null;
			//public UnityAction<Vector2> MouseEnterEvent => m_mouseEnterEvent;

			///// <summary>
			///// マウスが外れた時のイベント
			///// </summary>
			//private UnityAction m_mouseExitEvent = null;
			//public UnityAction MouseExitEvent => m_mouseExitEvent;

			public EventData(
				UnityAction<Vector2> beginDragEvent,
				UnityAction<Vector2> dragEvent,
				UnityAction endDragEvent,
				UnityAction<Vector2> clickEvent)
			{
				m_beginDragEvent = beginDragEvent;
				m_dragEvent = dragEvent;
				m_endDragEvent = endDragEvent;
				m_clickEvent = clickEvent;
			}

			//public EventData(
			//	UnityAction<Vector2> dragEvent,
			//	UnityAction endDragEvent,
			//	UnityAction<Vector2> clickEvent,
			//	UnityAction<Vector2> mouseEnterEvent,
			//	UnityAction mouseExitEvent)
			//{
			//	m_dragEvent = dragEvent;
			//	m_endDragEvent = endDragEvent;
			//	m_clickEvent = clickEvent;
			//	m_mouseEnterEvent = mouseEnterEvent;
			//	m_mouseExitEvent = mouseExitEvent;
			//}
		}

		[SerializeField]
		/// <summary>
		/// ドラッグ判定とする為の最低の長さ
		/// </summary>
		private float m_dragLength = 0.0f;

		/// <summary>
		/// イベントデータ
		/// </summary>
		private EventData m_eventData = null;

		private Vector2 m_beginPosition = Vector2.zero;

		private Vector2 m_dragPosition = Vector2.zero;



		private IEnumerator UpdateCoroutine()
		{
			if (m_eventData == null || m_eventData.DragEvent == null)
			{
				yield break;
			}

			while (true)
			{
				if (m_beginPosition == Vector2.zero ||
					m_dragPosition == Vector2.zero)
				{
					yield return null;
					continue;
				}

				Vector2 dir = m_beginPosition - m_dragPosition;
				if (dir.magnitude >= m_dragLength)
				{
					m_eventData.DragEvent(m_dragPosition);
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

			StartCoroutine(UpdateCoroutine());
		}

		/// <summary>
		/// ドラックが開始したとき呼ばれる
		/// </summary>
		/// <param name="e"></param>
		public void OnBeginDrag(PointerEventData e)
		{
			//Debug.Log("OnBeginDrag e.position = " + e.position);

			if (m_eventData == null || m_eventData.BeginDragEvent == null)
			{
				return;
			}

			m_eventData.BeginDragEvent(e.position);

			m_beginPosition = e.position;
		}

		/// <summary>
		/// ドラック中に呼ばれる
		/// </summary>
		/// <param name="e"></param>
		public void OnDrag(PointerEventData e)
		{
			//Debug.Log("OnDrag e.position = " + e.position);

			m_dragPosition = e.position;
		}

		/// <summary>
		/// ドラックが終了したとき呼ばれる
		/// </summary>
		/// <param name="e"></param>
		public void OnEndDrag(PointerEventData e)
		{
			//Debug.Log("OnEndDrag");

			if (m_eventData == null || m_eventData.EndDragEvent == null)
			{
				return;
			}

			m_eventData.EndDragEvent();

			m_beginPosition = Vector2.zero;
			m_dragPosition = Vector2.zero;
		}

		/// <summary>
		/// 押した時に呼ばれる
		/// </summary>
		/// <param name="e"></param>
		public void OnPointerDown(PointerEventData e)
		{
			//Debug.Log("OnPointerDown");

			if (m_eventData == null || m_eventData.BeginDragEvent == null)
			{
				return;
			}

			m_eventData.BeginDragEvent(e.position);
		}

		/// <summary>
		/// 離した時に呼ばれる
		/// </summary>
		/// <param name="e"></param>
		public void OnPointerUp(PointerEventData e)
		{
			//Debug.Log("OnPointerUp");

			if (m_eventData == null || m_eventData.EndDragEvent == null)
			{
				return;
			}

			m_eventData.EndDragEvent();
		}

		/// <summary>
		/// クリックした時に呼ばれる
		/// </summary>
		/// <param name="e"></param>
		public void OnPointerClick(PointerEventData e)
		{
			if (m_eventData == null || m_eventData.ClickEvent == null)
			{
				return;
			}

			if (m_dragPosition != Vector2.zero)
			{
				// ドラッグ操作でマウスボタンがアップしたので無効
				return;
			}

			m_eventData.ClickEvent(e.position);
		}

		///// <summary>
		///// マウスが乗った時に呼ばれる
		///// </summary>
		//public void OnMouseEnter()
		//{
		//	if (m_eventData == null || m_eventData.MouseEnterEvent == null)
		//	{
		//		return;
		//	}

		//	m_eventData.MouseEnterEvent(Input.mousePosition);
		//}

		///// <summary>
		///// マウスが乗った時に呼ばれる
		///// </summary>
		//public void OnMouseExit()
		//{
		//	if (m_eventData == null || m_eventData.MouseExitEvent == null)
		//	{
		//		return;
		//	}

		//	m_eventData.MouseExitEvent();
		//}
	}
}