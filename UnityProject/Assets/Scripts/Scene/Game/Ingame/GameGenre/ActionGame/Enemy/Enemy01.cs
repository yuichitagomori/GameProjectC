using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.actiongame
{
	[System.Serializable]
	public class Enemy01 : EnemyBase
	{
		[SerializeField]
		private float m_waitTime;



		public override void Initialize(UnityAction<string[]> onEvent)
		{
			StartCoroutine(MoveCoroutine());

			base.Initialize(onEvent);
		}

		private IEnumerator MoveCoroutine()
		{
			m_colliderObject.SetActive(false);

			var wait = new WaitForSeconds(m_waitTime);
			while (true)
			{
				bool isDone = false;
				m_fbx.Anime.Play("AttackIn", () => { isDone = true; });
				while (!isDone) { yield return null; }

				m_colliderObject.SetActive(true);
				isDone = false;
				m_fbx.Anime.Play("AttackOut", () => { isDone = true; });
				while (!isDone) { yield return null; }
				m_colliderObject.SetActive(false);

				m_fbx.Anime.Play("Default");

				yield return wait;
			}
		}
	}
}
