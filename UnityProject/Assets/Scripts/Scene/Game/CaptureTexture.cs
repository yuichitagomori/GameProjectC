using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace scene
{
	public class CaptureTexture : MonoBehaviour
	{
		[SerializeField]
		private Camera m_camera;

		[SerializeField]
		private string m_savePath;

		[SerializeField]
		private string m_saveName;

		[SerializeField]
		private int m_captureTextureWidth;

		[SerializeField]
		private int m_captureTextureHeight;



		[ContextMenu("CaptureFromCamera")]
		private void CaptureFromCamera()
		{
			var render = new RenderTexture(
				m_captureTextureWidth,
				m_captureTextureHeight,
				24);
			var texture = new Texture2D(
				m_captureTextureWidth,
				m_captureTextureHeight,
				TextureFormat.RGB24, //TextureFormat.ARGB32,
				false);

			m_camera.targetTexture = render;
			m_camera.Render();

			RenderTexture.active = m_camera.targetTexture;
			texture.ReadPixels(new Rect(0, 0, m_captureTextureWidth, m_captureTextureHeight), 0, 0);
			texture.Apply();

			m_camera.targetTexture = null;
			RenderTexture.active = null;

			byte[] bytes = texture.EncodeToPNG();
			string savePath = Application.streamingAssetsPath + m_savePath + "/" + m_saveName  + ".png";
			File.WriteAllBytes(savePath, bytes);

			Destroy(texture);
		}
	}
}