using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene.dialog
{
	public class ShopDialog : SceneBase
	{
		public class Data
		{
		}

		/// <summary>
		/// アニメーター
		/// </summary>
		[SerializeField]
		private Common.AnimatorExpansion m_animator;



		/// <summary>
		/// Finish時
		/// </summary>
		private UnityAction m_finishCallback = null;

		private Data m_data = null;


		/// <summary>
		/// 事前設定
		/// </summary>
		/// <param name="data"></param>
		/// <param name="finishCallback"></param>
		public void Setting(Data data, UnityAction finishCallback)
		{
			m_data = data;
			m_finishCallback = finishCallback;
		}

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
		private IEnumerator ReadyCoroutine(UnityAction _callback)
		{
			bool isDone = false;
			m_animator.Play("In", () => { isDone = true; });
			while (!isDone) { yield return null; }

			if (_callback != null)
			{
				_callback();
			}
		}

		public override void Go()
		{
		}

		public override void Finish(UnityAction callback)
		{
			StartCoroutine(FinishCoroutine(callback));
		}

		private IEnumerator FinishCoroutine(UnityAction callback)
		{
			bool isDone = false;
			m_animator.Play("Out", () => { isDone = true; });
			while (!isDone) { yield return null; }

			if (m_finishCallback != null)
			{
				m_finishCallback();
			}

			if (callback != null)
			{
				callback();
			}
		}
	}
}