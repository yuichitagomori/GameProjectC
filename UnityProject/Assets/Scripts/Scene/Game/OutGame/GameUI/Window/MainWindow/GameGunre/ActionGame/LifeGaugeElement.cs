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
				Active,
				Inactive,
				ToActive,
				ToInactive,
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
		private Common.AnimatorExpansion m_animator;



		public void Initialize()
		{
		}

		public void Setting(Data data, UnityAction callback)
		{
			switch (data.NowState)
			{
				case Data.State.Active:
					{
						m_animator.Play("Active", callback);
						break;
					}
				case Data.State.Inactive:
					{
						m_animator.Play("Inactive", callback);
						break;
					}
				case Data.State.ToActive:
					{
						m_animator.Play("ToActive", callback);
						break;
					}
				case Data.State.ToInactive:
					{
						m_animator.Play("ToInactive", callback);
						break;
					}
			}
		}
	}
}
