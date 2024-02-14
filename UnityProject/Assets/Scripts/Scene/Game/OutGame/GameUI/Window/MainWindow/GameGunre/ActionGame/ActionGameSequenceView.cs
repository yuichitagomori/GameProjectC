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


		public override void Setting()
		{
			m_animation.Play("Default");

			UpdateLifeGauge(0);
		}

		public override IEnumerator SetupEventCoroutine(string[] paramStrings, UnityAction callback)
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
						UpdateLifeGauge(life);
						yield return null;

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

		private void UpdateLifeGauge(int value)
		{
			var elements = m_lifeGaugeElementList.GetElements();
			for (int i = 0; i < elements.Count; ++i)
			{
				if (i >= value)
				{
					elements[i].SetActive(false);
					continue;
				}
				elements[i].SetActive(true);
				var lifeGaugeElement = elements[i].GetComponent<LifeGaugeElement>();
				lifeGaugeElement.Setting(new LifeGaugeElement.Data(LifeGaugeElement.Data.State.NORMAL));
			}
		}
	}
}
