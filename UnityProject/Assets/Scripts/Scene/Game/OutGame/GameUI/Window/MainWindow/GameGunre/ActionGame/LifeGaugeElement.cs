using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.outgame.window.main.actiongame
{
	[System.Serializable]
	public class LifeGaugeElement : MonoBehaviour
	{
		[System.Serializable]
		public class Data
		{
			public enum State
			{
				NORMAL,
				WARNING,
				ERROR,
			}

			[SerializeField]
			private State m_nowState;
			public State NowState => m_nowState;

			public Data(State nowState)
			{
				m_nowState = nowState;
			}
		}

		[SerializeField]
		private UnityEngine.UI.Image m_gaugeImage;

		[SerializeField]
		private Sprite m_gaugeNormalSprite;

		[SerializeField]
		private Sprite m_gaugeWarningSprite;

		[SerializeField]
		private Sprite m_gaugeErrorSprite;



		public void Initialize()
		{
		}

		public void Setting(Data data)
		{
			if (data.NowState == Data.State.NORMAL)
			{
				m_gaugeImage.sprite = m_gaugeNormalSprite;
			}
			else if (data.NowState == Data.State.WARNING)
			{
				m_gaugeImage.sprite = m_gaugeWarningSprite;
			}
			else if (data.NowState == Data.State.ERROR)
			{
				m_gaugeImage.sprite = m_gaugeErrorSprite;
			}
		}
	}
}
