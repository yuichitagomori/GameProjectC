using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene.game.outgame.window
{
    [System.Serializable]
    public class WindowHoldArea : MonoBehaviour
    {
        [SerializeField]
        private Handler m_hander;



        private Transform m_windowTransform;

        private Vector2 m_windowSize;

        private Vector3[] m_areaWorldCorner;

        private Vector3 m_beginDragOffset = Vector2.zero;

        private UnityAction m_holdCallback;

        public void Initialize(
            Transform windowTransform,
            Vector2 windowSize,
            RectTransform windowArea,
            UnityAction holdCallback)
		{
            m_windowTransform = windowTransform;
            m_windowSize = new Vector2(windowSize.x * 1.5f, windowSize.y * 1.5f);
            m_areaWorldCorner = new Vector3[4];
            windowArea.GetWorldCorners(m_areaWorldCorner);
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
            Vector3 setPosition = new Vector3(v.x, v.y, 0.0f) + m_beginDragOffset;
            SetupWindowPosition(setPosition);
        }

        private void OnEndDrag()
        {
        }

		public void OnMove(Vector2 moveV)
		{
            SetupWindowPosition(m_windowTransform.position + new Vector3(moveV.x, moveV.y, 0.0f));
        }

        private void SetupWindowPosition(Vector2 position)
		{
            float halfx = m_windowSize.x * 0.5f;
            float halfy = m_windowSize.y * 0.5f;
            if (m_areaWorldCorner[0].x > (position.x - halfx) ||   // 左下
                m_areaWorldCorner[0].y > (position.y - halfy) ||
                m_areaWorldCorner[1].x > (position.x - halfx) ||   // 左上
                m_areaWorldCorner[1].y < (position.y + halfy) ||
                m_areaWorldCorner[2].x < (position.x + halfx) ||   // 右上
                m_areaWorldCorner[2].y < (position.y + halfy) ||
                m_areaWorldCorner[3].x < (position.x + halfx) ||   // 右下
                m_areaWorldCorner[3].y > (position.y - halfy))
            {
                if ((position.x - halfx) < m_areaWorldCorner[0].x)
                {
                    position.x = (m_areaWorldCorner[0].x + halfx);
                }
                else if ((position.x + halfx) > m_areaWorldCorner[2].x)
                {
                    position.x = (m_areaWorldCorner[2].x - halfx);
                }
                if ((position.y - halfy) < m_areaWorldCorner[3].y)
                {
                    position.y = (m_areaWorldCorner[3].y + halfy);
                }
                else if ((position.y + halfy) > m_areaWorldCorner[1].y)
                {
                    position.y = (m_areaWorldCorner[1].y - halfy);
                }
            }

            m_windowTransform.position = position;
        }
    }
}