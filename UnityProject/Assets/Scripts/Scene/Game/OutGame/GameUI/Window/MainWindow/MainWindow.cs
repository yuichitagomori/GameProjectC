using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.outgame.window
{
    [System.Serializable]
    public class MainWindow : WindowBase
	{
		[SerializeField]
		private Transform m_sequenceViewParent;

		[SerializeField]
		private GameObject[] m_sequenceViewPrefabs;



		private List<KeyCode> m_downKeyList = new List<KeyCode>();

		private UnityAction<KeyCode[]> m_inputEvent;

		private main.SequenceViewBase m_sequenceView;

		public void Setting(
			Game.GenreType genreType,
			UnityAction<KeyCode[]> inputEvent)
		{
			var prefab = m_sequenceViewPrefabs[(int)genreType];
			var sequenceViewObject = GameObject.Instantiate(prefab, m_sequenceViewParent);
			m_sequenceView = sequenceViewObject.GetComponent<main.SequenceViewBase>();
			m_sequenceView.Setting();

			m_downKeyList.Clear();
			m_inputEvent = inputEvent;
		}

		public override void Go()
		{
			StartCoroutine(GoCoroutine());
		}

		private IEnumerator GoCoroutine()
		{
			while (true)
			{
				m_inputEvent(m_downKeyList.ToArray());

				yield return null;
			}
		}

		public override void OnMovieStart(string[] paramStrings, UnityAction callback)
		{
			StartCoroutine(m_sequenceView.OnMovieStartCoroutine(paramStrings, callback));
		}

		public override void SetupInputKeyEvent()
		{
			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Down, KeyCode.W, () =>
			{
				if (m_downKeyList.Contains(KeyCode.W) == false)
				{
					m_downKeyList.Add(KeyCode.W);
				}
				SetTopSibling();
			});
			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.W, () =>
			{
				if (m_downKeyList.Contains(KeyCode.W) == true)
				{
					m_downKeyList.Remove(KeyCode.W);
				}
			});

			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Down, KeyCode.S, () =>
			{
				if (m_downKeyList.Contains(KeyCode.S) == false)
				{
					m_downKeyList.Add(KeyCode.S);
				}
				SetTopSibling();
			});
			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.S, () =>
			{
				if (m_downKeyList.Contains(KeyCode.S) == true)
				{
					m_downKeyList.Remove(KeyCode.S);
				}
			});

			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Down, KeyCode.A, () =>
			{
				if (m_downKeyList.Contains(KeyCode.A) == false)
				{
					m_downKeyList.Add(KeyCode.A);
				}
				SetTopSibling();
			});
			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.A, () =>
			{
				if (m_downKeyList.Contains(KeyCode.A) == true)
				{
					m_downKeyList.Remove(KeyCode.A);
				}
			});

			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Down, KeyCode.D, () =>
			{
				if (m_downKeyList.Contains(KeyCode.D) == false)
				{
					m_downKeyList.Add(KeyCode.D);
				}
				SetTopSibling();
			});
			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.D, () =>
			{
				if (m_downKeyList.Contains(KeyCode.D) == true)
				{
					m_downKeyList.Remove(KeyCode.D);
				}
			});

			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Down, KeyCode.Space, () =>
			{
				if (m_downKeyList.Contains(KeyCode.Space) == false)
				{
					m_downKeyList.Add(KeyCode.Space);
				}
				SetTopSibling();
			});
			GeneralRoot.Input.UpdateEvent(system.InputSystem.Type.Up, KeyCode.Space, () =>
			{
				if (m_downKeyList.Contains(KeyCode.Space) == true)
				{
					m_downKeyList.Remove(KeyCode.Space);
				}
			});
		}
	}
}
