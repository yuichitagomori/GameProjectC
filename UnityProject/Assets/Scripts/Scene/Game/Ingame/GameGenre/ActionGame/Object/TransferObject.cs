using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.actiongame
{
	[System.Serializable]
	public class TransferObject : ObjectBase
	{
		public enum Type
		{
			In,
			Out,
			Break,
		}

		[SerializeField]
		private Type m_type;

		[SerializeField]
		private Texture[] m_emissionTextures;

		[SerializeField]
		private GameObject m_breakEffectObject;

		[SerializeField]
		private string m_breakSceneName;



		public new void Initialize(UnityAction<string[]> onEvent)
		{
			if (m_type == Type.Break)
			{
				var temporary = GeneralRoot.User.LocalTemporaryData;
				if (temporary.ClearSceneNameList.Contains(m_breakSceneName) == true)
				{
					m_type = Type.Out;
				}
			}

			var material = m_fbx.Models[0].Mesh.material;
			material.SetTexture("_Emission", m_emissionTextures[(int)m_type]);
			if (m_breakEffectObject != null)
			{
				m_breakEffectObject.SetActive(m_type == Type.Break);
			}

			if (m_type == Type.Out)
			{
				// イベントを発生させないようにする
				base.Initialize(null);
			}
			else
			{
				base.Initialize(onEvent);
			}
		}
	}
}
