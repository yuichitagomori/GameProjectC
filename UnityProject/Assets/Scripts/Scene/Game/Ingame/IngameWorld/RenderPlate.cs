using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scene.game.ingame.world
{
	[System.Serializable]
	public class RenderPlate : MonoBehaviour
	{
		[SerializeField]
		private FBXBase m_fbx = null;



		private Transform m_transform = null;
		public new Transform transform => m_transform;



		public void Initialize()
		{
			m_fbx.Anime.Play("Ready");
			m_transform = base.transform;
		}

		public IEnumerator SetActiveColoutine(bool _value)
		{
			if (_value == true)
			{
				bool isDone = false;
				m_fbx.Anime.Play("In", () => { isDone = true; });
				while (!isDone) { yield return null; }
				m_fbx.Anime.PlayLoop("Wait");
			}
			else
			{
				bool isDone = false;
				m_fbx.Anime.Play("Out", () => { isDone = true; });
				while (!isDone) { yield return null; }
				m_fbx.Anime.Play("Ready");
			}
		}
	}
}