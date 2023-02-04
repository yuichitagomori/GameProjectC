using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene
{
	public class TestScene : SceneBase
	{
		public override void Ready(UnityAction _callback)
		{
			StartCoroutine(ReadyCoroutine(_callback));
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
			yield return null;
		}
	}
}