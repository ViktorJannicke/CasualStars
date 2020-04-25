using System;
using System.Collections;
using System.Collections.Generic;
using MadGoat.Core.Utils;
using UnityEngine;
using UnityEngine.Rendering;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if SSAA_URP
#if UNITY_2019_3_OR_NEWER
using UnityEngine.Rendering.Universal;
#elif UNITY_2019_1_OR_NEWER
using UnityEngine.Rendering.LWRP;
#endif
#endif

#if SSAA_HDRP
#if UNITY_2019_3_OR_NEWER
using UnityEngine.Rendering.HighDefinition;
#elif UNITY_2019_1_OR_NEWER
using UnityEngine.Experimental.Rendering.HDPipeline;  
#endif
#endif

namespace MadGoat.SSAA {
    [RequireComponent(typeof(Camera))]
    public class MadGoatSSAA_VR : MadGoatSSAA {

        #region Properties
        public PostAntiAliasingMode SsaaUltraOld { get; set; }
        private float VRCachedRenderScale { get; set; }
        #endregion

        #region MonoBehaviour Event Methods
        private void Start() {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return;
#endif 
            if (Initialized) return;
            OnInitializeProps();
            StartCoroutine(UpdateAdaptiveRes());
        }
        private void OnEnable() {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return;
#endif

            // Handle original scaling
#if UNITY_2017_2_OR_NEWER
            if (!UnityEngine.XR.XRDevice.isPresent)
                throw new Exception("VRDevice not present or not detected");
            VRCachedRenderScale = UnityEngine.XR.XRSettings.eyeTextureResolutionScale;
#else
            if (!UnityEngine.VR.VRDevice.isPresent)
                throw new Exception("VRDevice not present or not detected");
            VRCachedRenderScale = UnityEngine.VR.VRSettings.renderScale;
#endif

            if (!Initialized) Start();
            else if (Pipeline == RenderPipelineUtils.PipelineType.BuiltInPipeline) SetupDownsamplerCommandBuffer();
        }
        private void Update() {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return;
#endif
            PerfSampler.Update();

            // Update the materials properties
            if (Pipeline == RenderPipelineUtils.PipelineType.BuiltInPipeline && Initialized) {
                // Change the material by the filter type
                ChangeMaterial(DownsamplerFilter);
                UpdateDownsamplerCommandBuffer();
            }

#if SSAA_HDRP
            OnBeginCameraRender(CurrentCamera);
#endif
        }
        private void OnDisable() {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return;
#endif

            // Handle VR device cleaning - reset to original scaling
#if UNITY_2017_2_OR_NEWER
            UnityEngine.XR.XRSettings.eyeTextureResolutionScale = VRCachedRenderScale;
#else
            UnityEngine.VR.VRSettings.renderScale = VRCachedRenderScale;
#endif

            // Handle command buffer cleaning
            if (Pipeline == RenderPipelineUtils.PipelineType.BuiltInPipeline) {
                ClearDownsamplerCommandBuffer();
            }
        }
        #endregion

        #region SSAA Event Implementation
        public override void OnBeginCameraRender(Camera cam) {
            if (cam != currentCamera || !enabled)
                return;

#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
                return;
#endif

            try {

#if SSAA_HDRP
                if (InternalRenderMultiplier > 1 && PostAntiAliasing == PostAntiAliasingMode.FSSAA && InternalRenderMode != RenderMode.AdaptiveResolution) {
                    HdCamDataCurrent.antialiasing = HDAdditionalCameraData.AntialiasingMode.FastApproximateAntialiasing;
                }
                else if (InternalRenderMultiplier > 1 && PostAntiAliasing == PostAntiAliasingMode.TSSAA && InternalRenderMode != RenderMode.AdaptiveResolution) {
                    HdCamDataCurrent.antialiasing = HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing;
                }
                else {
                    HdCamDataCurrent.antialiasing = HDAdditionalCameraData.AntialiasingMode.None;
                }
#endif
#if SSAA_URP && UNITY_2019_3_OR_NEWER
                if (InternalRenderMultiplier > 1 && PostAntiAliasing == PostAntiAliasingMode.FSSAA && InternalRenderMode != RenderMode.AdaptiveResolution) {
                    UniversalCamDataCurrent.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
                }
                else {
                    UniversalCamDataCurrent.antialiasing = AntialiasingMode.None;
                }
#endif

                // Handle the resolution multiplier
#if UNITY_2017_2_OR_NEWER
                if (!UnityEngine.XR.XRDevice.isPresent)
                    throw new Exception("VRDevice not present or not detected");
                UnityEngine.XR.XRSettings.eyeTextureResolutionScale = InternalRenderMultiplier;
#else
                if (!UnityEngine.VR.VRDevice.isPresent)
                    throw new Exception("VRDevice not present or not detected");
                UnityEngine.VR.VRSettings.renderScale = InternalRenderMultiplier;
#endif
            }
            catch (Exception ex) {
                Debug.LogError("Something went wrong. SSAA has been set to off and the plugin was disabled");
                Debug.LogError(ex);
                SetAsSSAA(SSAAMode.SSAA_OFF);
                enabled = false;
            }

        }
        protected override void OnInitialize() {
            if (currentCamera == null)
                currentCamera = GetComponent<Camera>();

#if UNITY_2017_2_OR_NEWER
            UnityEngine.XR.XRSettings.eyeTextureResolutionScale = InternalRenderMultiplier;
#else
            UnityEngine.VR.VRSettings.renderScale = InternalRenderMultiplier;
#endif 
            MaterialCurrent = MaterialDefault;
            MaterialOld = MaterialCurrent;

            if (Pipeline == RenderPipelineUtils.PipelineType.BuiltInPipeline)
                SetupDownsamplerCommandBuffer();
#if SSAA_URP && UNITY_2019_3_OR_NEWER
            UniversalCamDataCurrent = GetComponent<UniversalAdditionalCameraData>();

            if (!UniversalCamDataCurrent) UniversalCamDataCurrent = gameObject.AddComponent<UniversalAdditionalCameraData>();
#elif SSAA_URP
            LwrpCamDataCurrent = GetComponent<LWRPAdditionalCameraData>();

            if (!LwrpCamDataCurrent) LwrpCamDataCurrent = gameObject.AddComponent<LWRPAdditionalCameraData>();

#elif SSAA_HDRP
            HdCamDataCurrent = GetComponent<HDAdditionalCameraData>();

            if (!HdCamDataCurrent) HdCamDataCurrent = gameObject.AddComponent<HDAdditionalCameraData>();
#endif 
            Initialized = true;
        }
        protected void ChangeMaterial(Filter Type) {

            // Point material_current to the given material
            switch (Type) {
                case Filter.POINT:
                    MaterialCurrent = MaterialDefault;
                    break;
                case Filter.BILINEAR:
                    MaterialCurrent = MaterialBilinear;
                    break;
                case Filter.BICUBIC:
                    MaterialCurrent = MaterialBicubic;
                    break;
            }

            // Hanle the correct pass
            if ((!DownsamplerEnabled || InternalRenderMultiplier == 1) && MaterialCurrent != MaterialDefault) {
                MaterialCurrent = MaterialDefault;
            }
            // if material must be changed we have to reset the command buffer
            if (MaterialCurrent != MaterialOld || SsaaUltraOld != PostAntiAliasing) {
                SsaaUltraOld = PostAntiAliasing;
                MaterialOld = MaterialCurrent;
                ClearDownsamplerCommandBuffer();
                SetupDownsamplerCommandBuffer();
            }

        }
        #endregion

        #region MadGoat SSAA Standard Pipeline Core Implementation
        protected void SetupDownsamplerCommandBuffer() {
            if (CbDownsampler == null) CbDownsampler = new CommandBuffer();
            if ((new List<CommandBuffer>(currentCamera.GetCommandBuffers(constCamEventDownsampler))).Find(x => x.name == "SSAA_VR_APPLY") == null) {
                // set up buffer rt
                if (RtDownsampler) RtDownsampler.Release();

#if UNITY_2017_2_OR_NEWER
                RtDownsampler = new RenderTexture(UnityEngine.XR.XRSettings.eyeTextureWidth * 2, UnityEngine.XR.XRSettings.eyeTextureHeight, 24, InternalTextureFormat);
#else
                RtDownsampler = new RenderTexture(UnityEngine.VR.VRSettings.eyeTextureWidth, UnityEngine.VR.VRSettings.eyeTextureHeight, 24, InternalTextureFormat);
#endif
                RenderTargetIdentifier ssaaCommandBufferTargetId = new RenderTargetIdentifier(RtDownsampler);

                // Fix for singlepass issue in u2018 and newer
#if UNITY_2017_2_OR_NEWER
                RtDownsampler.vrUsage = UnityEngine.XR.XRSettings.eyeTextureDesc.vrUsage;
#endif

                // Command buffer setup
                CbDownsampler.Clear();
                CbDownsampler.name = "SSAA_VR_APPLY";
                CbDownsampler.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);

                // Blits for doownsampling and apply to camera target
                CbDownsampler.Blit(BuiltinRenderTextureType.CameraTarget, ssaaCommandBufferTargetId, MaterialCurrent, 0);
                CbDownsampler.Blit(ssaaCommandBufferTargetId, BuiltinRenderTextureType.CameraTarget, InternalRenderMultiplier > 1 && PostAntiAliasing == PostAntiAliasingMode.FSSAA && InternalRenderMode != RenderMode.AdaptiveResolution ? MaterialFXAA : MaterialDefault, 0);

                // Register cb to camera
                currentCamera.AddCommandBuffer(constCamEventDownsampler, CbDownsampler);
            }
        }
        protected void UpdateDownsamplerCommandBuffer() {
            // fix for VR black screen
            MaterialCurrent.SetOverrideTag("RenderType", "Opaque");
            MaterialCurrent.SetInt("_SrcBlend", (int)BlendMode.One);
            MaterialCurrent.SetInt("_DstBlend", (int)BlendMode.Zero);
            MaterialCurrent.SetInt("_ZWrite", 1);
            MaterialCurrent.renderQueue = -1;

            MaterialDefault.SetOverrideTag("RenderType", "Opaque");
            MaterialDefault.SetInt("_SrcBlend", (int)BlendMode.One);
            MaterialDefault.SetInt("_DstBlend", (int)BlendMode.Zero);
            MaterialDefault.SetInt("_ZWrite", 1);
            MaterialDefault.renderQueue = -1;

#if UNITY_2017_2_OR_NEWER
            MaterialCurrent.SetFloat("_ResizeWidth", UnityEngine.XR.XRSettings.eyeTextureWidth);
            MaterialCurrent.SetFloat("_ResizeHeight", UnityEngine.XR.XRSettings.eyeTextureHeight);
#else
            MaterialCurrent.SetFloat("_ResizeWidth", UnityEngine.VR.VRSettings.eyeTextureWidth);
            MaterialCurrent.SetFloat("_ResizeHeight", UnityEngine.VR.VRSettings.eyeTextureHeight);
#endif
            MaterialCurrent.SetFloat("_Sharpness", DownsamplerSharpness);
            MaterialCurrent.SetFloat("_SampleDistance", DownsamplerDistance);

            MaterialFXAA.SetVector("_QualitySettings", new Vector3(1.0f, 0.063f, 0.0312f));
            MaterialFXAA.SetVector("_ConsoleSettings", new Vector4(0.5f, 2.0f, 0.125f, 0.04f));
            MaterialFXAA.SetFloat("_Intensity", 1);
        }
        protected void ClearDownsamplerCommandBuffer() {
            if (currentCamera == null) return;
            if ((new List<CommandBuffer>(currentCamera.GetCommandBuffers(constCamEventDownsampler))).Find(x => x.name == "SSAA_VR_APPLY") != null) {
                currentCamera.RemoveCommandBuffer(constCamEventDownsampler, CbDownsampler);
            }
        }
        #endregion

        #region MadGoat SSAA Public API Implementation
        #region Instance API
        /// <summary>
        /// Set the multiplier of each screen axis independently. does not use downsampling filter.
        /// </summary>
        public override void SetAsAxisBased(float MultiplierX, float MultiplierY) {
            Debug.LogWarning("SetAsAxisBased is not supported in VR.\nX axis will be used as global multiplier instead.");
            base.SetAsAxisBased(MultiplierX, MultiplierY);
        }
        /// <summary>
        ///  Set the multiplier of each screen axis independently while using the downsampling filter.
        /// </summary>
        public override void SetAsAxisBased(float MultiplierX, float MultiplierY, Filter FilterType, float sharpnessfactor, float sampledist) {
            Debug.LogWarning("SetAsAxisBased is not supported in VR.\nX axis will be used as global multiplier instead.");
            base.SetAsAxisBased(MultiplierX, MultiplierY, FilterType, sharpnessfactor, sampledist);
        }

        /// <summary>
        /// Take a screenshot of resolution Size (x is width, y is height) rendered at a higher resolution given by the multiplier. The screenshot is saved at the given path in PNG format.
        /// </summary>
        public override void TakeScreenshot(string path, Vector2 Size, int multiplier) {
            Debug.LogWarning("Not available in VR mode");
        }
        /// <summary>
        /// Take a screenshot of resolution Size (x is width, y is height) rendered at a higher resolution given by the multiplier. The screenshot is saved at the given path in PNG format. Uses given post process AA method on top of SSAA
        /// </summary>
        public override void TakeScreenshot(string path, Vector2 Size, int multiplier, PostAntiAliasingMode postAntiAliasing) {
            Debug.LogWarning("Not available in VR mode");
        }
        /// <summary>
        /// Take a screenshot of resolution Size (x is width, y is height) rendered at a higher resolution given by the multiplier. The screenshot is saved at the given path in PNG format.
        /// </summary>
        public override void TakeScreenshot(string path, Vector2 size, int multiplier, Filter filterType, float sharpness, float sampleDistance) {
            Debug.LogWarning("Not available in VR mode");
        }
        /// <summary>
        /// Take a screenshot of resolution Size (x is width, y is height) rendered at a higher resolution given by the multiplier. The screenshot is saved at the given path in PNG format. Uses given post process AA method on top of SSAA.
        /// </summary>
        public override void TakeScreenshot(string path, Vector2 size, int multiplier, Filter filterType, float sharpness, float sampleDistance, PostAntiAliasingMode postAntiAliasing) {
            Debug.LogWarning("Not available in VR mode");
        }

        /// <summary>
        /// Sets up the screenshot module to use the PNG image format. This enables transparency in output images.
        /// </summary>
        public override void SetScreenshotModuleToPNG() {
            Debug.LogWarning("Not available in VR mode");
        }
        /// <summary>
        /// Sets up the screenshot module to use the JPG image format. Quality is parameter from 1 to 100 and represents the compression quality of the JPG file. Incorrect quality values will be clamped.
        /// </summary>
        /// <param name="quality"></param>
        public override void SetScreenshotModuleToJPG(int quality) {
            Debug.LogWarning("Not available in VR mode");
        }
#if UNITY_5_6_OR_NEWER
        /// <summary>
        /// Sets up the screenshot module to use the EXR image format. The EXR32 bool parameter dictates whether to use or not 32 bit exr encoding. This method is only available in Unity 5.6 and newer.
        /// </summary>
        /// <param name="EXR32"></param>
        public override void SetScreenshotModuleToEXR(bool EXR32) {
            Debug.LogWarning("Not available in VR mode");
        }
#endif
        #endregion

        #region Deprecated - Will be removed in the future
        /// <summary>
        /// Take a screenshot of resolution Size (x is width, y is height) rendered at a higher resolution given by the multiplier and use the bicubic downsampler. The screenshot is saved at the given path in PNG format. 
        /// </summary>
        [System.Obsolete()]
        public override void TakeScreenshot(string path, Vector2 Size, int multiplier, float sharpness) {
            Debug.LogWarning("Not available in VR mode");
        }
        /// <summary>
        /// Returns a ray from a given screenpoint
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [Obsolete("SSAA ScreenPointToRay has been deprecated. Use Camera's API instead")]
        public override Ray ScreenPointToRay(Vector3 position) {
            return currentCamera.ScreenPointToRay(position);
        }
        /// <summary>
        /// Transforms position from screen space into world space
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [Obsolete("SSAA ScreenToWorldPoint has been deprecated. Use Camera's API instead")]
        public override Vector3 ScreenToWorldPoint(Vector3 position) {
            return currentCamera.ScreenToWorldPoint(position);
        }
        /// <summary>
        /// Transforms postion from screen space into viewport space.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [Obsolete("SSAA ScreenToViewportPoint has been deprecated. Use Camera's API instead")]
        public override Vector3 ScreenToViewportPoint(Vector3 position) {
            return currentCamera.ScreenToViewportPoint(position);
        }
        /// <summary>
        /// Transforms position from world space to screen space
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [Obsolete("SSAA WorldToScreenPoint has been deprecated. Use Camera's API instead")]
        public override Vector3 WorldToScreenPoint(Vector3 position) {
            return currentCamera.WorldToScreenPoint(position);
        }
        /// <summary>
        /// Transforms position from viewport space to screen space
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [Obsolete("SSAA ViewportToScreenPoint has been deprecated. Use Camera's API instead")]
        public override Vector3 ViewportToScreenPoint(Vector3 position) {
            return currentCamera.ViewportToScreenPoint(position);
        }
        #endregion
        #endregion
    }
}