using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.world
{
	[System.Serializable]
	public class ObjectBase : ActionTargetBase
	{
		[SerializeField]
		private Transform m_faceChangerTransform = null;

		[SerializeField]
		private Material m_faceMaterial = null;




		private Transform m_transform = null;
		//public new Transform transform => m_transform;

		private int m_objectId = -1;
		//public int ObjectId => m_objectId;

		private string m_defaultActionEventParam = "";



		public void Initialize(
			ObjectController.Data.ObjectType type,
			int controllId,
			ObjectController.Data.Parameter param,
			UnityAction<string> eventCallback)
		{
			m_controllId = controllId;

			EventBase.Data[] eventDatas = new EventBase.Data[]
			{
				new EventBase.Data(
					EventBase.Data.Type.Enter,
					string.Format("SearchIn_{0}_{1}", (int)Category.Object, m_controllId),
					new EventBase.Data.ColliderFilter[]{ EventBase.Data.ColliderFilter.SearchIn },
					false),
				new EventBase.Data(
					EventBase.Data.Type.Exit,
					string.Format("SearchOut_{0}_{1}", (int)Category.Object, m_controllId),
					new EventBase.Data.ColliderFilter[]{ EventBase.Data.ColliderFilter.SearchOut },
					false),
			};
			m_event.Initialize(eventDatas, eventCallback);
			m_transform = base.transform;
			if (param != null)
			{
				param.TransformData.SetupTransform(m_transform);
			}
			m_transformPosition = m_transform.position;

			m_defaultActionEventParam = param.ActionEventParam;

			m_fbx.Anime.PlayLoop("Wait");
		}

		private void FixedUpdate()
		{
			if (m_faceChangerTransform != null)
			{
				// enable = false 状態でも表情切り替えを行いたいので、手前で更新処理をはさむ
				int offsetUIndex = Mathf.RoundToInt(m_faceChangerTransform.localPosition.x * 100);
				int offsetVIndex = Mathf.RoundToInt(m_faceChangerTransform.localPosition.y * 100);
				float offsetU = 0.25f * offsetUIndex;
				float offsetV = 0.25f * offsetVIndex;
				m_faceMaterial.SetVector("_UVOffset", new Vector4(offsetU, offsetV, 0.0f, 0.0f));
			}
		}

		public override string GetActionEventParam()
		{
			if (string.IsNullOrEmpty(m_defaultActionEventParam) == false)
			{
				return m_defaultActionEventParam;
			}

			int subCharaId = 30;
			int controllId = 7;
			return string.Format("AddTarget_{0}_{1}",
				subCharaId,
				controllId);
		}

		public override void SearchIn()
		{
			Debug.Log("Object SearchIn");
		}

		public override void SearchOut()
		{
			Debug.Log("Object SearchOut");
		}

		public override void OnCharaActionButtonPressed(UnityAction callback)
		{
			// 特になし
			if (callback != null)
			{
				callback();
			}
		}

		public override void PlayReaction(
			IngameWorld.ReactionType type,
			UnityAction loopEffectOutEvent,
			UnityAction callback)
		{
			// 特になし
			if (callback != null)
			{
				callback();
			}
		}
	}
}
