using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using BoardPartsData = data.UserData.LocalSave.CustomizeBoardPartsData;

namespace scene.dialog.board
{
	/// <summary>
	/// パーツ表示管理
	/// </summary>
	public class CustomizeBoardPartsView : MonoBehaviour
	{
		/// <summary>
		/// マス目サイズ
		/// </summary>
		private int GridSize = 60;

		/// <summary>
		/// マス目サイズ
		/// </summary>
		private int HalfGridSize = 30;

		/// <summary>
		/// データクラス
		/// </summary>
		public class Data
		{
			/// <summary>
			/// パーツの状態
			/// </summary>
			public enum Type
			{
				Seted,
				Setting,
				NotSet,
			}

			/// <summary>
			/// パーツ情報
			/// </summary>
			private BoardPartsData m_boardPartsData;
			public BoardPartsData BoardPartsData => m_boardPartsData;

			/// <summary>
			/// 領域情報
			/// </summary>
			private Grid[] m_areaGrids;
			public Grid[] AreaGrids => m_areaGrids;

			/// <summary>
			/// パーツ画像
			/// </summary>
			private Sprite m_partsSprite;
			public Sprite PartsSprite => m_partsSprite;

			/// <summary>
			/// 選択状態かどうか
			/// </summary>
			private Type m_stateType;
			public Type StateType => m_stateType;

			public Data(
				BoardPartsData boardPartsData,
				Grid[] areaGrids,
				Sprite partsSprite,
				Type stateType)
			{
				m_boardPartsData = boardPartsData;
				m_areaGrids = areaGrids;
				m_partsSprite = partsSprite;
				m_stateType = stateType;
			}

			public Grid[] GetUseAreaGrids()
			{
				return m_areaGrids
					.Select(d => GetRotateGrid(d, m_boardPartsData))
					.ToArray();
			}

			private Grid GetRotateGrid(Grid grid, BoardPartsData boardPartsData)
			{
				// 2, 1
				// -1, 2
				// -2, -1
				// 1, -2
				Grid newGrid = Grid.zero;
				switch (boardPartsData.Rot)
				{
					case BoardPartsData.Rotate.Z0:
						{
							newGrid = Grid.Create(grid.x, grid.y);
							break;
						}
					case BoardPartsData.Rotate.Z90:
						{
							newGrid = Grid.Create(-grid.y, grid.x);
							break;
						}
					case BoardPartsData.Rotate.Z180:
						{
							newGrid = Grid.Create(-grid.x, -grid.y);
							break;
						}
					case BoardPartsData.Rotate.Z270:
						{
							newGrid = Grid.Create(grid.y, -grid.x);
							break;
						}
				}
				return newGrid + boardPartsData.Grid;
			}
		}

		/// <summary>
		/// パーツアイコン
		/// </summary>
		[SerializeField]
		private Image m_partsImage;

		/// <summary>
		/// パーツのトランスフォーム
		/// </summary>
		[SerializeField]
		private RectTransform m_partsTransform;

		/// <summary>
		/// パーツのアニメーション
		/// </summary>
		[SerializeField]
		private AnimatorExpansion m_partsAnimation;



		public void Initialize()
		{

		}

		public void UpdateView(Data data)
		{
			m_partsImage.sprite = data.PartsSprite;
			m_partsImage.SetNativeSize();
			int rotateZ = 0;
			switch (data.BoardPartsData.Rot)
			{
				case BoardPartsData.Rotate.Z0:
					{
						rotateZ = 0;
						break;
					}
				case BoardPartsData.Rotate.Z90:
					{
						rotateZ = -90;
						break;
					}
				case BoardPartsData.Rotate.Z180:
					{
						rotateZ = -180;
						break;
					}
				case BoardPartsData.Rotate.Z270:
					{
						rotateZ = -270;
						break;
					}
			}

			m_partsTransform.localPosition = GetPosition(data.BoardPartsData.Grid);
			m_partsTransform.localRotation = Quaternion.Euler(0, 0, rotateZ);
			m_partsTransform.pivot = GetPivot();

			switch (data.StateType)
			{
				case Data.Type.Seted:
					{
						m_partsAnimation.Play("Seted");
						break;
					}
				case Data.Type.Setting:
					{
						m_partsAnimation.Play("Setting");
						break;
					}
				case Data.Type.NotSet:
					{
						m_partsAnimation.PlayLoop("NotSet");
						break;
					}
			}
		}

		private Vector3 GetPosition(Grid grid)
		{
			int positionX = grid.x * GridSize - HalfGridSize + 16;
			int positionY = -(grid.y * GridSize - HalfGridSize + 16);
			return new Vector3(positionX, positionY, 0);
		}

		private Vector2 GetPivot()
		{
			return new Vector2(
				GetPivotParam((int)m_partsTransform.sizeDelta.x),
				1.0f - GetPivotParam((int)m_partsTransform.sizeDelta.y));
		}

		private float GetPivotParam(int spriteSize)
		{
			const int One = 92; // 60 + 32
			const int Two = 152; // 120 + 32
			const int Three = 212; // 180 + 32
			const int Fore = 272; // 240 + 32
			const int Five = 332; // 300 + 32
			switch (spriteSize)
			{
				case One:
				case Two:
					{
						return (float)(HalfGridSize + 16) / (float)spriteSize;
					}
				case Three:
				case Fore:
					{
						return (float)(GridSize + HalfGridSize + 16) / (float)spriteSize;
					}
				case Five:
					{
						return (float)(GridSize * 2 + HalfGridSize + 16) / (float)spriteSize;
					}
				default:
					{
						return (float)(HalfGridSize + 16) / (float)spriteSize;
					}
			}
		}
	}
}