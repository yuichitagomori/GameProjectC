using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scene.initialize
{
    public class DebugParamElement : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Text m_title;
        public UnityEngine.UI.Text Title => m_title;

        [SerializeField]
        private UnityEngine.UI.Text m_param;
        public UnityEngine.UI.Text Param => m_param;
    }
}