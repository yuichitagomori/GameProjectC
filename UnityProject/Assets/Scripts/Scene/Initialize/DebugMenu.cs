using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scene
{
    public class DebugMenu : MonoBehaviour
    {
        private enum Type
		{
            FPS,
            SetPassCalls,
            DrawCalls,
            Triangles,
            Vertices,
        }

        [SerializeField]
        private Common.ElementList m_element;
        //public Common.ElementList Element => m_element;

		public void Show()
		{
			StartCoroutine(UpdateCoroutine());
		}

		private IEnumerator UpdateCoroutine()
		{
			var setPassCallsRecorder = Unity.Profiling.ProfilerRecorder.StartNew(
				Unity.Profiling.ProfilerCategory.Render,
				"SetPass Calls Count");
			var drawCallsRecorder = Unity.Profiling.ProfilerRecorder.StartNew(
				Unity.Profiling.ProfilerCategory.Render,
				"Draw Calls Count");
			var trianglesRecorder = Unity.Profiling.ProfilerRecorder.StartNew(
				Unity.Profiling.ProfilerCategory.Render,
				"Triangles Count");
			var verticesRecorder = Unity.Profiling.ProfilerRecorder.StartNew(
				Unity.Profiling.ProfilerCategory.Render,
				"Vertices Count");

			var elements = m_element.GetElements();
			List<initialize.DebugParamElement> debugParamList = new List<initialize.DebugParamElement>();
			for (int i = 0; i < elements.Count; ++i)
			{
				debugParamList.Add(elements[i].GetComponent<initialize.DebugParamElement>());
			}

			var wait = new WaitForSeconds(0.5f);

			while (true)
			{
				float fps = 1f / Time.deltaTime;
				var fpsParam = debugParamList[(int)Type.FPS];
				fpsParam.Title.text = "FPS";
				fpsParam.Param.text = string.Format("{0:F4}", fps);

				var setPassCallsParam = debugParamList[(int)Type.SetPassCalls];
				setPassCallsParam.Title.text = "SetPass Calls";
				if (setPassCallsRecorder.Valid == true)
				{
					setPassCallsParam.Param.text = string.Format("{0}", setPassCallsRecorder.LastValue);
				}
				else
				{
					setPassCallsParam.Param.text = string.Format("{0}", "-");
				}

				var drawCallsParam = debugParamList[(int)Type.DrawCalls];
				drawCallsParam.Title.text = "Draw Calls";
				if (drawCallsRecorder.Valid == true)
				{
					drawCallsParam.Param.text = string.Format("{0}", drawCallsRecorder.LastValue);
				}
				else
				{
					drawCallsParam.Param.text = string.Format("{0}", "-");
				}

				var trianglesParam = debugParamList[(int)Type.Triangles];
				trianglesParam.Title.text = "Triangles";
				if (trianglesRecorder.Valid == true)
				{
					trianglesParam.Param.text = string.Format("{0}", trianglesRecorder.LastValue);
				}
				else
				{
					trianglesParam.Param.text = string.Format("{0}", "-");
				}

				var verticesParam = debugParamList[(int)Type.Vertices];
				verticesParam.Title.text = "Vertices";
				if (verticesRecorder.Valid == true)
				{
					verticesParam.Param.text = string.Format("{0}", verticesRecorder.LastValue);
				}
				else
				{
					verticesParam.Param.text = string.Format("{0}", "-");
				}

				yield return wait;
			}
		}
	}
}