using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.outgame.window.main.actiongame
{
	[System.Serializable]
	public class ActionGameSequenceView : SequenceViewBase
	{
		[SerializeField]
		private Common.ElementList m_lifeGaugeElementList;



		private int nowLife = 0;

		public override void Setting()
		{
			m_animation.Play("Default");

			StartCoroutine(UpdateLifeGaugeCoroutine(0, 0));
		}

		public override IEnumerator OnMovieStartCoroutine(string[] paramStrings, UnityAction callback)
		{
			switch (paramStrings[0])
			{
				case "SequenceAnime":
					{
						string animationName = paramStrings[1];
						bool isDone = false;
						m_animation.Play(animationName, () => { isDone = true; });
						while (!isDone) { yield return null; }

						break;
					}
				case "UpdateLifeGauge":
					{
						int life = int.Parse(paramStrings[1]);
						int lifeMax = int.Parse(paramStrings[2]);
						yield return UpdateLifeGaugeCoroutine(life, lifeMax);
						break;
					}
				default:
					{
						break;
					}
			}

			if (callback != null)
			{
				callback();
			}
		}

		private IEnumerator UpdateLifeGaugeCoroutine(int value, int valueMax)
		{
			int doneCount = 0;
			var elements = m_lifeGaugeElementList.GetElements();
			for (int i = 0; i < elements.Count; ++i)
			{
				if (i >= valueMax)
				{
					elements[i].SetActive(false);
					doneCount++;
					continue;
				}

				elements[i].SetActive(true);
				LifeGaugeElement.Data.State state = LifeGaugeElement.Data.State.Active;
				if (i >= value)
				{
					if (i >= nowLife)
					{
						state = LifeGaugeElement.Data.State.Inactive;
					}
					else
					{
						state = LifeGaugeElement.Data.State.ToInactive;
					}
				}
				else
				{
					if (i < nowLife)
					{
						state = LifeGaugeElement.Data.State.Active;
					}
					else
					{
						state = LifeGaugeElement.Data.State.ToActive;
					}
				}
				var lifeGaugeElement = elements[i].GetComponent<LifeGaugeElement>();
				lifeGaugeElement.Setting(new LifeGaugeElement.Data(state), () => { doneCount++; });
			}

			while (doneCount < elements.Count) { yield return null; }
			nowLife = value;
		}
	}
}
