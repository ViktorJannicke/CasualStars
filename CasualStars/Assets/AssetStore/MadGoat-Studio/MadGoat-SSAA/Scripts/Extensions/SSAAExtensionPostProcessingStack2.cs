using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif
namespace MadGoat.SSAA {
    [System.Serializable]
    public class SSAAExtensionPostProcessingStack2 : SSAAExtensionBase {
        public static string description = "Enables support for Unity's Postprocessing Stack v2." +
                                    "\n   • updateFromOriginal - if enabled, the SSAA camera PPLayer settings will always be synced with this cameras PPLayer (if available)" +
                                    "\n   • lwrpLegacySupport - enables features which are missing by default on SSAA with legacy LWRP";
        public static string requirement = "- Requires Post Processing Stack v2 package" +
                                    "\n- Requires Built-In Pipeline or LWRP 2019.2 or lower";

        public override bool IsSupported() {
#if UNITY_POST_PROCESSING_STACK_V2 && SSAA_URP && !UNITY_2019_3_OR_NEWER
            return true;
#elif UNITY_POST_PROCESSING_STACK_V2 && !SSAA_HDRP && !SSAA_URP
            return true;
#else
            return false;
#endif

        }
#if (UNITY_POST_PROCESSING_STACK_V2 && !SSAA_HDRP && !SSAA_URP) || (UNITY_POST_PROCESSING_STACK_V2 && SSAA_URP && !UNITY_2019_3_OR_NEWER)
        public bool updateFromOriginal = true;
        public bool lwrpLegacySupport = true;

        private PostProcessLayer ppLayerCurrent;
        private PostProcessLayer ppLayerRenderer;
        public override void OnInitialize(MadGoatSSAA ssaaInstance) {
            base.OnInitialize(ssaaInstance); 
            if (!enabled) return;

            ppLayerCurrent = ssaaInstance.CurrentCamera.GetComponent<PostProcessLayer>();
            ppLayerRenderer = ssaaInstance.RenderCamera.GetComponent<PostProcessLayer>();
            if (ppLayerRenderer == null) ppLayerRenderer = ssaaInstance.RenderCamera.gameObject.AddComponent<PostProcessLayer>();
            
            // Handle enabled states
            ppLayerRenderer.enabled = true;
            if(ppLayerCurrent && ppLayerCurrent.enabled) ppLayerCurrent.enabled = false;
        }
        public override void OnUpdate(MadGoatSSAA ssaaInstance) {
            if (!enabled) return;

            if (ppLayerCurrent && ppLayerCurrent.enabled) ppLayerCurrent.enabled = false;

            if (updateFromOriginal && ppLayerCurrent) {
                ppLayerRenderer.antialiasingMode = ppLayerCurrent.antialiasingMode;
                ppLayerRenderer.breakBeforeColorGrading = ppLayerCurrent.breakBeforeColorGrading;
#if UNITY_2019_1_OR_NEWER
                ppLayerRenderer.finalBlitToCameraTarget = ppLayerCurrent.finalBlitToCameraTarget;
#endif
                ppLayerRenderer.fog = ppLayerCurrent.fog;
                ppLayerRenderer.stopNaNPropagation = ppLayerCurrent.stopNaNPropagation;
                ppLayerRenderer.volumeTrigger = ppLayerCurrent.transform;
                ppLayerRenderer.volumeLayer = ppLayerCurrent.volumeLayer; 
            }

#if UNITY_2019_3_OR_NEWER && SSAA_URP
            // not supported on URP
#elif UNITY_2019_1_OR_NEWER && SSAA_URP
            // LWRP
            if(ssaaInstance.PostAntiAliasing == PostAntiAliasingMode.FSSAA && lwrpLegacySupport) {
                ppLayerRenderer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
                ppLayerRenderer.fastApproximateAntialiasing.keepAlpha = true;
            }
            else {
                ppLayerRenderer.antialiasingMode = PostProcessLayer.Antialiasing.None;
            } 
#endif
        }
        public override void OnDeinitialize(MadGoatSSAA ssaaInstance) {
            base.OnDeinitialize(ssaaInstance);

            if (!enabled) return;

            // Handle enabled states
            if (ppLayerCurrent && !ppLayerCurrent.enabled) ppLayerCurrent.enabled = true;
            if (ppLayerRenderer && ppLayerRenderer.enabled) ppLayerRenderer.enabled = false;
        }
#endif
    }
}