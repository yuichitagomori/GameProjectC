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
			
			var localSaveData = GeneralRoot.User.LocalSaveData;
			m_bgmSlider.value = localSaveData.BgmVolume;
			m_bgmSlider.onValueChanged.AddListener((_value) =>
			{
				SetupBgmVolume(_value);
			});

			m_seSlider.value = localSaveData.SEVolume;
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
			var localSaveData = GeneralRoot.User.LocalSaveData;
			localSaveData.UpdateBgmVolume(_volume);
		}

		private void SetupSEVolume(float _volume)
		{
			var localSaveData = GeneralRoot.User.LocalSaveData;
			localSaveData.UpdateSEVolume(_volume);
		}

		private void OnBackButtonPressed()
		{
			StartCoroutine(OnBackButtonPressedCoroutine());
		}

		private IEnumerator OnBackButtonPressedCoroutine()
		{
			bool isDone = false;
			GeneralRoot.User.Save(() => { isDone = true; });
			while (!isDone) { yield return null; }

			isDone = false;
			m_sceneController.RemoveScene(this, () => { isDone = true; });
			while (!isDone) { yield return null; }
		}
	}
}