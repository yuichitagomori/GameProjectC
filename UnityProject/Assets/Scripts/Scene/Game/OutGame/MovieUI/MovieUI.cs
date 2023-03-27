using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.outgame
{
    [System.Serializable]
    public class MovieUI : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup m_canvasGroup;

		/// <summary>
		/// �N�G�X�g�N���A���o�\�L�I�u�W�F�N�g
		/// </summary>
		[SerializeField]
		private GameObject m_questClearObject;

		/// <summary>
		/// �N�G�X�g�N���A���o�A�j���[�^�[
		/// </summary>
		[SerializeField]
		private Common.AnimatorExpansion m_questClearAnimator;

		/// <summary>
		/// �N�G�X�g�N���A���o�i�s�{�^��
		/// </summary>
		[SerializeField]
		private CommonUI.ButtonExpansion m_questClearTapButton;



		public void Initialize()
		{
			m_questClearObject.SetActive(false);
		}

		public void SetVisible(bool value)
		{
			if (value == true)
			{
				m_canvasGroup.alpha = 1.0f;
			}
			else
			{
				m_canvasGroup.alpha = 0.0f;
			}
		}

		public void PlayMovieQuestClearIn(int rewardItemId, UnityAction callback)
		{
			StartCoroutine(PlayMovieQuestClearInCoroutine(rewardItemId, callback));
		}

		public IEnumerator PlayMovieQuestClearInCoroutine(int rewardItemId, UnityAction callback)
		{
			m_questClearObject.SetActive(true);

			bool isDone = false;
			m_questClearAnimator.Play("In", () => { isDone = true; });
			while (!isDone) { yield return null; }

			isDone = false;
			m_questClearTapButton.SetupClickEvent(() => { isDone = true; });
			while (!isDone) { yield return null; }

			isDone = false;
			m_questClearAnimator.Play("In2", () => { isDone = true; });
			while (!isDone) { yield return null; }

			if (callback != null)
			{
				callback();
			}
		}
	}
}
