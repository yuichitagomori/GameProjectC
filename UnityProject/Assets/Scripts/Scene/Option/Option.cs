using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene
{
	public class Option : SceneBase
	{
		[SerializeField]
		private CommonUI.ButtonExpansion m_backButton;

		[SerializeField]
		private Slider m_bgmSlider;

		[SerializeField]
		private Slider m_seSlider;

		[SerializeField]
		private CommonUI.ButtonExpansion m_webButton;

		[SerializeField]
		private CommonUI.ButtonExpansion m_purchaseButton;

		[SerializeField]
		private CommonUI.ButtonExpansion m_restoreButton;



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
			m_backButton.SetupClickEvent(OnBackButtonPressed);
			
			var userData = GeneralRoot.Instance.UserData.Data;
			m_bgmSlider.value = userData.BgmVolume;
			m_bgmSlider.onValueChanged.AddListener((_value) =>
			{
				SetupBgmVolume(_value);
			});

			m_seSlider.value = userData.SEVolume;
			m_seSlider.onValueChanged.AddListener((_value) =>
			{
				SetupSEVolume(_value);
			});

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

		private void SetupBgmVolume(float _volume)
		{
			var userData = GeneralRoot.Instance.UserData.Data;
			userData.SetBgmVolume(_volume);
		}

		private void SetupSEVolume(float _volume)
		{
			var userData = GeneralRoot.Instance.UserData.Data;
			userData.SetSEVolume(_volume);
		}

		private void OnBackButtonPressed()
		{
			StartCoroutine(OnBackButtonPressedCoroutine());
		}

		private IEnumerator OnBackButtonPressedCoroutine()
		{
			bool isDone = false;
			GeneralRoot.Instance.UserData.Save(() => { isDone = true; });
			while (!isDone) { yield return null; }

			isDone = false;
			GeneralRoot.Instance.SceneController.RemoveScene(this, () => { isDone = true; });
			while (!isDone) { yield return null; }
		}
	}
}