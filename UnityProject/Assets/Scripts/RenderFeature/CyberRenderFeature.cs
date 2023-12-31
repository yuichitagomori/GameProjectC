using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace render
{
	public class CyberRenderFeature : ScriptableRendererFeature
	{
		class CyberRenderPass : ScriptableRenderPass
		{
			private const string k_profilerTag = nameof(CyberRenderPass);
			private readonly int m_mainTextureId = Shader.PropertyToID("_MainTex");



			private Material m_material;

			public void Initialize(RenderPassEvent renderPass, Material material)
			{
				renderPassEvent = renderPass;
				m_material = material;
			}

			/// <summary>
			/// レンダリング処理前
			/// </summary>
			/// <param name="cmd"></param>
			/// <param name="cameraTextureDescriptor"></param>
			public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
			{
			}

			/// <summary>
			/// レンダリング処理
			/// </summary>
			/// <param name="context"></param>
			/// <param name="renderingData"></param>
			public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
			{
				if (m_material == null)
				{
					Debug.LogError("RenderFeature Execute : Material null");
					return;
				}
				//var camera = renderingData.cameraData.camera;
				//if (camera.targetTexture == null)
				//{
				//	Debug.LogError("RenderFeature Execute : camera.targetTexture null");
				//	return;
				//}
				//Debug.Log("camera.targetTexture = " + camera.targetTexture.name);

				
				var cmd = CommandBufferPool.Get(k_profilerTag);
				var cameraColorTarget = renderingData.cameraData.renderer.cameraColorTarget;
				cmd.Blit(cameraColorTarget, cameraColorTarget, m_material);
				context.ExecuteCommandBuffer(cmd);
				//CommandBufferPool.Release(cmd);
			}

			/// <summary>
			/// レンダリング処理後
			/// </summary>
			/// <param name="cmd"></param>
			public override void FrameCleanup(CommandBuffer cmd)
			{
			}
		}

		[SerializeField]
		private RenderPassEvent m_renderPassEvent;

		[SerializeField]
		private Material m_renderMaterial;



		private CyberRenderPass m_pass;



		public override void Create()
		{
			m_pass = new CyberRenderPass();
			m_pass.Initialize(m_renderPassEvent, m_renderMaterial);
		}

		public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
		{
			renderer.EnqueuePass(m_pass);
		}
	}
}
