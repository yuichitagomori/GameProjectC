using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene.game.outgame.window.result
{
	[System.Serializable]
	public class ResultReviewElement : MonoBehaviour
	{
		[SerializeField]
		private Common.AnimatorExpansion m_animation;

		[SerializeField]
		private Image m_icon;

		[SerializeField]
		private Common.AnimatorExpansion m_starsAnimation;

		[SerializeField]
		private Image[] m_stars;

		[SerializeField]
		private CommonUI.TextExpansion m_infoText;



		private int m_infoId;

		public void Setting(int starValue, Color enableStarColor, Color disableStarColor, int infoId)
		{
			for (int i = 0; i < m_stars.Length; ++i)
			{
				bool isEnable = (i < starValue);
				m_stars[i].color = isEnable ? enableStarColor : disableStarColor;
			}
			m_infoText.text = "";
			m_animation.Play("Default");
			m_starsAnimation.Play("Default");

			m_infoId = infoId;
		}

		public void Play(UnityAction callback)
		{
			StartCoroutine(PlayCoroutine(callback));
		}

		private IEnumerator PlayCoroutine(UnityAction callback)
		{
			bool isDone = false;
			m_animation.Play("In", () => { isDone = true; });
			while (!isDone) { yield return null; }

			isDone = false;
			m_starsAnimation.Play("In", () => { isDone = true; });
			while (!isDone) { yield return null; }

			isDone = false;
			string infoString = "とても面白かったです！\n次の作品も気になります！";
			m_infoText.PlayProgression(infoString, 0.05f, () => { isDone = true; });
			while (!isDone) { yield return null; }

			if (callback != null)
			{
				callback();
			}
		}
	}
}
