using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scene.game.outgame
{
    [System.Serializable]
    public class MovieUI : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup m_canvasGroup = null;



		public void Initialize()
		{

		}

		public void SetVisible(bool value)
		{
			if (value == true)
			{
				m_canvasGroup.alpha = 1.0f;
			}
			else
			{
				m_canvasGroup.alpha = 0.0f;
			}
		}
	}
}
