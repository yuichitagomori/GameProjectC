using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//#if UNITY_EDITOR
//using UnityEditor;
//#endif

namespace CommonUI
{
	/// <summary>
	/// イベント拡張機能
	/// </summary>
	public class ButtonExpansion : Selectable, IPointerClickHandler
	{
		/// <summary>
		/// アニメーター
		/// </summary>
		[SerializeField]
		private Common.AnimatorExpansion m_animator;

		[SerializeField]
		private GameObject m_raycast;



		private UnityAction m_callback = null;



		public void SetupClickEvent(UnityAction callback)
		{
			m_callback = callback;
		}

		public void SetupActive(bool value)
		{
			if (m_animator != null)
			{
				m_animator.Play(value ? "Enable" : "Disable", null);
			}
			m_raycast.SetActive(value);
		}

		public void OnDown()
		{
			if (m_animator != null)
			{
				m_animator.Play("Down", null);
			}
		}

		public override void OnPointerDown(PointerEventData e)
		{
			OnDown();
			base.OnPointerDown(e);
		}

		public void OnUp()
		{
			if (m_animator != null)
			{
				m_animator.Play("Up", null);
			}
		}

		public override void OnPointerUp(PointerEventData e)
		{
			OnUp();
			base.OnPointerUp(e);
		}

		public void OnClick()
		{
			if (m_callback != null)
			{
				m_callback();
			}
		}

		public void OnPointerClick(PointerEventData e)
		{
			OnClick();
		}

#if UNITY_EDITOR
		protected override void Reset()
		{
			if (m_animator == null)
			{
				m_animator = GetComponent<Common.AnimatorExpansion>();
			}

			transition = Transition.None;

			base.Reset();
		}
#endif

		//#if UNITY_EDITOR
		//	[CustomEditor(typeof(ButtonExpansion))]
		//	class ButtonExpansionEditor : Editor
		//	{
		//		public override void OnInspectorGUI()
		//		{
		//			// インスペクターになにも表示しないようにする
		//		}
		//	}
		//#endif
	}
}