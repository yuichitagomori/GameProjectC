using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace system
{
	public class PosproController : MonoBehaviour
	{
		[System.Serializable]
		public class Data
		{
			[SerializeField]
			private bool m_enableBloom;
			public bool EnableBloom => m_enableBloom;

			[SerializeField]
			private bool m_enableVignette;
			public bool EnableVignette => m_enableVignette;

			[SerializeField]
			private Color m_vignetteColor;
			public Color VignetteColor => m_vignetteColor;

			[SerializeField]
			private bool m_enableDepthOfField;
			public bool EnableDepthOfField => m_enableDepthOfField;
		}

		/// <summary>
		/// É{ÉäÉÖÅ[ÉÄ
		/// </summary>
		[SerializeField]
		private Volume m_volume;




		public void Setting(Data data)
		{
			SettingBloom(data.EnableBloom);
			SettingVignette(data.EnableVignette, data.VignetteColor);
			SettingDepthOfField(data.EnableDepthOfField);
		}

		private void SettingBloom(bool value)
		{
			Bloom bloom = null;
			if (m_volume.profile.TryGet(out bloom) == false)
			{
				return;
			}
			bloom.active = value;
		}

		private void SettingVignette(bool value, Color color)
		{
			Vignette vignette = null;
			if (m_volume.profile.TryGet(out vignette) == false)
			{
				return;
			}
			vignette.active = value;
			vignette.color.value = color;
		}

		private void SettingDepthOfField(bool value)
		{
			DepthOfField dof = null;
			if (m_volume.profile.TryGet(out dof) == false)
			{
				return;
			}
			dof.active = value;
		}
	}
}
