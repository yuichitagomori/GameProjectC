using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// イベント拡張機能
/// </summary>
public class EventExpansion : MonoBehaviour,
	IPointerDownHandler,
	IPointerUpHandler,
	IBeginDragHandler,
	IDragHandler,
	IEndDragHandler
{
	public UnityAction<Vector2> DownEvent { private get; set; }
	public UnityAction<Vector2> UpEvent { private get; set; }
	public UnityAction<Vector2> BeginDragEvent { private get; set; }
	public UnityAction<Vector2> DragEvent { private get; set; }
	public UnityAction<Vector2> EndDragEvent { private get; set; }

	public void OnPointerDown(PointerEventData _e)
	{
		if (DownEvent != null)
		{
			DownEvent(_e.position);
		}
	}

	public void OnPointerUp(PointerEventData _e)
	{
		if (UpEvent != null)
		{
			UpEvent(_e.position);
		}
	}

	public void OnBeginDrag(PointerEventData _e)
	{
		if (BeginDragEvent != null)
		{
			BeginDragEvent(_e.position);
		}
	}

	public void OnDrag(PointerEventData _e)
	{
		if (DragEvent != null)
		{
			DragEvent(_e.position);
		}
	}

	public void OnEndDrag(PointerEventData _e)
	{
		if (EndDragEvent != null)
		{
			EndDragEvent(_e.position);
		}
	}
}
