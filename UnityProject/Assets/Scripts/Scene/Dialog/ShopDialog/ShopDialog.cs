using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene.dialog
{
	public class ShopDialog : SceneBase
	{
		/// <summary>
		/// アニメーター
		/// </summary>
		[SerializeField]
		private AnimatorExpansion m_animator;


		public override void Ready(UnityAction _callback)
		{
			StartCoroutine(ReadyCoroutine(() =>
			{
				if (_callback != null)
				{
					_callback();
				}
			}));
		}

		public override void Go()
		{
			StartCoroutine(GoCoroutine());
		}

		private IEnumerator ReadyCoroutine(UnityAction _callback)
		{
			yield return null;

			if (_callback != null)
			{
				_callback();
			}
		}

		private IEnumerator GoCoroutine()
		{
			//bool isDone = false;
			//m_animator.Play("Open", () => { isDone = true; });
			//while (!isDone) { yield return null; }

			yield return null;
		}
	}
}