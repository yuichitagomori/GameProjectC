﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace scene.game.outgame.window
{
    [System.Serializable]
    public class ResultWindow : WindowBase
    {
		[SerializeField]
		private CommonUI.TextExpansion m_titleText;

		[SerializeField]
		private Image m_logo;

		[SerializeField]
		private CommonUI.TextExpansion m_infoText;

		[SerializeField]
		private result.ResultReviewElement[] m_reviewElements;

		[SerializeField]
		private Common.AnimatorExpansion m_starsAnimation;
		
		[SerializeField]
		private Image[] m_stars;

		[SerializeField]
		private Color m_enableStarColor;

		[SerializeField]
		private Color m_disableStarColor;

		[SerializeField]
		private CommonUI.TextExpansion m_moneyText;

		[SerializeField]
		private CommonUI.ButtonExpansion m_yesButton;

		[SerializeField]
		private Sprite[] m_logoSprites;



		private int m_resultMoney = 0;

		public void Setting()
		{
			var local = GeneralRoot.User.LocalSaveData;
			var gameGunreMaster = GeneralRoot.Master.GameGunreData;
			var gameGunreMasterData = gameGunreMaster.Find(local.ChallengeGameGunreId);
			if (gameGunreMasterData == null)
			{
				return;
			}

			m_logo.sprite = m_logoSprites[local.ChallengeGameGunreId - 1];

			var temporary = GeneralRoot.User.LocalTemporaryData;
			int rewardMasterDataId = gameGunreMasterData.RewardDataId;
			if (temporary.OccurredBugId > -1)
			{
				var checkSheetBugMaster = GeneralRoot.Master.CheckSheetBugData;
				var checkSheetBugMasterData = checkSheetBugMaster.Find(temporary.OccurredBugId);
				if (checkSheetBugMasterData == null)
				{
					return;
				}
				rewardMasterDataId = checkSheetBugMasterData.RewardDataId;
			}
			var rewardMaster = GeneralRoot.Master.RewardData;
			var rewardMasterData = rewardMaster.Find(rewardMasterDataId);
			if (rewardMasterData == null)
			{
				return;
			}
			int totalStar = 0;
			for (int i = 0; i < m_reviewElements.Length; ++i)
			{
				int star = 0;
				int infoTextId = -1;
				switch (i)
				{
					case 0:
						{
							star = rewardMasterData.ReviewStar_1;
							infoTextId = rewardMasterData.ReviewTextId_1;
							break;
						}
					case 1:
						{
							star = rewardMasterData.ReviewStar_2;
							infoTextId = rewardMasterData.ReviewTextId_2;
							break;
						}
					case 2:
						{
							star = rewardMasterData.ReviewStar_3;
							infoTextId = rewardMasterData.ReviewTextId_3;
							break;
						}
				}
				m_reviewElements[i].Setting(
					star,
					m_enableStarColor,
					m_disableStarColor,
					infoTextId);
				totalStar += star;
			}

			int averageStar = (int)(totalStar / 3);
			for (int i = 0; i < m_stars.Length; ++i)
			{
				bool isEnable = (i < averageStar);
				m_stars[i].color = isEnable ? m_enableStarColor : m_disableStarColor;
			}
			m_starsAnimation.Play("Default");
			m_moneyText.text = "";
			m_resultMoney = rewardMasterData.Money;

			m_yesButton.gameObject.SetActive(false);
		}

		public override void Go()
		{
		}

		public override void OnMovieStart(string[] paramStrings, UnityAction callback)
		{
			StartCoroutine(OnMovieStartCoroutine(paramStrings, callback));
		}

		private IEnumerator OnMovieStartCoroutine(string[] paramStrings, UnityAction callback)
		{
			switch (paramStrings[0])
			{
				case "Phase1":
					{
						yield return OnMoviePhase1();
						break;
					}
				case "Phase2":
					{
						yield return OnMoviePhase2();
						break;
					}
				case "TopSibling":
					{
						SetTopSibling();
						break;
					}
			}

			if (callback != null)
			{
				callback();
			}
		}

		private IEnumerator OnMoviePhase1()
		{
			int doneCount = 0;
			for (int i = 0; i < m_reviewElements.Length; ++i)
			{
				m_reviewElements[i].Play(() => { doneCount++; });
				yield return new WaitForSeconds(0.5f);
			}
			while (doneCount < m_reviewElements.Length) { yield return null; }

			yield return new WaitForSeconds(1.0f);

			bool isDone = false;
			m_starsAnimation.Play("In", () => { isDone = true; });
			while (!isDone) { yield return null; }

			yield return new WaitForSeconds(1.0f);

			isDone = false;
			m_moneyText.PlayProgression(m_resultMoney.ToString("N0"), 0.2f, () => { isDone = true; });
			while (!isDone) { yield return null; }
		}

		private IEnumerator OnMoviePhase2()
		{
			yield return new WaitForSeconds(1.0f);

			m_yesButton.gameObject.SetActive(true);

			bool isDone = false;
			m_yesButton.SetupClickEvent(() => { isDone = true; });
			while (!isDone) { yield return null; }
		}

		public override void SetupInputKeyEvent()
		{
			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Down, KeyCode.Space, () =>
			{
				m_yesButton.OnDown();
			});
			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.Space, () =>
			{
				m_yesButton.OnUp();
				m_yesButton.OnClick();
			});
		}
	}
}