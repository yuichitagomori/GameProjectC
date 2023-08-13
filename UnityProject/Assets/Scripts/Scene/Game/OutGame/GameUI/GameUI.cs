using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

namespace scene.game.outgame
{
	[System.Serializable]
	public class GameUI : MonoBehaviour
	{

		[SerializeField]
		private CanvasGroup m_canvasGroup;

		[SerializeField]
		private window.WindowController m_windowController;

		[SerializeField]
		private Text m_mapNameText;

		[SerializeField]
		private Common.AnimatorExpansion m_mapNameAnime;




		public void Initialize(
			UnityAction<ingame.world.ActionTargetBase.Category, int> charaActionButtonEvent,
			UnityAction<Vector2> cameraBeginMoveEvent,
			UnityAction<Vector2> cameraMoveEvent,
			UnityAction cameraEndMoveEvent,
			UnityAction<Vector2> charaBeginMoveEvent,
			UnityAction<Vector2> charaMoveEvent,
			UnityAction charaEndMoveEvent,
			UnityAction<float> cameraZoomEvent)
		{
			m_windowController.Initialize(
				charaActionButtonEvent,
				cameraBeginMoveEvent,
				cameraMoveEvent,
				cameraEndMoveEvent,
				charaBeginMoveEvent,
				charaMoveEvent,
				charaEndMoveEvent,
				cameraZoomEvent);
		}

		public void Go()
		{
			m_windowController.Go();
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

		public void UpdateMapName(string mapName)
		{
			//m_mapNameText.text = string.Format("MapId:{0}", m_mapId);
		}

		public void OnMovieStart(string[] paramStrings, UnityAction callback)
		{
			switch (paramStrings[0])
			{
				default:
					{
						m_windowController.OnMovieStart(paramStrings, callback);
						break;
					}
			}
		}

        public void UpdateMainWindow(
            window.MainWindow.CharaActionButtonData actionButtonData,
            float weightParam)
        {
            m_windowController.UpdateMainWindow(actionButtonData, weightParam);
        }
    }
}
