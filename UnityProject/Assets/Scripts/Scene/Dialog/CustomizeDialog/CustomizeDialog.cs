using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using UniqueItem = data.UserData.LocalSave.UniqueItem;
using BoardPartsData = data.UserData.LocalSave.CustomizeData.BoardPartsData;

namespace scene.dialog
{
	public class CustomizeDialog : SceneBase
	{
		public class Data
		{
		}

		/// <summary>
		/// アニメーター
		/// </summary>
		[SerializeField]
		private Common.AnimatorExpansion m_animator;

		/// <summary>
		/// 閉じるボタン
		/// </summary>
		[SerializeField]
		private CommonUI.ButtonExpansion m_closeButton;

		/// <summary>
		/// タイトル
		/// </summary>
		[SerializeField]
		private CommonUI.TextExpansion m_titleText;

		/// <summary>
		/// カスタマイズボード
		/// </summary>
		[SerializeField]
		private CustomizeBoard m_board;

		/// <summary>
		/// パーツリストの右切り替えボタン
		/// </summary>
		[SerializeField]
		private CommonUI.ButtonExpansion m_partsListRightButton;

		/// <summary>
		/// パーツリストの左切り替えボタン
		/// </summary>
		[SerializeField]
		private CommonUI.ButtonExpansion m_partsListLeftButton;

		/// <summary>
		/// パーツリストのアニメーター
		/// </summary>
		[SerializeField]
		private Common.AnimatorExpansion m_partsListAnimator;

		/// <summary>
		/// パーツ一覧のパーツ
		/// </summary>
		[SerializeField]
		private Common.ElementList m_partsListElement;

		/// <summary>
		/// パーツ一覧のカルーセル
		/// </summary>
		[SerializeField]
		private Common.ElementList m_partsListCarousel;

		/// <summary>
		/// 選択中パーツ詳細テキスト
		/// </summary>
		[SerializeField]
		private CommonUI.TextExpansion m_partsInfoText;



		/// <summary>
		/// Finish時
		/// </summary>
		private UnityAction m_finishCallback = null;

		/// <summary>
		/// 外部設定情報
		/// </summary>
		private Data m_data = null;

		/// <summary>
		/// ユーザーのカスタマイズパーツリスト
		/// </summary>
		private List<UniqueItem> m_userCustomizePartsList = null;

		/// <summary>
		/// 選択中パーツ番号
		/// </summary>
		private int m_selectIndex = 0;

		/// <summary>
		/// 選択中ボード内パーツの位置（選択中パーツがボード内に存在しない場合のキャッシュ値）
		/// </summary>
		private Grid m_cacheSelectBoardPartsGrid;

		/// <summary>
		/// 選択中ボード内パーツの回転状態（選択中パーツがボード内に存在しない場合のキャッシュ値）
		/// </summary>
		private BoardPartsData.Rotate m_cacheSelectBoardPartsRotate;



		/// <summary>
		/// 事前設定
		/// </summary>
		/// <param name="data"></param>
		/// <param name="finishCallback"></param>
		public void Setting(Data data, UnityAction finishCallback)
		{
			m_data = data;
			m_finishCallback = finishCallback;
		}

		public override void Ready(UnityAction callback)
		{
			StartCoroutine(ReadyCoroutine(callback));
		}

		private IEnumerator ReadyCoroutine(UnityAction callback)
		{
			m_closeButton.SetupClickEvent(() =>
			{
				m_sceneController.RemoveScene(this, null);
			});
			m_closeButton.interactable = false;

			m_partsListRightButton.SetupClickEvent(OnPartsListRightButtonPressed);
			m_partsListRightButton.interactable = false;
			m_partsListLeftButton.SetupClickEvent(OnPartsListLeftButtonPressed);
			m_partsListRightButton.interactable = false;
			m_partsListAnimator.Play("Default");

			var localSaveData = GeneralRoot.User.LocalSaveData;
			m_userCustomizePartsList = localSaveData.UniqueItemList.FindAll(d =>
				d.Category == UniqueItem.CategoryType.CustomizeParts &&
				d.Id != 101);

			m_board.Initialize(
				OnBoardPartsMoveRightButtonPressed,
				OnBoardPartsMoveLeftButtonPressed,
				OnBoardPartsMoveUpButtonPressed,
				OnBoardPartsMoveDownButtonPressed,
				OnBoardPartsRotateButtonPressed,
				OnBoardPartsDecisionButtonPressed);

			m_selectIndex = 0;

			// キャッシュ値、パーツ一覧表記、ボード表記の初期化をここでまとめて行う。
			ChangePartsList();

			bool isDone = false;
			m_animator.Play("In", () => { isDone = true; });
			while (!isDone) { yield return null; }

			m_closeButton.interactable = true;
			m_partsListRightButton.interactable = (m_userCustomizePartsList.Count > 1);
			m_partsListLeftButton.interactable = (m_userCustomizePartsList.Count > 1);

			if (callback != null)
			{
				callback();
			}
		}

		public override void Go()
		{
		}

		public override void Finish(UnityAction callback)
		{
			StartCoroutine(FinishCoroutine(callback));
		}

		private IEnumerator FinishCoroutine(UnityAction callback)
		{
			bool isDone = false;
			m_animator.Play("Out", () => { isDone = true; });
			while (!isDone) { yield return null; }

			if (m_finishCallback != null)
			{
				m_finishCallback();
			}

			if (callback != null)
			{
				callback();
			}
		}

		private void OnPartsListRightButtonPressed()
		{
			StartCoroutine(OnPartsListRightButtonPressedCoroutine());
		}

		private IEnumerator OnPartsListRightButtonPressedCoroutine()
		{
			m_selectIndex++;
			if (m_selectIndex >= m_userCustomizePartsList.Count)
			{
				m_selectIndex = 0;
			}

			bool isDone = false;
			m_partsListAnimator.Play("ChangeRight", () => { isDone = true; });
			while (!isDone) { yield return null; }

			m_partsListAnimator.Play("Default");

			ChangePartsList();
		}

		private void OnPartsListLeftButtonPressed()
		{
			StartCoroutine(OnPartsListLeftButtonPressedCoroutine());
		}

		private IEnumerator OnPartsListLeftButtonPressedCoroutine()
		{
			m_selectIndex--;
			if (m_selectIndex < 0)
			{
				m_selectIndex = m_userCustomizePartsList.Count - 1;
			}

			bool isDone = false;
			m_partsListAnimator.Play("ChangeLeft", () => { isDone = true; });
			while (!isDone) { yield return null; }

			m_partsListAnimator.Play("Default");

			ChangePartsList();
		}

		private void ChangePartsList()
		{
			var localSaveData = GeneralRoot.User.LocalSaveData;
			var customizeData = localSaveData.Customize;
			var selectParts = m_userCustomizePartsList[m_selectIndex];
			var boardPartsData = customizeData.Find(selectParts.UniqueId);
			if (boardPartsData != null)
			{
				// ボード内にセットされているパーツなので、そのパーツの位置をキャッシュ
				m_cacheSelectBoardPartsGrid = boardPartsData.Grid;
				m_cacheSelectBoardPartsRotate = boardPartsData.Rot;
			}
			else
			{
				// 初期化
				m_cacheSelectBoardPartsGrid = Grid.Create(3, 3);
				m_cacheSelectBoardPartsRotate = BoardPartsData.Rotate.Z0;
			}

			UpdatePartsView();
			UpdateBoardView();
		}

		private void OnBoardPartsMoveRightButtonPressed()
		{
			m_cacheSelectBoardPartsGrid.x += 1;
			UpdateBoardView();
		}

		private void OnBoardPartsMoveLeftButtonPressed()
		{
			m_cacheSelectBoardPartsGrid.x -= 1;
			UpdateBoardView();
		}

		private void OnBoardPartsMoveUpButtonPressed()
		{
			m_cacheSelectBoardPartsGrid.y -= 1;
			UpdateBoardView();
		}

		private void OnBoardPartsMoveDownButtonPressed()
		{
			m_cacheSelectBoardPartsGrid.y += 1;
			UpdateBoardView();
		}

		private void OnBoardPartsRotateButtonPressed()
		{
			m_cacheSelectBoardPartsRotate = GetNextRotate(m_cacheSelectBoardPartsRotate);
			UpdateBoardView();
		}

		private void OnBoardPartsDecisionButtonPressed()
		{
			var selectParts = m_userCustomizePartsList[m_selectIndex];
			var localSaveData = GeneralRoot.User.LocalSaveData;
			var customizeData = localSaveData.Customize;
			var selectBoardParts = customizeData.Find(selectParts.UniqueId);
			if (selectBoardParts != null)
			{
				if (selectBoardParts.Grid == m_cacheSelectBoardPartsGrid &&
					selectBoardParts.Rot == m_cacheSelectBoardPartsRotate)
				{
					// 同じ位置、同じ回転の同じピースが決定された場合は削除
					customizeData.Remove(selectParts.UniqueId);
				}
				else
				{
					selectBoardParts.UpdateGrid(m_cacheSelectBoardPartsGrid);
					selectBoardParts.UpdateRotate(m_cacheSelectBoardPartsRotate);
				}
			}
			else
			{
				customizeData.Add(
					selectParts.UniqueId,
					m_cacheSelectBoardPartsGrid,
					m_cacheSelectBoardPartsRotate);
			}
			UpdatePartsView();
			UpdateBoardView();
		}

		/// <summary>
		/// 回転後の回転状態を取得
		/// </summary>
		/// <param name="rot"></param>
		/// <returns></returns>
		private BoardPartsData.Rotate GetNextRotate(BoardPartsData.Rotate rot)
		{
			switch (rot)
			{
				case BoardPartsData.Rotate.Z0:
					{
						return BoardPartsData.Rotate.Z90;
					}
				case BoardPartsData.Rotate.Z90:
					{
						return BoardPartsData.Rotate.Z180;
					}
				case BoardPartsData.Rotate.Z180:
					{
						return BoardPartsData.Rotate.Z270;
					}
				case BoardPartsData.Rotate.Z270:
					{
						return BoardPartsData.Rotate.Z0;
					}
				default:
					{
						return BoardPartsData.Rotate.Z0;
					}
			}
		}

		/// <summary>
		/// ボード表示更新
		/// </summary>
		private void UpdateBoardView()
		{
			List<board.CustomizeBoardPartsView.Data> partsViewDataList = new List<board.CustomizeBoardPartsView.Data>();
			var localSaveData = GeneralRoot.User.LocalSaveData;
			var selectParts = m_userCustomizePartsList[m_selectIndex];
			var userCustomizePartsList = localSaveData.UniqueItemList.FindAll(d =>
				d.Category == UniqueItem.CategoryType.CustomizeParts);
			var customizeData = localSaveData.Customize;

			var customizePartsResource = GeneralRoot.Resource.CustomizePartsResource;
			var customizePartsMaster = GeneralRoot.Master.CustomizeParts;
			var customizePartsAreaMaster = GeneralRoot.Master.CustomizePartsArea;
			for (int i = 0; i < customizeData.BoardPartsDatas.Length; ++i)
			{
				var boardParts = customizeData.BoardPartsDatas[i];
				var userParts = userCustomizePartsList.Find(d => d.UniqueId == boardParts.UniqueId);
				var customizePartsMasterData = customizePartsMaster.Find(userParts.Id);
				var customizePartsAreaMasterData = customizePartsAreaMaster.Find(customizePartsMasterData.AreaId);
				var customizePartsResourceData = customizePartsResource.Find(customizePartsMasterData.SpriteId);
				var stateType = (boardParts.UniqueId == selectParts.UniqueId) ?
					board.CustomizeBoardPartsView.Data.Type.Selecting :
					board.CustomizeBoardPartsView.Data.Type.Seted;
				partsViewDataList.Add(new board.CustomizeBoardPartsView.Data(
					boardParts,
					customizePartsAreaMasterData.Grids,
					customizePartsResourceData.Sprite,
					stateType));
			}

			bool isNotSetPartsView = !customizeData.BoardPartsDatas.Any(d =>
				d.UniqueId == selectParts.UniqueId &&
				d.Grid == m_cacheSelectBoardPartsGrid &&
				d.Rot == m_cacheSelectBoardPartsRotate);
			if (isNotSetPartsView == true)
			{
				// 選択中パーツとして追加
				var customizePartsMasterData = customizePartsMaster.Find(selectParts.Id);
				var customizePartsAreaMasterData = customizePartsAreaMaster.Find(customizePartsMasterData.AreaId);
				var customizePartsResourceData = customizePartsResource.Find(customizePartsMasterData.SpriteId);
				var boardPartsData = new BoardPartsData(
					selectParts.UniqueId,
					m_cacheSelectBoardPartsGrid,
					m_cacheSelectBoardPartsRotate);
				var boardPartsViewData = new board.CustomizeBoardPartsView.Data(
					boardPartsData,
					customizePartsAreaMasterData.Grids,
					customizePartsResourceData.Sprite,
					board.CustomizeBoardPartsView.Data.Type.NotSet);

				// 回転によりエリアをはみ出したパーツを調整
				var useAreaGrids = boardPartsViewData.GetUseAreaGrids();
				while (true)
				{
					bool isUpdate = false;
					if (useAreaGrids.Any(d => d.x < 1) == true)
					{
						isUpdate = true;
						m_cacheSelectBoardPartsGrid.x += 1;
					}
					if (useAreaGrids.Any(d => d.x > 6) == true)
					{
						isUpdate = true;
						m_cacheSelectBoardPartsGrid.x -= 1;
					}
					if (useAreaGrids.Any(d => d.y < 1) == true)
					{
						isUpdate = true;
						m_cacheSelectBoardPartsGrid.y += 1;
					}
					if (useAreaGrids.Any(d => d.y > 6) == true)
					{
						isUpdate = true;
						m_cacheSelectBoardPartsGrid.y -= 1;
					}
					if (isUpdate == true)
					{
						boardPartsData = new BoardPartsData(
							selectParts.UniqueId,
							m_cacheSelectBoardPartsGrid,
							m_cacheSelectBoardPartsRotate);
						boardPartsViewData = new board.CustomizeBoardPartsView.Data(
							boardPartsData,
							customizePartsAreaMasterData.Grids,
							customizePartsResourceData.Sprite,
							board.CustomizeBoardPartsView.Data.Type.NotSet);
						useAreaGrids = boardPartsViewData.GetUseAreaGrids();
						continue;
					}

					break;
				}
				partsViewDataList.Add(boardPartsViewData);
			}
			m_board.UpdateView(new CustomizeBoard.Data(partsViewDataList.ToArray()));
		}

		/// <summary>
		/// パーツ一覧表示更新
		/// </summary>
		private void UpdatePartsView()
		{
			var localSaveData = GeneralRoot.User.LocalSaveData;
			var customizeData = localSaveData.Customize;

			var customizePartsMaster = GeneralRoot.Master.CustomizeParts;
			var customizePartsResource = GeneralRoot.Resource.CustomizePartsResource;
			var element = m_partsListElement.GetElements();
			if (m_userCustomizePartsList.Count <= 1)
			{
				for (int i = 0; i < element.Count; ++i)
				{
					if (i == 3)
					{
						element[i].SetActive(true);
					}
					else
					{
						element[i].SetActive(false);
						var userParts = m_userCustomizePartsList[0];
						var customizePartsMasterData = customizePartsMaster.Find(userParts.Id);
						var customizePartsResourceData = customizePartsResource.Find(customizePartsMasterData.SpriteId); 
						var partsView = element[i].GetComponent<CustomizePartsView>();
						partsView.UpdateView(new CustomizePartsView.Data(
							customizePartsResourceData.Sprite,
							customizeData.Find(userParts.UniqueId) != null));
					}
				}
			}
			else
			{
				for (int i = 0; i < element.Count; ++i)
				{
					// 0, 1, 2, 3, 4, 5, 6, 7, 8
					//-4,-3,-2,-1, 0, 1, 2, 3, 4(0を中央にするため-4)
					//-1, 0, 1, 2, 3, 4, 5, 6, 7(selectIndexにより+2)
					// 2, 0, 1, 2, 3, 4, 5, 6, 7(負数は要素数を足す+3)
					// 2, 0, 1, 2, 0, 1, 2, 0, 1(結果から要素数で割ったあまりにより%3)

					int index = (i - 4 + m_selectIndex);
					while (index < 0) { index += m_userCustomizePartsList.Count; }
					index = index % m_userCustomizePartsList.Count;
					var userParts = m_userCustomizePartsList[index];
					var customizePartsMasterData = customizePartsMaster.Find(userParts.Id);
					var customizePartsResourceData = customizePartsResource.Find(customizePartsMasterData.SpriteId);
					element[i].SetActive(true);
					var partsView = element[i].GetComponent<CustomizePartsView>();
					partsView.UpdateView(new CustomizePartsView.Data(
						customizePartsResourceData.Sprite,
						customizeData.Find(userParts.UniqueId) != null));
				}
			}

			var carouselElement = m_partsListCarousel.GetElements();
			for (int i = 0; i < carouselElement.Count; ++i)
			{
				if (i >= m_userCustomizePartsList.Count)
				{
					carouselElement[i].SetActive(false);
					continue;
   				}
				carouselElement[i].SetActive(true);
				var switchObj = carouselElement[i].GetComponent<CommonUI.SwitchObject>();
				switchObj.Setup(i == m_selectIndex);
			}

			// パーツ効果テキスト
			{
				var selectParts = m_userCustomizePartsList[m_selectIndex];
				var customizePartsMasterData = customizePartsMaster.Find(selectParts.Id);
				var customizePartsEffectMaster = GeneralRoot.Master.CustomizePartsEffect;
				var customizePartsEffectMasterData = customizePartsEffectMaster.Find(customizePartsMasterData.EffectId);
				string effectText = string.Format("速度＋{0}", customizePartsEffectMasterData.Param);
				m_partsInfoText.text = effectText;
			}
		}
	}
}