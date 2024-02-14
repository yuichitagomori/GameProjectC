using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.outgame.window.main
{
	[System.Serializable]
	public abstract class SequenceViewBase : MonoBehaviour
	{
		[SerializeField]
		protected Common.AnimatorExpansion m_animation;

		public abstract void Setting();

		public abstract IEnumerator SetupEventCoroutine(string[] paramStrings, UnityAction callback);
	}
}
