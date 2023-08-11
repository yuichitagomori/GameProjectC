using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.outgame.window
{
    [System.Serializable]
    public class WindowHoldArea : MonoBehaviour
    {
        [SerializeField]
        private Handler m_hander;




        private Transform m_windowTransform;

        private Vector3 m_beginDragOffset = Vector2.zero;

        private UnityAction m_holdCallback;

        public void Initialize(Transform windowTransform, UnityAction holdCallback)
		{
            m_windowTransform = windowTransform;
            m_holdCallback = holdCallback;

            m_hander.Initialize(new Handler.EventData(
                beginDragEvent: OnBeginDrag,
                dragEvent: OnDrag,
                endDragEvent: OnEndDrag,
                clickEvent: null));
		}
        private void OnBeginDrag(Vector2 v)
        {
            m_beginDragOffset = m_windowTransform.position - new Vector3(v.x, v.y, 0.0f);

            if (m_holdCallback != null)
			{
                m_holdCallback();
			}
        }

        private void OnDrag(Vector2 v)
		{
            m_windowTransform.position = new Vector3(v.x, v.y, 0.0f) + m_beginDragOffset;
        }

        private void OnEndDrag()
        {
        }
    }
}