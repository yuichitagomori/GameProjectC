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
		private string UpdateLifeGameStringFormat = "Main,UpdateLifeGauge,{0}";



		[System.Serializable]
		public class MoveVector
		{
			[SerializeField]
			private KeyCode m_key;
			public KeyCode Key => m_key;

			[SerializeField]
			private Vector3 m_vector;
			public Vector3 Vector => m_vector;
		}

		[System.Serializable]
		public class BugTarget
		{
			[SerializeField]
			private int m_bugId;
			public int BugId => m_bugId;

			[SerializeField]
			private GameObject[] m_colliders;
			public GameObject[] Colliders => m_colliders;
		}



		[SerializeField]
		private MoveVector[] m_moveVectors;

		[SerializeField]
		private EventBase[] m_mapEvents;

		[SerializeField]
		private actiongame.ActionGamePlayerChara m_player;

		[SerializeField]
		private BugTarget[] m_bugTargets;

		[SerializeField]
		private actiongame.TransferObject[] m_transfers;

		[SerializeField]
		private actiongame.GuideObject[] m_guides;

		[SerializeField]
		private actiongame.SwitchObject[] m_switches;



		private bool m_isEnableInput = false;

		private KeyCode[] m_beforePressKeys;

		private int m_life;

		private List<int> m_reactionBugIdList = new List<int>();



		public override void Initialize()
		{
			m_isEnableInput = false;

			m_cameraTransform.localPosition = m_cameraAngles[0].localPosition;
			m_cameraTransform.localRotation = m_cameraAngles[0].localRotation;

			for (int i = 0; i < m_mapEvents.Length; ++i)
			{
				m_mapEvents[i].Initialize(OnMapEvent);
			}
			m_player.Initialize(BugEvent);
			for (int i = 0; i < m_transfers.Length; ++i)
			{
				m_transfers[i].Initialize(
					OnTransferEvent,
					OnChangeGameEvent);
			}
			for (int i = 0; i < m_guides.Length; ++i)
			{
				m_guides[i].Initialize();
			}
			for (int i = 0; i < m_switches.Length; ++i)
			{
				int id = i + 1;
				m_switches[i].Initialize(false, (value) => { OnSwitchEvent(id, value); });
			}
			m_beforePressKeys = null;
			m_life = 4;

			var local = GeneralRoot.User.LocalSaveData;
			var bugTarget = m_bugTargets.FirstOrDefault(d => d.BugId == local.OccurredBugId);
			if (bugTarget != null)
			{
				// コライダー削除
				for (int i = 0; i < bugTarget.Colliders.Length; ++i)
				{
					bugTarget.Colliders[i].SetActive(false);
				}
			}
		}

		public override void Go()
		{
			StartCoroutine(GoCoroutine());
		}

		private IEnumerator GoCoroutine()
		{
			bool isDone = false;
			string paramString = string.Format(SequenceAnimeStringFormat, "LoadingOut");
			OutgameSetupEvent(paramString, () => { isDone = true; });
			while (!isDone) { yield return null; }

			if (m_state == State.None)
			{
				isDone = false;
				paramString = string.Format(SequenceAnimeStringFormat, "TitleIn");
				OutgameSetupEvent(paramString, () => { isDone = true; });
				while (!isDone) { yield return null; }

				m_state = State.Title;
				m_isEnableInput = true;
				while (m_state == State.Title) { yield return null; }
				m_isEnableInput = false;

				isDone = false;
				paramString = string.Format(SequenceAnimeStringFormat, "TitleOut");
				OutgameSetupEvent(paramString, () => { isDone = true; });
				while (!isDone) { yield return null; }
			}

			isDone = false;
			m_player.SequenceIn(2.0f, () => { isDone = true; });
			while (!isDone) { yield return null; }

			Vector3 beforePosition = m_cameraAngles[0].localPosition;
			Quaternion beforeRotation = m_cameraAngles[0].localRotation;
			Vector3 afterPosition = m_cameraAngles[1].localPosition;
			Quaternion afterRotation = m_cameraAngles[1].localRotation;
			yield return CommonMath.EaseInOutTransform(
				m_cameraTransform,
				beforePosition,
				beforeRotation,
				afterPosition,
				afterRotation,
				1.0f,
				null);

			isDone = false;
			paramString = string.Format(UpdateLifeGameStringFormat, 0);
			OutgameSetupEvent(paramString, () => { isDone = true; });
			while (!isDone) { yield return null; }

			isDone = false;
			paramString = string.Format(SequenceAnimeStringFormat, "GameIn");
			OutgameSetupEvent(paramString, () => { isDone = true; });
			while (!isDone) { yield return null; }

			var wait = new WaitForSeconds(0.2f);
			for (int i = 0; i < m_life; ++i)
			{
				isDone = false;
				paramString = string.Format(UpdateLifeGameStringFormat, i + 1);
				OutgameSetupEvent(paramString, () => { isDone = true; });
				while (!isDone) { yield return null; }

				yield return wait;
			}

			m_isEnableInput = true;
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
			if (m_isEnableInput == false)
			{
				return;
			}

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
				if (pressKeys.Contains(KeyCode.A))
				{
					var moveVector = m_moveVectors.FirstOrDefault(d => d.Key == KeyCode.A);
					if (moveVector != null)
					{
						m_player.Move(moveVector.Vector);
					}
				}
				else if (pressKeys.Contains(KeyCode.D))
				{
					var moveVector = m_moveVectors.FirstOrDefault(d => d.Key == KeyCode.D);
					if (moveVector != null)
					{
						m_player.Move(moveVector.Vector);
					}
				}

				if (pressKeys.Contains(KeyCode.W))
				{
					var moveVector = m_moveVectors.FirstOrDefault(d => d.Key == KeyCode.W);
					if (moveVector != null)
					{
						m_player.Move(moveVector.Vector);
					}
					else if (m_beforePressKeys != null & !m_beforePressKeys.Contains(KeyCode.W))
					{
						m_player.Jump();
					}
				}
				else if (pressKeys.Contains(KeyCode.S))
				{
					var moveVector = m_moveVectors.FirstOrDefault(d => d.Key == KeyCode.S);
					if (moveVector != null)
					{
						m_player.Move(moveVector.Vector);
					}
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
						OutgameSetupEvent(paramString, () =>
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
			m_isEnableInput = false;

			bool isDone = false;
			m_player.SequenceOut(0.5f, () => { isDone = true; });
			while (!isDone) { yield return null; }

			m_player.Transfer();

			yield return new WaitForSeconds(0.5f);
			m_player.transform.position = m_transfers[index].transform.position;
			yield return new WaitForSeconds(0.5f);

			isDone = false;
			m_player.SequenceIn(1.0f, () => { isDone = true; });
			while (!isDone) { yield return null; }

			m_isEnableInput = true;
		}

		private void OnChangeGameEvent(string sceneName)
		{
			StartCoroutine(OnChangeGameEventCoroutine(sceneName));
		}

		private IEnumerator OnChangeGameEventCoroutine(string sceneName)
		{
			m_isEnableInput = false;

			bool isDone = false;
			m_player.SequenceOut(0.5f, () => { isDone = true; });
			while (!isDone) { yield return null; }

			m_player.Transfer();

			isDone = true;
			string paramString = string.Format(SequenceAnimeStringFormat, "GameOut");
			OutgameSetupEvent(paramString, () => { isDone = true; });
			while (!isDone) { yield return null; }

			isDone = false;
			paramString = string.Format(SequenceAnimeStringFormat, "LoadingIn");
			OutgameSetupEvent(paramString, () => { isDone = true; });
			while (!isDone) { yield return null; }

			ChangeGameEvent(sceneName, State.Game);
		}

		private void OnSwitchEvent(int id, bool value)
		{

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
			OutgameSetupEvent(paramString, null);
		}
	}
}