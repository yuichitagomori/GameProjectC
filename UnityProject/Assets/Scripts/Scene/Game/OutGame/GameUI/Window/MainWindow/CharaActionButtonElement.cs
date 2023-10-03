using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace scene.game.outgame.window
{
    public class CharaActionButtonElement : MonoBehaviour
    {
        public enum ActionType
		{
            Active,
            Talk
		}

        [SerializeField]
        private Common.AnimatorExpansion m_anime;

        public Common.AnimatorExpansion Anime => m_anime;

        [SerializeField]
        private CommonUI.ButtonExpansion m_button;

        [SerializeField]
        private CommonUI.TextExpansion m_actionText;



        private UnityAction m_callback = null;

        public void Initialize(UnityAction callback)
		{
            m_callback = callback;
            m_button.SetupClickEvent(m_callback);
            m_button.SetupActive(true);
		}

        public void Setup(ActionType mode)
		{
		}
	}
}