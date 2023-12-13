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
		private Common.AnimatorExpansion m_sequenceAnimation;

		[SerializeField]
		private Common.AnimatorExpansion m_infoViewAnimation;

		[SerializeField]
		private CommonUI.ButtonExpansion m_changeInfoViewButton;

		[SerializeField]
		private CommonUI.ButtonExpansion m_powerButton;

		[SerializeField]
		private Common.ElementList m_lifeGaugeElementList;



		private List<KeyCode> m_downKeyList = new List<KeyCode>();

		private UnityAction<KeyCode[]> m_inputEvent;

		private bool m_isEnableInfoView;



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
			UnityAction powerButtonEvent,
			UnityAction<KeyCode[]> inputEvent)
		{
			m_sequenceAnimation.Play("Default");
			m_infoViewAnimation.Play("Default");
			m_changeInfoViewButton.SetupClickEvent(() =>
			{
				m_isEnableInfoView = !m_isEnableInfoView;
				string animationName = (m_isEnableInfoView == true) ? "In" : "Out";
				m_infoViewAnimation.Play(animationName);
			});
			m_powerButton.SetupClickEvent(powerButtonEvent);

			m_downKeyList.Clear();
			m_inputEvent = inputEvent;
			m_isEnableInfoView = false;

			UpdateLifeGauge(0);
		}

		public override void SetupEvent(string[] paramStrings, UnityAction callback)
		{
			StartCoroutine(SetupEventCoroutine(paramStrings, callback));
		}

		protected override void SetupInputKeyEvent()
		{
			if (GeneralRoot.Instance.IsPCPlatform() == false)
			{
				return;
			}

			var input = GeneralRoot.Instance.Input;
			UnityAction<KeyCode> setupKey = (key) =>
			{
				input.UpdateEvent(system.InputSystem.Type.Down, key, () =>
				{
					if (m_downKeyList.Contains(key) == false)
					{
						m_downKeyList.Add(key);
					}
				});
				input.UpdateEvent(system.InputSystem.Type.Up, key, () =>
				{
					if (m_downKeyList.Contains(key) == true)
					{
						m_downKeyList.Remove(key);
					}
				});
			};
			for (int i = 0; i < k_useKeys.Length; ++i)
			{
				var key = k_useKeys[i];
				if (key == KeyCode.W ||
					key == KeyCode.S ||
					key == KeyCode.A ||
					key == KeyCode.D)
				{
					setupKey(key);
				}
				else if (key == KeyCode.Space)
				{
					input.UpdateEvent(system.InputSystem.Type.Down, key, () =>
					{
						if (m_isEnableInfoView == true)
						{
							m_powerButton.OnDown();
						}
						else
						{
							if (m_downKeyList.Contains(key) == false)
							{
								m_downKeyList.Add(key);
							}
						}
					});
					input.UpdateEvent(system.InputSystem.Type.Up, key, () =>
					{
						if (m_isEnableInfoView == true)
						{
							m_powerButton.OnUp();
							m_powerButton.OnClick();
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
					input.UpdateEvent(system.InputSystem.Type.Down, key, () =>
					{
						m_changeInfoViewButton.OnDown();
					});
					input.UpdateEvent(system.InputSystem.Type.Up, key, () =>
					{
						m_changeInfoViewButton.OnUp();
						m_changeInfoViewButton.OnClick();
					});
				}
				else
				{
					input.UpdateEvent(system.InputSystem.Type.Down, key, null);
					input.UpdateEvent(system.InputSystem.Type.Up, key, null);
				}
			}
		}

		private IEnumerator SetupEventCoroutine(string[] paramStrings, UnityAction callback)
		{
			switch (paramStrings[0])
			{
				case "SequenceAnime":
					{
						string animationName = paramStrings[1];
						bool isDone = false;
						m_sequenceAnimation.Play(animationName, () => { isDone = true; });
						while (!isDone) { yield return null; }

						break;
					}
				case "UpdateLifeGauge":
					{
						int life = int.Parse(paramStrings[1]);
						UpdateLifeGauge(life);
						yield return null;

						break;
					}
				default:
					{
						break;
					}
			}

			if (callback != null)
			{
				callback();
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

        private void UpdateLifeGauge(int value)
		{
			var elements = m_lifeGaugeElementList.GetElements();
			for (int i = 0; i < elements.Count; ++i)
			{
				if (i >= value)
				{
					elements[i].SetActive(false);
					continue;
				}
				elements[i].SetActive(true);
				var lifeGaugeElement = elements[i].GetComponent<main.LifeGaugeElement>();
				lifeGaugeElement.Setting(new main.LifeGaugeElement.Data(main.LifeGaugeElement.Data.State.NORMAL));
			}
		}
    }
}
