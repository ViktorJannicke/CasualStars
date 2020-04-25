using UnityEngine;
using UnityEngine.Rendering;
#if SSAA_URP
#if UNITY_2019_3_OR_NEWER
using UnityEngine.Rendering.Universal;
#elif UNITY_2019_1_OR_NEWER
using UnityEngine.Rendering.LWRP;
#endif
#endif

#if SSAA_URP
public class SSAARenderPassFeature : ScriptableRendererFeature
{
    class SSAARenderPass : ScriptableRenderPass
    {
     
        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var ssaa = renderingData.cameraData.camera.GetComponent<MadGoat.SSAA.MadGoatSSAA>();

            if (ssaa == null) return;
            else if (!ssaa.enabled) return;


            var isVR = ssaa.GetType() == typeof(MadGoat.SSAA.MadGoatSSAA_VR);
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying && isVR) return;
#endif
            // other pass goes here
            var material = ssaa.MaterialCurrent;
            if(material == null) return;
            // set blending - no stacking support on lwrp/urp
            material.SetOverrideTag("RenderType", "Opaque");
            material.SetInt("_SrcBlend", (int)BlendMode.One);
            material.SetInt("_DstBlend", (int)BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.renderQueue = -1;

            // update values
            material.SetFloat("_ResizeWidth", ssaa.CameraTargetWidth);
            material.SetFloat("_ResizeHeight", ssaa.CameraTargetHeight);
            material.SetFloat("_Sharpness", ssaa.DownsamplerSharpness);
            material.SetFloat("_SampleDistance", ssaa.DownsamplerDistance);

            if (!isVR) {
                // setup command buffer
                var cmd = CommandBufferPool.Get("SSAA_HDRP_DOWNSAMPLER");
                CoreUtils.ClearRenderTarget(cmd, ClearFlag.All, Color.clear);
                CoreUtils.SetRenderTarget(cmd, BuiltinRenderTextureType.CurrentActive);

                Blit(cmd, ssaa.RenderCamera.targetTexture, BuiltinRenderTextureType.CurrentActive, material, 0);

                // execute command buffer
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
            else {
                // breaks the pipeline
                // --- 
                //// setup command buffer
                //var cmd = CommandBufferPool.Get("SSAA_HDRP_DOWNSAMPLER");
                //CoreUtils.ClearRenderTarget(cmd, ClearFlag.All, Color.clear);
                //CoreUtils.SetRenderTarget(cmd, BuiltinRenderTextureType.CurrentActive);
                 
                //RenderTargetIdentifier ssaaCommandBufferTargetId = new RenderTargetIdentifier(rtDownsampler);

                //Blit(cmd, BuiltinRenderTextureType.CameraTarget, ssaaCommandBufferTargetId, material, 0);
                //Blit(cmd, ssaaCommandBufferTargetId, BuiltinRenderTextureType.CameraTarget);
     
                //// execute command buffer
                //context.ExecuteCommandBuffer(cmd);
                //CommandBufferPool.Release(cmd); 
            }
        } 
    }

    SSAARenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new SSAARenderPass();

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    } 
}
#endif
