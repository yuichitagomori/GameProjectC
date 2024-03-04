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
		private string UpdateLifeGameStringFormat = "Outgame,PlayWindow,Main,UpdateLifeGauge,{0},{1}";



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

		[SerializeField]
		private actiongame.EnemyBase[] m_enemies;



		private bool m_enableInput = false;

		private KeyCode[] m_beforePressKeys;

		private int m_life;

		private int m_lifeMax;



		public override void Initialize()
		{
			m_enableInput = false;

			m_cameraTransform.localPosition = m_cameraAngles[0].localPosition;
			m_cameraTransform.localRotation = m_cameraAngles[0].localRotation;

			for (int i = 0; i < m_mapEvents.Length; ++i)
			{
				m_mapEvents[i].Initialize(OnEvent);
			}
			m_player.Initialize(BugEvent);
			for (int i = 0; i < m_transfers.Length; ++i)
			{
				m_transfers[i].Initialize(OnEvent);
			}
			for (int i = 0; i < m_guides.Length; ++i)
			{
				m_guides[i].Initialize();
			}
			for (int i = 0; i < m_switches.Length; ++i)
			{
				m_switches[i].Initialize(OnEvent);
			}
			for (int i = 0; i < m_enemies.Length; ++i)
			{
				m_enemies[i].Initialize(OnEvent);
			}
			m_beforePressKeys = null;
			m_life = 4;
			m_lifeMax = 4;

			if (string.IsNullOrEmpty(m_initializeParam) == false)
			{
				int transferIndex = int.Parse(m_initializeParam);
				Vector3 pos = m_transfers[transferIndex].transform.position;
				m_player.transform.position = pos;
				m_cameraParentTransform.position = pos;
			}

			var temporary = GeneralRoot.User.LocalTemporaryData;
			var bugTarget = m_bugTargets.FirstOrDefault(d => d.BugId == temporary.OccurredBugId);
			if (bugTarget != null)
			{
				// コライダー削除
				for (int i = 0; i < bugTarget.Colliders.Length; ++i)
				{
					bugTarget.Colliders[i].SetActive(false);
				}
			}

			SetSkyboxSequenceTime(0.0f);
		}

		public override void Go()
		{
			StartCoroutine(GoCoroutine());
		}

		private IEnumerator GoCoroutine()
		{
			bool isDone = false;
			string paramString = string.Format(SequenceAnimeStringFormat, "LoadingOut");
			PlayMovieEvent(paramString, () => { isDone = true; });
			while (!isDone) { yield return null; }

			if (m_state == State.None)
			{
				isDone = false;
				paramString = string.Format(SequenceAnimeStringFormat, "TitleIn");
				PlayMovieEvent(paramString, () => { isDone = true; });
				while (!isDone) { yield return null; }

				m_state = State.Title;
				m_enableInput = true;
				while (m_state == State.Title) { yield return null; }
				m_enableInput = false;

				isDone = false;
				paramString = string.Format(SequenceAnimeStringFormat, "TitleOut");
				PlayMovieEvent(paramString, () => { isDone = true; });
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
			paramString = string.Format(UpdateLifeGameStringFormat, 0, m_lifeMax);
			PlayMovieEvent(paramString, () => { isDone = true; });
			while (!isDone) { yield return null; }

			isDone = false;
			paramString = string.Format(SequenceAnimeStringFormat, "GameIn");
			PlayMovieEvent(paramString, () => { isDone = true; });
			while (!isDone) { yield return null; }

			var wait = new WaitForSeconds(0.2f);
			for (int i = 0; i < m_life; ++i)
			{
				isDone = false;
				paramString = string.Format(UpdateLifeGameStringFormat, i + 1, m_lifeMax);
				PlayMovieEvent(paramString, () => { isDone = true; });
				while (!isDone) { yield return null; }

				yield return wait;
			}

			m_enableInput = true;
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
			if (m_enableInput == false)
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
					var moveVector = m_moveVectors.FirstOrDefault(d => d.Key == KeyCode.Space);
					if (moveVector != null)
					{
						if (m_beforePressKeys != null & !m_beforePressKeys.Contains(KeyCode.Space))
						{
							m_player.Jump();
						}
					}
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

		private void OnEvent(string[] eventParams)
		{
			StartCoroutine(OnEventCoroutine(eventParams));
		}

		private IEnumerator OnEventCoroutine(string[] eventParams)
		{
			for (int i = 0; i < eventParams.Length; ++i)
			{
				string[] eventParamStrings = eventParams[i].Split(',');
				switch (eventParamStrings[0])
				{
					case "Delete":
						{
							int submitLife = int.Parse(eventParamStrings[1]);
							m_life -= submitLife;

							bool isDone = false;
							string paramString = string.Format(UpdateLifeGameStringFormat, m_life, m_lifeMax);
							PlayMovieEvent(paramString, () => { isDone = true; });
							while (!isDone) { yield return null; }
							yield return TransferCoroutine(0);
							break;
						}
					case "Transfer":
						{
							int index = int.Parse(eventParamStrings[1]);
							yield return TransferCoroutine(index);
							break;
						}
					case "Switch":
						{
							int index = int.Parse(eventParamStrings[1]);
							m_switches[index].Switch(!m_switches[index].Value);
							break;
						}
					case "ChangeGame":
						{
							string changeGameName = eventParamStrings[1];
							string sceneParamString = "";
							if (eventParamStrings.Length >= 3)
							{
								sceneParamString = eventParamStrings[2];
							}
							yield return OnChangeGameEventCoroutine(changeGameName, sceneParamString);
							break;
						}
					case "AddClearSceneName":
						{
							string clearSceneName = eventParamStrings[1];
							var temporary = GeneralRoot.User.LocalTemporaryData;
							temporary.ClearSceneNameList.Add(clearSceneName);
							break;
						}
					case "SkyboxSequence":
						{
							bool isIn = bool.Parse(eventParamStrings[1]);
							yield return OnSkyboxSequenceCoroutine(isIn);
							break;
						}
				}
			}
		}

		private IEnumerator TransferCoroutine(int index)
		{
			m_enableInput = false;

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

			m_enableInput = true;
		}

		private IEnumerator OnChangeGameEventCoroutine(string sceneName, string sceneParamString)
		{
			m_enableInput = false;

			bool isDone = false;
			m_player.SequenceOut(0.5f, () => { isDone = true; });
			while (!isDone) { yield return null; }

			m_player.Transfer();

			isDone = true;
			string paramString = string.Format(SequenceAnimeStringFormat, "GameOut");
			PlayMovieEvent(paramString, () => { isDone = true; });
			while (!isDone) { yield return null; }

			isDone = false;
			paramString = string.Format(SequenceAnimeStringFormat, "LoadingIn");
			PlayMovieEvent(paramString, () => { isDone = true; });
			while (!isDone) { yield return null; }

			ChangeGameEvent(sceneName, State.Game, sceneParamString);
		}

		private IEnumerator OnSkyboxSequenceCoroutine(bool isIn)
		{
			m_enableInput = false;
			if (isIn)
			{
				SetSkyboxSequenceTime(0.0f);
				yield return CommonMath.EaseInOut(
					1.0f,
					(time) => { SetSkyboxSequenceTime(time); },
					() => { SetSkyboxSequenceTime(1.0f); });
			}
			else
			{
				SetSkyboxSequenceTime(1.0f);
				yield return CommonMath.EaseInOut(
					1.0f,
					(time) => { SetSkyboxSequenceTime(1.0f - time); },
					() => { SetSkyboxSequenceTime(0.0f); });
			}
			m_enableInput = true;
		}

		private void BugEvent()
		{
			var temporary = GeneralRoot.User.LocalTemporaryData;
			var checkSheetBugMaster = GeneralRoot.Master.CheckSheetBugData;
			var checkSheetBugMasterData = checkSheetBugMaster.Find(temporary.OccurredBugId);
			if (checkSheetBugMasterData == null)
			{
				return;
			}

			string reactionAnimeName = "";
			switch (checkSheetBugMasterData.ReactionType)
			{
				case 1:
				case 2:
				case 3:
					{
						reactionAnimeName = "Reaction02";
						break;
					}
			}
			if (string.IsNullOrEmpty(reactionAnimeName) == true)
			{
				return;
			}

			string paramString = string.Format(CameraReactionStringFormat, reactionAnimeName, "false");
			PlayMovieEvent(paramString, () =>
			{
				paramString = string.Format(CameraReactionStringFormat, "Wait", "true");
				PlayMovieEvent(paramString, null);
			});
		}
	}
}