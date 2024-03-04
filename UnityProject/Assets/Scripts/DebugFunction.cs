using System.IO;
using UnityEngine;

public class DebugFunction : MonoBehaviour
{
	[SerializeField]
	private RenderTexture m_captureTexture;



	[ContextMenu("Capture")]
	private void Capture()
	{
		RenderTexture.active = m_captureTexture;
		Texture2D texture = new Texture2D(m_captureTexture.width, m_captureTexture.height, TextureFormat.ARGB32, false);
		texture.ReadPixels(new Rect(0, 0, m_captureTexture.width, m_captureTexture.height), 0, 0);
		texture.Apply();
		RenderTexture.active = null;
		var color = texture.GetPixels();
		for (int i = 0; i < color.Length; i++)
		{
			color[i].r = Mathf.Pow(color[i].r, 1.0f / 2.2f);
			color[i].g = Mathf.Pow(color[i].g, 1.0f / 2.2f);
			color[i].b = Mathf.Pow(color[i].b, 1.0f / 2.2f);
		}
		texture.SetPixels(color);
		byte[] bytes = texture.EncodeToPNG();
		File.WriteAllBytes(Application.streamingAssetsPath + "/CaptureTexture.png", bytes);
    }
}
