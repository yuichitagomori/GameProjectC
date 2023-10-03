using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

namespace scene.game.ingame
{
	public class ActionGame : GameGenreBase
	{
		[SerializeField]
		private Transform[] m_cameraAngles;

		[SerializeField]
		private EventBase[] m_mapEvents;

		[SerializeField]
		private actiongame.ActionGamePlayerChara m_player;

		[SerializeField]
		private actiongame.TransferObject[] m_transfers;



		private bool m_isEnableControll = false;

		private KeyCode[] m_beforePressKeys;

		public override void Initialize()
		{
			m_state = State.None;
			m_outgameSetupEvent("Main,SequenceAnime,Default", null);

			m_cameraTransform.localPosition = m_cameraAngles[0].localPosition;
			m_cameraTransform.localRotation = m_cameraAngles[0].localRotation;

			for (int i = 0; i < m_mapEvents.Length; ++i)
			{
				m_mapEvents[i].Initialize(OnMapEvent);
			}
			m_player.Initialize();
			for (int i = 0; i < m_transfers.Length; ++i)
			{
				m_transfers[i].Initialize(OnTransferEvent);
			}
			m_isEnableControll = false;
			m_beforePressKeys = null;
		}

		public override void Go()
		{
			StartCoroutine(GoCoroutine());
		}

		private IEnumerator GoCoroutine()
		{
			bool isDone = false;
			m_outgameSetupEvent("Main,SequenceAnime,TitleIn", () => { isDone = true; });
			while (!isDone) { yield return null; }

			m_state = State.Title;
			while (m_state == State.Title) { yield return null; }

			isDone = false;
			m_outgameSetupEvent("Main,SequenceAnime,TitleOut", () => { isDone = true; });
			while (!isDone) { yield return null; }

			isDone = false;
			m_player.SequenceIn(2.0f, () => { isDone = true; });
			while (!isDone) { yield return null; }

			Vector3 beforePosition = m_cameraAngles[0].localPosition;
			Quaternion beforeRotation = m_cameraAngles[0].localRotation;
			Vector3 afterPosition = m_cameraAngles[1].localPosition;
			Quaternion afterRotation = m_cameraAngles[1].localRotation;
			yield return CommonMath.TransformLerpCoroutine(
				m_cameraTransform,
				beforePosition,
				beforeRotation,
				afterPosition,
				afterRotation,
				1.0f,
				null);

			m_isEnableControll = true;

			isDone = false;
			m_outgameSetupEvent("Main,SequenceAnime,GameIn", () => { isDone = true; });
			while (!isDone) { yield return null; }
		}

		private void FixedUpdate()
		{
			if (m_player.transform == null)
			{
				return;
			}

			// カメラ座標更新は、FixedUpdateで呼ばないとガタガタする
			Vector3 setPosition = Vector3.Lerp(m_cameraParentTransform.position, m_player.transform.position, 0.2f);
			m_cameraParentTransform.position = setPosition;
		}

		public override void OnInputEvent(KeyCode[] pressKeys)
		{
			if (m_state == State.None)
			{
				return;
			}
			else if (m_state == State.Title)
			{
				if (pressKeys.Contains(KeyCode.Space) || pressKeys.Contains(KeyCode.Return))
				{
					m_state = State.Game;
				}
			}
			else
			{
				if (m_isEnableControll == false)
				{
					return;
				}

				if (pressKeys.Contains(KeyCode.A))
				{
					m_player.Move(new Vector3(1.0f, 0.0f, 0.0f));
				}
				else if (pressKeys.Contains(KeyCode.D))
				{
					m_player.Move(new Vector3(-1.0f, 0.0f, 0.0f));
				}

				if (pressKeys.Contains(KeyCode.W) && m_beforePressKeys != null & !m_beforePressKeys.Contains(KeyCode.W))
				{
					m_player.Jump();
				}
			}

			m_beforePressKeys = pressKeys;
		}

		//public override void OnSetupEvent(string param)
		//{
		//	switch (param)
		//	{

		//	}
		//}

		private void OnMapEvent(string param)
		{
			switch (param)
			{
				case "Delete":
					{
						OnTransferEvent(0);
						break;
					}
			}
		}

		private void OnTransferEvent(int index)
		{
			StartCoroutine(TransferCoroutine(index));
		}

		private IEnumerator TransferCoroutine(int index)
		{
			m_isEnableControll = false;

			bool isDone = false;
			m_player.SequenceOut(0.5f, () => { isDone = true; });
			while (!isDone) { yield return null; }

			yield return new WaitForSeconds(0.5f);
			m_player.transform.position = m_transfers[index].transform.position;
			yield return new WaitForSeconds(0.5f);

			isDone = false;
			m_player.SequenceIn(1.0f, () => { isDone = true; });
			while (!isDone) { yield return null; }

			m_isEnableControll = true;
		}
	}
}