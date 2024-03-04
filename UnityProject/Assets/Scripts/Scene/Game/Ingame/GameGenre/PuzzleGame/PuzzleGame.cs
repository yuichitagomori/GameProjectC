using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene.game.ingame
{
	public class PuzzleGame : GameGenreBase
	{
		private string UpdateTurnStringFormat = "Main,UpdateTurn,{0}";



		[System.Serializable]
		public class Data
		{
			[System.Serializable]
			public class ElementData
			{
				[SerializeField]
				private puzzlegame.FieldTileElement.DirtType m_dirtType;
				public puzzlegame.FieldTileElement.DirtType DirtType => m_dirtType;

				[SerializeField]
				private Grid m_grid;
				public Grid Grid => m_grid;

				public void UpdateDirtType(puzzlegame.FieldTileElement.DirtType dirtType)
				{
					m_dirtType = dirtType;
				}

				public ElementData Copy()
				{
					return new ElementData(DirtType, Grid);
				}

				public ElementData(puzzlegame.FieldTileElement.DirtType dirtType, Grid grid)
				{
					m_dirtType = dirtType;
					m_grid = grid;
				}
			}

			[SerializeField]
			private int m_maxTurn;
			public int MaxTurn => m_maxTurn;

			[SerializeField]
			private ElementData[] m_fieldElementDatas;
			public ElementData[] FieldElementDatas => m_fieldElementDatas;

			public ElementData[] CopyFieldElementData()
			{
				List<ElementData> dataList = new List<ElementData>();
				for (int i = 0; i < FieldElementDatas.Length; ++i)
				{
					dataList.Add(FieldElementDatas[i].Copy());
				}
				return dataList.ToArray();
			}
		}

		[SerializeField]
		private Data m_data;

		[SerializeField]
		private Common.ElementList m_fieldTileElementList;

		[SerializeField]
		private puzzlegame.Cleaner m_cleaner;

		[SerializeField]
		private string m_changeSceneName;

		[SerializeField]
		private bool m_isLastStage;



		private bool m_isEnableInput = false;

		private KeyCode[] m_beforePressKeys;

		private UnityAction<int> m_moveEvent;

		private UnityAction<bool> m_rotateEvent;

		private int m_gridLine;

		private bool m_isVertical;

		private int m_nowTurn;

		private Data.ElementData[] m_nowFieldElementDatas;



		public override void Initialize()
		{
			m_isEnableInput = false;

			m_cameraTransform.localPosition = m_cameraAngles[0].localPosition;
			m_cameraTransform.localRotation = m_cameraAngles[0].localRotation;

			m_nowTurn = m_data.MaxTurn;
			var elements = m_fieldTileElementList.GetElements();
			for (int i = 0; i < elements.Count; ++i)
			{
				var elementObject = elements[i];
				int gridX = (int)(i / 7);
				int gridY = i % 7;
				int posX = gridX * 4 - 12;
				int posY = gridY * -4 + 12;
				Vector3 pos = new Vector3(posX, 0.0f, posY);
				elementObject.transform.position = pos;
				var element = elementObject.GetComponent<puzzlegame.FieldTileElement>();
				Color color = (i % 2 == 0) ? Color.white : new Color(0.6f, 0.6f, 0.6f);
				element.Initialize(Grid.Create(gridX, gridY), color, CleanFieldTile);
			}
			ResetFieldTile();
			m_cleaner.Initialize();

			m_moveEvent = (gridLine) =>
			{
				m_gridLine = gridLine;
				UpdateSelectFieldTile();
			};
			m_rotateEvent = (value) =>
			{
				m_isVertical = value;
				m_gridLine = 3;
				UpdateSelectFieldTile();
			};
			m_gridLine = 3;
			m_isVertical = false;

			UpdateCleanerPosition();
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
				m_isEnableInput = true;
				while (m_state == State.Title) { yield return null; }
				m_isEnableInput = false;

				isDone = false;
				paramString = string.Format(SequenceAnimeStringFormat, "TitleOut");
				PlayMovieEvent(paramString, () => { isDone = true; });
				while (!isDone) { yield return null; }
			}

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
			paramString = string.Format(UpdateTurnStringFormat, 0);
			PlayMovieEvent(paramString, () => { isDone = true; });
			while (!isDone) { yield return null; }

			isDone = false;
			paramString = string.Format(SequenceAnimeStringFormat, "GameIn");
			PlayMovieEvent(paramString, () => { isDone = true; });
			while (!isDone) { yield return null; }

			for (int i = 0; i < m_nowTurn; ++i)
			{
				isDone = false;
				paramString = string.Format(UpdateTurnStringFormat, i + 1);
				PlayMovieEvent(paramString, () => { isDone = true; });
				while (!isDone) { yield return null; }
			}

			UpdateSelectFieldTile();

			m_isEnableInput = true;
		}

		private void FixedUpdate()
		{
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
				if (pressKeys.Contains(KeyCode.A) &&
					!m_beforePressKeys.Contains(KeyCode.A))
				{
					if (m_isVertical == true)
					{
						if (m_gridLine > 0)
						{
							m_moveEvent(m_gridLine - 1);
						}
					}
					else
					{
						m_rotateEvent(!m_isVertical);
					}
				}
				else if (pressKeys.Contains(KeyCode.D) &&
					!m_beforePressKeys.Contains(KeyCode.D))
				{
					if (m_isVertical == true)
					{
						if (m_gridLine < (7 - 1))
						{
							m_moveEvent(m_gridLine + 1);
						}
					}
					else
					{
						m_rotateEvent(!m_isVertical);
					}
				}
				else if (pressKeys.Contains(KeyCode.W) &&
					!m_beforePressKeys.Contains(KeyCode.W))
				{
					if (m_isVertical == false)
					{
						if (m_gridLine > 0)
						{
							m_moveEvent(m_gridLine - 1);
						}
					}
					else
					{
						m_rotateEvent(!m_isVertical);
					}
				}
				else if (pressKeys.Contains(KeyCode.S) &&
					!m_beforePressKeys.Contains(KeyCode.S))
				{
					if (m_isVertical == false)
					{
						if (m_gridLine < (7 - 1))
						{
							m_moveEvent(m_gridLine + 1);
						}
					}
					else
					{
						m_rotateEvent(!m_isVertical);
					}
				}
				else if (pressKeys.Contains(KeyCode.Space) &&
					!m_beforePressKeys.Contains(KeyCode.Space))
				{
					StartCoroutine(PlayClean());
				}
			}
			
			m_beforePressKeys = pressKeys;
		}

		private void UpdateCleanerPosition()
		{
			if (m_isVertical == true)
			{
				float posX = m_gridLine * 4.0f - 12.0f;
				m_cleaner.transform.position = new Vector3(posX, 0.0f, -12.0f);
				m_cleaner.transform.rotation = Quaternion.AngleAxis(0.0f, Vector3.up);
			}
			else
			{
				float posZ = m_gridLine * -4.0f + 12.0f;
				m_cleaner.transform.position = new Vector3(-12.0f, 0.0f, posZ);
				m_cleaner.transform.rotation = Quaternion.AngleAxis(90.0f, Vector3.up);
			}
		}

		private void UpdateSelectFieldTile()
		{
			var elements = m_fieldTileElementList.GetElements();
			for (int i = 0; i < elements.Count; ++i)
			{
				var elementObject = elements[i];
				int gridX = (int)(i / 7);
				int gridY = i % 7;
				var element = elementObject.GetComponent<puzzlegame.FieldTileElement>();
				bool isSelected = false;
				if (m_isVertical == true)
				{
					isSelected = (m_gridLine == gridX);
				}
				else
				{
					isSelected = (m_gridLine == gridY);
				}
				element.SetSelected(isSelected);
			}
		}

		private void ResetFieldTile()
		{
			m_nowFieldElementDatas = m_data.CopyFieldElementData();
			var elements = m_fieldTileElementList.GetElements();
			for (int i = 0; i < elements.Count; ++i)
			{
				var elementObject = elements[i];
				int gridX = (int)(i / 7);
				int gridY = i % 7;
				var element = elementObject.GetComponent<puzzlegame.FieldTileElement>();
				var fieldElementData = m_nowFieldElementDatas.FirstOrDefault(d =>
					d.Grid.x == gridX &&
					d.Grid.y == gridY);
				puzzlegame.FieldTileElement.DirtType dirtType = (fieldElementData != null) ?
					fieldElementData.DirtType :
					puzzlegame.FieldTileElement.DirtType.None;
				element.Setting(dirtType);
			}
		}

		private void CleanFieldTile(Grid grid)
		{
			Data.ElementData data = m_nowFieldElementDatas.FirstOrDefault(d => d.Grid == grid);
			if (data == null)
			{
				return;
			}
			switch (data.DirtType)
			{
				case puzzlegame.FieldTileElement.DirtType.Milk:
					{
						data.UpdateDirtType(puzzlegame.FieldTileElement.DirtType.None);
						break;
					}
				case puzzlegame.FieldTileElement.DirtType.Art01:
					{
						data.UpdateDirtType(puzzlegame.FieldTileElement.DirtType.Art02);
						break;
					}
				case puzzlegame.FieldTileElement.DirtType.Art02:
					{
						data.UpdateDirtType(puzzlegame.FieldTileElement.DirtType.Art03);
						break;
					}
				case puzzlegame.FieldTileElement.DirtType.Art03:
					{
						data.UpdateDirtType(puzzlegame.FieldTileElement.DirtType.None);
						break;
					}
				default:
					{
						return;
					}
			}

			var elements = m_fieldTileElementList.GetElements();
			int index = grid.x * 7 + grid.y;
			var element = elements[index].GetComponent<puzzlegame.FieldTileElement>();
			element.Setting(data.DirtType);
		}

		private IEnumerator PlayClean()
		{
			m_isEnableInput = false;

			UpdateCleanerPosition();
			bool isDone = false;
			m_cleaner.Play(() => { isDone = true; });
			while (!isDone) { yield return null; }

			m_nowTurn--;

			isDone = false;
			string paramString = string.Format(UpdateTurnStringFormat, m_nowTurn);
			PlayMovieEvent(paramString, () => { isDone = true; });
			while (!isDone) { yield return null; }

			var dirtData = m_nowFieldElementDatas.FirstOrDefault(d => d.DirtType != puzzlegame.FieldTileElement.DirtType.None);
			bool isClear = dirtData == null;
			if (isClear == true)
			{
				// クリア

				m_gridLine = -1;
				UpdateSelectFieldTile();

				isDone = false;
				paramString = string.Format(SequenceAnimeStringFormat, "ResultIn");
				PlayMovieEvent(paramString, () => { isDone = true; });
				while (!isDone) { yield return null; }
			}
			else if (m_nowTurn <= 0)
			{
				// クリア失敗

				m_gridLine = -1;
				UpdateSelectFieldTile();
			}
			else
			{
				m_isEnableInput = true;
				yield break;
			}

			// 次のステージへ
			yield return new WaitForSeconds(1.0f);

			isDone = false;
			paramString = string.Format(SequenceAnimeStringFormat, "LoadingIn");
			PlayMovieEvent(paramString, () => { isDone = true; });
			while (!isDone) { yield return null; }

			if (isClear == true)
			{
				if (m_isLastStage == true)
				{
					ChangeGameEvent(m_changeSceneName, State.None, "");
				}
				else
				{
					ChangeGameEvent(m_changeSceneName, State.Game, "");
				}
			}
			else
			{
				ChangeGameEvent(SceneName, State.Game, "");
			}
		}
	}
}