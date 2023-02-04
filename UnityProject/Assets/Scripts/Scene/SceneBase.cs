using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace scene
{
	/// <summary>
	/// シーン基底クラス
	/// </summary>
	public abstract class SceneBase : MonoBehaviour
	{
		[Header("SceneBase")]

		/// <summary>
		/// キャンバス
		/// </summary>
		[SerializeField]
		private Canvas m_canvas;

		/// <summary>
		/// キャンバス親オブジェクト
		/// </summary>
		[SerializeField]
		private CanvasGroup m_topCanvasGroup;
		public CanvasGroup TopCanvasGroup { get { return m_topCanvasGroup; } }

		/// <summary>
		/// 翻訳をかけるテキストリスト
		/// </summary>
		[SerializeField]
		private List<LocalizeText> m_localizeTextList;



		/// <summary>
		/// 終了時イベント
		/// </summary>
		private UnityAction<SceneBase, UnityAction> m_finishEvent;



		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="finishEvent"></param>
		public void Initialize(UnityAction<SceneBase, UnityAction> finishEvent)
		{
			if (m_canvas != null)
			{
				m_canvas.worldCamera = GeneralRoot.Instance.GetOutgameCamera();
			}
			m_finishEvent = finishEvent;

			for (int i = 0; i < m_localizeTextList.Count; ++i)
			{
				m_localizeTextList[i].SetText();
			}
		}

		/// <summary>
		/// 遷移アニメーション開始時準備
		/// </summary>
		/// <param name="callback"></param>
		public abstract void Ready(UnityAction callback);

		/// <summary>
		/// 遷移アニメーション終了
		/// </summary>
		public abstract void Go();

		/// <summary>
		/// 終了
		/// </summary>
		/// <param name="callback"></param>
		public virtual void Finish(UnityAction callback)
		{
			if (m_finishEvent != null)
			{
				m_finishEvent(this, callback);
			}
			else
			{
				callback();
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// テキストリスト設定
		/// </summary>
		/// <param name="localizeTextList"></param>
		public void SetTextList(List<LocalizeText> localizeTextList)
		{
			m_localizeTextList = localizeTextList;
		}
#endif
	}
}