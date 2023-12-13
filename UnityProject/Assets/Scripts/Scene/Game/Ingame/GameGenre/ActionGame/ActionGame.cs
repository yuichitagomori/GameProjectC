using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene.game.ingame
{
	public class ActionGame : GameGenreBase
	{
		private string SequenceAnimeStringFormat = "Main,SequenceAnime,{0}";
		private string UpdateInfoViewStringFormat = "Main,UpdateInfoView,{0}";
		private string UpdateLifeGameStringFormat = "Main,UpdateLifeGauge,{0}";
		private string CharaReactionStringFormat = "Chara,Play,{0}";


		[SerializeField]
		private Transform[] m_cameraAngles;

		[SerializeField]
		private EventBase[] m_mapEvents;

		[SerializeField]
		private actiongame.ActionGamePlayerChara m_player;

		[SerializeField]
		private GameObject[] m_mapColliders;

		[SerializeField]
		private actiongame.TransferObject[] m_transfers;

		[SerializeField]
		private actiongame.GuideObject[] m_guides;



		private bool m_isEnableControll = false;

		private KeyCode[] m_beforePressKeys;

		private int m_life;

		private List<int> m_reactionBugIdList = new List<int>();



		public override void Initialize()
		{
			m_state = State.None;

			m_cameraTransform.localPosition = m_cameraAngles[0].localPosition;
			m_cameraTransform.localRotation = m_cameraAngles[0].localRotation;

			for (int i = 0; i < m_mapEvents.Length; ++i)
			{
				m_mapEvents[i].Initialize(OnMapEvent);
			}
			m_player.Initialize(BugEvent);
			for (int i = 0; i < m_transfers.Length; ++i)
			{
				m_transfers[i].Initialize(OnTransferEvent);
			}
			for (int i = 0; i < m_guides.Length; ++i)
			{
				m_guides[i].Initialize();
			}
			m_isEnableControll = false;
			m_beforePressKeys = null;
			m_life = 4;

			var local = GeneralRoot.User.LocalSaveData;
			if (local.OccurredBugIds.Contains(4))
			{
				// 一部の足場用コライダー削除
				m_mapColliders[2].SetActive(false);
			}
			if (local.OccurredBugIds.Contains(5))
			{
				// 落下用コライダー削除
				m_mapColliders[0].SetActive(false);
			}
		}

		public override void Go()
		{
			StartCoroutine(GoCoroutine());
		}

		private IEnumerator GoCoroutine()
		{
			bool isDone = false;
			string paramString = string.Format(SequenceAnimeStringFormat, "TitleIn");
			m_outgameSetupEvent(paramString, () => { isDone = true; });
			while (!isDone) { yield return null; }

			m_state = State.Title;
			while (m_state == State.Title) { yield return null; }

			isDone = false;
			paramString = string.Format(SequenceAnimeStringFormat, "TitleOut");
			m_outgameSetupEvent(paramString, () => { isDone = true; });
			while (!isDone) { yield return null; }

			isDone = false;
			paramString = string.Format(UpdateLifeGameStringFormat, 0);
			m_outgameSetupEvent(paramString, () => { isDone = true; });
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

			isDone = false;
			paramString = string.Format(SequenceAnimeStringFormat, "GameIn");
			m_outgameSetupEvent(paramString, () => { isDone = true; });
			while (!isDone) { yield return null; }

			var wait = new WaitForSeconds(0.2f);
			for (int i = 0; i < m_life; ++i)
			{
				isDone = false;
				paramString = string.Format(UpdateLifeGameStringFormat, i + 1);
				m_outgameSetupEvent(paramString, () => { isDone = true; });
				while (!isDone) { yield return null; }

				yield return wait;
			}

			m_isEnableControll = true;
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
				if (pressKeys.Contains(KeyCode.Space))
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

				if (pressKeys.Contains(KeyCode.Space))
				{
					m_state = State.Game;
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
						m_life--;
						string paramString = string.Format(UpdateLifeGameStringFormat, m_life);
						m_outgameSetupEvent(paramString, () =>
						{
							OnTransferEvent(0);
						});
						
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

		private void BugEvent(int bugId)
		{
			if (m_reactionBugIdList.Contains(bugId) == true)
			{
				return;
			}
			m_reactionBugIdList.Add(bugId);

			int reactionId = 0;
			switch (bugId)
			{
				case 3:
					{
						reactionId = 3;
						break;
					}
			}
			string paramString = string.Format(CharaReactionStringFormat, reactionId);
			m_outgameSetupEvent(paramString, null);
		}
	}
}