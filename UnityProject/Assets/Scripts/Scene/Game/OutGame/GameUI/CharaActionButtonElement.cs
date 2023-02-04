using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace scene.game.outgame
{
    public class CharaActionButtonElement : MonoBehaviour
    {
        public enum ActionType
		{
            Active,
            Talk
		}

        [SerializeField]
        private AnimatorExpansion m_anime;

        public AnimatorExpansion Anime => m_anime;

        [SerializeField]
        private CommonUI.ButtonExpansion m_button;

        [SerializeField]
        private Image m_actionIconImage;

        [SerializeField]
        private Sprite[] m_actionIconSprites;



        private UnityAction m_callback = null;

        public void Initialize(UnityAction callback)
		{
            m_callback = callback;
            m_button.SetupClickEvent(m_callback);
		}

        public void Setup(ActionType mode)
		{
            m_actionIconImage.sprite = m_actionIconSprites[(int)mode];
		}
	}
}