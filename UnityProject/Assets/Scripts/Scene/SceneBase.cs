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

		[SerializeField]
		private string m_sceneName;
		public string SceneName
		{
			get
			{
				if (string.IsNullOrEmpty(m_sceneName) == false)
				{
					return m_sceneName;
				}
				return GetType().Name;
			}
		}

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
		/// シーン管理
		/// </summary>
		protected scene.SceneController m_sceneController;



		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="sceneController"></param>
		public void Initialize(scene.SceneController sceneController)
		{
			m_sceneController = sceneController;

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
			if (callback != null)
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