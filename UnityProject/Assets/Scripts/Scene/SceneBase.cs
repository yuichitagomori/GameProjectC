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
		/// シーン管理
		/// </summary>
		protected scene.SceneController m_sceneController;



		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="sceneController"></param>
		public void Initialize(SceneController sceneController)
		{
			m_sceneController = sceneController;
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
	}
}