using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.outgame.window.main.puzzlegame
{
	[System.Serializable]
	public class TurnIconElement : MonoBehaviour
	{
		[SerializeField]
		private Common.AnimatorExpansion m_animation;

		[SerializeField]
		private UnityEngine.UI.Image m_iconImage;



		public void Initialize()
		{
			m_animation.Play("Default");
		}

		public void Setting(bool value, UnityAction callback)
		{
			StartCoroutine(SettingCoroutine(value, callback));
		}

		public IEnumerator SettingCoroutine(bool value, UnityAction callback)
		{
			string nowAnimationName = m_animation.GetAnimationName();
			if (value == true)
			{
				if (nowAnimationName != "In")
				{
					bool isDone = false;
					m_animation.Play("In", () => { isDone = true; });
					while (!isDone) { yield return null; }
				}
			}
			else
			{
				if (nowAnimationName == "In")
				{
					bool isDone = false;
					m_animation.Play("Out", () => { isDone = true; });
					while (!isDone) { yield return null; }
				}
			}

			if (callback != null)
			{
				callback();
			}
		}
	}
}
