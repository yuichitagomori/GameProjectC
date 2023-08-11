using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif 

namespace CommonUI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class RayCast : Graphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            vh.Clear();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RayCast))]
    class RayCastEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // インスペクターになにも表示しないようにする
        }
    }
#endif 

}