using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.outgame.window
{
    [System.Serializable]
    public class MainWindow : WindowBase
	{
		[SerializeField]
		private Common.AnimatorExpansion m_infoViewAnimation;

		[SerializeField]
		private CommonUI.ButtonExpansion m_changeInfoViewButton;

		[SerializeField]
		private CommonUI.ButtonExpansion m_powerButton;

		[SerializeField]
		private CommonUI.ButtonExpansion m_recreateButton;

		[SerializeField]
		private CommonUI.ButtonExpansion m_releaseButton;

		[SerializeField]
		private GameObject[] m_infoMenuCursols;

		[SerializeField]
		private Transform m_sequenceViewParent;

		[SerializeField]
		private GameObject[] m_sequenceViewPrefabs;



		private List<KeyCode> m_downKeyList = new List<KeyCode>();

		private UnityAction<KeyCode[]> m_inputEvent;

		private bool m_isEnableInfoView;

		private int m_infoMenuSelectIndex;

		private main.SequenceViewBase m_sequenceView;



		public override void Go()
		{
			StartCoroutine(GoCoroutine());
		}

		private IEnumerator GoCoroutine()
		{
			while (true)
			{
				// PC設定
				if (GeneralRoot.Instance.IsPCPlatform() == true)
				{
					if (m_isActiveWindow == false || m_isSelectWindow == false)
					{
						yield return null;
						continue;
					}

					m_inputEvent(m_downKeyList.ToArray());
				}
				
				yield return null;
			}
		}

		public void Setting(
			Game.GenreType genreType,
			UnityAction powerButtonEvent,
			UnityAction recreateButtonEvent,
			UnityAction releaseButtonEvent,
			UnityAction<KeyCode[]> inputEvent)
		{
			var prefab = m_sequenceViewPrefabs[(int)genreType];
			var sequenceViewObject = GameObject.Instantiate(prefab, m_sequenceViewParent);
			m_sequenceView = sequenceViewObject.GetComponent<main.SequenceViewBase>();
			m_sequenceView.Setting();

			m_infoViewAnimation.Play("Default");
			m_changeInfoViewButton.SetupClickEvent(() =>
			{
				m_isEnableInfoView = !m_isEnableInfoView;
				string animationName = (m_isEnableInfoView == true) ? "In" : "Out";
				m_infoViewAnimation.Play(animationName);

				if (m_infoMenuSelectIndex != 0)
				{
					m_infoMenuSelectIndex = 0;
					UpdateInfoMenuCursols();
				}
			});
			m_powerButton.SetupClickEvent(powerButtonEvent);
			m_recreateButton.SetupClickEvent(recreateButtonEvent);
			m_releaseButton.SetupClickEvent(releaseButtonEvent);

			m_downKeyList.Clear();
			m_inputEvent = inputEvent;
			m_isEnableInfoView = false;
			m_infoMenuSelectIndex = 2;
			UpdateInfoMenuCursols();
		}

		public override void SetupEvent(string[] paramStrings, UnityAction callback)
		{
			StartCoroutine(m_sequenceView.SetupEventCoroutine(paramStrings, callback));
		}

		protected override void SetupInputKeyEvent()
		{
			if (GeneralRoot.Instance.IsPCPlatform() == false)
			{
				return;
			}

			for (int i = 0; i < k_useKeys.Length; ++i)
			{
				var key = k_useKeys[i];
				if (key == KeyCode.W)
				{
					GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Down, key, () =>
					{
						if (m_isEnableInfoView == true)
						{

						}
						else
						{
							if (m_downKeyList.Contains(key) == false)
							{
								m_downKeyList.Add(key);
							}
						}
					});
					GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, key, () =>
					{
						if (m_isEnableInfoView == true)
						{
							if (m_infoMenuSelectIndex < 2)
							{
								m_infoMenuSelectIndex++;
								UpdateInfoMenuCursols();
							}
						}
						else
						{
							if (m_downKeyList.Contains(key) == true)
							{
								m_downKeyList.Remove(key);
							}
						}
					});
				}
				else if (key == KeyCode.S)
				{
					GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Down, key, () =>
					{
						if (m_isEnableInfoView == true)
						{

						}
						else
						{
							if (m_downKeyList.Contains(key) == false)
							{
								m_downKeyList.Add(key);
							}
						}
					});
					GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, key, () =>
					{
						if (m_isEnableInfoView == true)
						{
							if (m_infoMenuSelectIndex > 0)
							{
								m_infoMenuSelectIndex--;
								UpdateInfoMenuCursols();
							}
						}
						else
						{
							if (m_downKeyList.Contains(key) == true)
							{
								m_downKeyList.Remove(key);
							}
						}
					});
				}
				else if (key == KeyCode.A || key == KeyCode.D)
				{
					GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Down, key, () =>
					{
						if (m_isEnableInfoView == true)
						{

						}
						else
						{
							if (m_downKeyList.Contains(key) == false)
							{
								m_downKeyList.Add(key);
							}
						}
					});
					GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, key, () =>
					{
						if (m_isEnableInfoView == true)
						{

						}
						else
						{
							if (m_downKeyList.Contains(key) == true)
							{
								m_downKeyList.Remove(key);
							}
						}
					});
				}
				else if (key == KeyCode.Space)
				{
					GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Down, key, () =>
					{
						if (m_isEnableInfoView == true)
						{
							if (m_infoMenuSelectIndex == 0)
							{
								m_powerButton.OnDown();
							}
							else if (m_infoMenuSelectIndex == 1)
							{
								m_recreateButton.OnDown();
							}
							else if (m_infoMenuSelectIndex == 2)
							{
								m_releaseButton.OnDown();
							}
						}
						else
						{
							if (m_downKeyList.Contains(key) == false)
							{
								m_downKeyList.Add(key);
							}
						}
					});
					GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, key, () =>
					{
						if (m_isEnableInfoView == true)
						{
							if (m_infoMenuSelectIndex == 0)
							{
								m_powerButton.OnUp();
								m_powerButton.OnClick();
							}
							else if (m_infoMenuSelectIndex == 1)
							{
								m_recreateButton.OnUp();
								m_recreateButton.OnClick();
							}
							else if (m_infoMenuSelectIndex == 2)
							{
								m_releaseButton.OnUp();
								m_releaseButton.OnClick();
							}
						}
						else
						{
							if (m_downKeyList.Contains(key) == true)
							{
								m_downKeyList.Remove(key);
							}
						}
					});
				}
				else if (key == KeyCode.X)
				{
					GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Down, key, () =>
					{
						m_changeInfoViewButton.OnDown();
					});
					GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, key, () =>
					{
						m_changeInfoViewButton.OnUp();
						m_changeInfoViewButton.OnClick();
					});
				}
				else
				{
					GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Down, key, null);
					GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, key, null);
				}
			}
		}

		private void UpdateInfoMenuCursols()
		{
			for (int i = 0; i < m_infoMenuCursols.Length; ++i)
			{
				m_infoMenuCursols[i].SetActive(i == m_infoMenuSelectIndex);
			}
		}

		//private void CameraBeginDragEvent(Vector2 position)
		//{
		//	if (m_isActiveWindow == false || m_isSelectWindow == false)
		//	{
		//		return;
		//	}
		//	m_cameraBeginMoveEvent(position);
		//}

		//private void CameraDragEvent(Vector2 position)
		//{
		//	if (m_isActiveWindow == false || m_isSelectWindow == false)
		//	{
		//		return;
		//	}
		//	m_cameraMoveEvent(position);
		//}

		//private void CameraEndDragEvent()
		//{
		//	if (m_isActiveWindow == false || m_isSelectWindow == false)
		//	{
		//		return;
		//	}
		//	m_cameraEndMoveEvent();
		//}

		//private void ClickEvent(Vector2 position)
		//{
		//	if (m_isActiveWindow == false || m_isSelectWindow == false)
		//	{
		//		return;
		//	}

		//	// レンダリング画像サイズに座標を修正
		//	// (RenderTextureを使用している場合はViewport座標に変換して使用する必要がある)
		//	position.x -= 1920.0f * 0.5f;
		//	position.y -= 1080.0f * 0.5f;
		//	position.x *= (1280.0f / 900.0f);
		//	position.y *= (720.0f / 506.25f);
		//	position.x += 1920.0f * 0.5f;
		//	position.y += 1080.0f * 0.5f;
		//	position.x -= m_windowTransform.localPosition.x * (1920.0f / 900.0f);
		//	position.y -= m_windowTransform.localPosition.y * (1080.0f / 506.25f);
		//	m_clickEvent(new Vector2(position.x / 1920.0f, position.y / 1080.0f));
		//}
	}
}
