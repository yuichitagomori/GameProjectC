using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.puzzlegame
{
    public class Cleaner : MonoBehaviour
    {
		[SerializeField]
		private FBXBase m_fbx;



		public void Initialize()
        {
            m_fbx.Anime.Play("Default");
        }

        public void Play(UnityAction callback)
        {
            m_fbx.Anime.Play("Play", callback);
        }
	}
}