﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.actiongame
{
	[System.Serializable]
	public class GuideObject : ObjectBase
	{
		[SerializeField]
		private Texture m_texture;



		public void Initialize()
		{
			m_fbx.Anime.Play("Default");
			m_fbx.Models[0].Mesh.material.SetTexture("_Texture", m_texture);
			base.Initialize(OnEvent);
		}

		private void OnEvent(string[] eventParams)
		{
			for (int i = 0; i < eventParams.Length; ++i)
			{
				string[] eventParamStrings = eventParams[i].Split(',');
				switch (eventParamStrings[0])
				{
					case "In":
						{
							m_fbx.Anime.Play("In");
							break;
						}
					case "Out":
						{
							m_fbx.Anime.Play("Out");
							break;
						}
				}
			}
		}
	}
}
