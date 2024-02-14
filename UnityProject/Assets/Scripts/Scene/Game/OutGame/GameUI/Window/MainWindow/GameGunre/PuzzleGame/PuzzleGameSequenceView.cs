using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.outgame.window.main.puzzlegame
{
	[System.Serializable]
	public class PuzzleGameSequenceView : SequenceViewBase
	{
		[SerializeField]
		private Common.ElementList m_turnIconElementList;



		public override void Setting()
		{
			m_animation.Play("Default");

			var elements = m_turnIconElementList.GetElements();
			for (int i = 0; i < elements.Count; ++i)
			{
				var turnIconElement = elements[i].GetComponent<TurnIconElement>();
				turnIconElement.Initialize();
			}
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
				case "UpdateTurn":
					{
						int turn = int.Parse(paramStrings[1]);
						yield return UpdateTurnCoroutine(turn);

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

		private IEnumerator UpdateTurnCoroutine(int value)
		{
			var elements = m_turnIconElementList.GetElements();
			int doneCount = 0;
			for (int i = 0; i < elements.Count; ++i)
			{
				var turnIconElement = elements[i].GetComponent<TurnIconElement>();
				turnIconElement.Setting(i < value, () => { doneCount++; });
			}

			while (doneCount < elements.Count) { yield return null; }
		}
	}
}
