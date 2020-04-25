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
#if UNITY_2018_3_OR_NEWER
    [ExecuteAlways]
#else
    [ExecuteInEditMode]
#endif
    public class MadGoatSSAA : MonoBehaviour {

        #region Fields  
        #region Constants 
        public const CameraEvent constCamEventDownsampler = CameraEvent.AfterImageEffects;
        public const CameraEvent constCamEventGrabAlphaForward = CameraEvent.BeforeImageEffectsOpaque - 1;
        public const CameraEvent constCamEventGrabAlphaDeferred = CameraEvent.BeforeImageEffects - 1;
        public const CameraEvent constCamEventApplyAlpha = CameraEvent.AfterEverything;
        public const CameraEvent constCamEventPostAntiAliasing = CameraEvent.BeforeImageEffects;
        #endregion

        #region Serialized Fields
        // Render settings
        [SerializeField]
        private RenderTextureFormat internalTextureFormat = RenderTextureFormat.ARGBFloat;
        [SerializeField]
        private RenderMode internalRenderMode = RenderMode.SSAA;
        [SerializeField]
        private float internalRenderMultiplier = 1f;
        [SerializeField]
        private float internalRenderMultiplierVertical = 1f;
        [SerializeField]
        private LayerMask internalRenderLayerMask;

        // Supersampling AA
        [SerializeField]
        private SsaaProfile ssaaProfileHalf = new SsaaProfile(.5f, true, Filter.POINT, 0, 0);
        [SerializeField]
        private SsaaProfile ssaaProfileX2 = new SsaaProfile(1.4f, true, Filter.BICUBIC, .8f, .95f);
        [SerializeField]
        private SsaaProfile ssaaProfileX4 = new SsaaProfile(2f, true, Filter.BICUBIC, .8f, .95f);
        [SerializeField]
        private SSAAMode ssaaMode = SSAAMode.SSAA_OFF;

        // Post AA
        [SerializeField]
        private PostAntiAliasingMode postAntiAliasing = PostAntiAliasingMode.Off;

        // Downsampler
        [SerializeField]
        private bool downsamplerEnabled = true;
        [SerializeField]
        private Filter downsamplerFilter = Filter.BILINEAR;
        [SerializeField]
        private float downsamplerSharpness = 0.8f;
        [SerializeField]
        private float downsamplerDistance = 1f;

        // Adaptive Resolution
        [SerializeField]
        private int adaptiveResTargetFps = 60;
        [SerializeField]
        private bool adaptiveResTargetVsync = false;
        [SerializeField]
        private float adaptiveResMinMultiplier = 0.5f;
        [SerializeField]
        private float adaptiveResMaxMultiplier = 1.5f;

        // Screenshots
        [SerializeField]
        private string screenshotPath = "Assets/SuperSampledSceenshots/";
        [SerializeField]
        private string screenshotPrefix = "SSAA";
        [SerializeField]
        private bool screenshotPrefixIsProduct = false;
        [SerializeField]
        private ImageFormat screenshotFormat;
        [SerializeField, Range(0, 100)]
        private int screenshotQuality = 90;
        [SerializeField]
        private bool screenshotExr32 = false;
        [SerializeField]
        private SsaaScreenshotProfile screenshotCaptureSettings = new SsaaScreenshotProfile();

        // Extensions
        [SerializeField]
        private SSAAExtensionPointerEventsSupport extensionIPointerEvents = new SSAAExtensionPointerEventsSupport();
        [SerializeField]
        private SSAAExtensionPostProcessingStack2 extensionPostProcessingStack = new SSAAExtensionPostProcessingStack2();
        [SerializeField]
        private SSAAExtensionCinemachine extensionCinemachine = new SSAAExtensionCinemachine();

        #endregion

        #region Inspector Fields
#if UNITY_EDITOR
        [SerializeField]
#pragma warning disable CS0414
        private bool inspectorShowHelp = true;
#pragma warning restore CS0414
#endif
        [SerializeField]
        private bool cameraFirstTimeSetup;
        #endregion
        #endregion

        #region Properties
        #region Serialized Fields
        // Render settings
        public RenderTextureFormat InternalTextureFormat {
            get {
                return internalTextureFormat;
            }
        }
        public RenderMode InternalRenderMode {
            get { return internalRenderMode; }
            protected set {
                internalRenderMode = value;
            }
        }
        public float InternalRenderMultiplier {
            get { return internalRenderMultiplier; }
            protected set {
                internalRenderMultiplier = value;
            }
        }
        public float InternalRenderMultiplierVertical {
            get { return internalRenderMultiplierVertical; }
            protected set {
                internalRenderMultiplierVertical = value;
            }
        }
        public LayerMask InternalRenderLayerMask {
            get { return internalRenderLayerMask; }
            protected set { internalRenderLayerMask = value; }
        }

        // Supersampling AA
        public SsaaProfile SsaaProfileHalf {
            get { return ssaaProfileHalf; }
            set {
                ssaaProfileHalf = value;
            }
        }
        public SsaaProfile SsaaProfileX2 {
            get { return ssaaProfileX2; }
            set {
                ssaaProfileX2 = value;
            }
        }
        public SsaaProfile SsaaProfileX4 {
            get { return ssaaProfileX4; }
            set {
                ssaaProfileX4 = value;
            }
        }
        public SSAAMode SsaaMode {
            get { return ssaaMode; }
            protected set {
                ssaaMode = value;
            }
        }

        // Post AA
        public PostAntiAliasingMode PostAntiAliasing {
            get { return postAntiAliasing; }
            protected set {
                postAntiAliasing = value;
            }
        }

        // Downsampler
        public bool DownsamplerEnabled {
            get { return downsamplerEnabled; }
            protected set {
                downsamplerEnabled = value;
            }
        }
        public Filter DownsamplerFilter {
            get { return downsamplerFilter; }
            protected set {
                downsamplerFilter = value;
            }
        }
        public float DownsamplerSharpness {
            get { return downsamplerSharpness; }
            protected set {
                downsamplerSharpness = value;
            }
        }
        public float DownsamplerDistance {
            get { return downsamplerDistance; }
            protected set {
                downsamplerDistance = value;
            }
        }

        // Adaptive Resolution
        public int AdaptiveResTargetFps {
            get { return adaptiveResTargetFps; }
            protected set {
                adaptiveResTargetFps = value;
            }
        }
        public bool AdaptiveResTargetVsync {
            get { return adaptiveResTargetVsync; }
            protected set {
                adaptiveResTargetVsync = value;
            }
        }
        public float AdaptiveResMinMultiplier {
            get { return adaptiveResMinMultiplier; }
            protected set {
                adaptiveResMinMultiplier = value;
            }
        }
        public float AdaptiveResMaxMultiplier {
            get { return adaptiveResMaxMultiplier; }
            protected set {
                adaptiveResMaxMultiplier = value;
            }
        }

        // Screenshots
        public string ScreenshotPath {
            get { return screenshotPath; }
            protected set {
                screenshotPath = value;
            }
        }
        public string ScreenshotPrefix {
            get { return screenshotPrefix; }
            protected set {
                screenshotPrefix = value;
            }
        }
        public bool ScreenshotPrefixIsProduct {
            get {
                return screenshotPrefixIsProduct;
            }

            protected set {
                screenshotPrefixIsProduct = value;
            }
        }
        public ImageFormat ScreenshotFormat {
            get { return screenshotFormat; }
            protected set {
                screenshotFormat = value;
            }
        }
        public int ScreenshotQuality {
            get { return screenshotQuality; }
            protected set {
                screenshotQuality = value;
            }
        }
        public bool ScreenshotExr32 {
            get { return screenshotExr32; }
            protected set {
                screenshotExr32 = value;
            }
        }
        public SsaaScreenshotProfile ScreenshotCaptureSettings {
            get { return screenshotCaptureSettings; }
            protected set { screenshotCaptureSettings = value; }
        }

        // extensions
        public SSAAExtensionPointerEventsSupport ExtensionIPointerEvents {
            get { return extensionIPointerEvents; }
            protected set { extensionIPointerEvents = value; }
        }
        public SSAAExtensionPostProcessingStack2 ExtensionPostProcessingStack {
            get { return extensionPostProcessingStack; }
            protected set { extensionPostProcessingStack = value; }
        }
        public SSAAExtensionCinemachine ExtensionCinemachine {
            get { return extensionCinemachine; }
            protected set { extensionCinemachine = value; }
        }
        #endregion

        #region Runtime
        protected Shader ShaderBilinear { get; set; }
        protected Shader ShaderBicubic { get; set; }
        protected Shader ShaderDefault { get; set; }
        protected Shader ShaderFXAA { get; set; }
        protected Shader ShaderAlpha { get; set; }

        protected Material MaterialBilinear { get; set; }
        protected Material MaterialBicubic { get; set; }
        protected Material MaterialDefault { get; set; }
        protected Material MaterialFXAA { get; set; }
        protected Material MaterialAlpha { get; set; }

        public Material MaterialCurrent { get; protected set; }
        protected Material MaterialOld { get; set; }

        public float CameraTargetWidth { get; protected set; }
        public float CameraTargetHeight { get; protected set; }

        protected Camera currentCamera;
        public Camera CurrentCamera {
            get { return currentCamera; }
            protected set { currentCamera = value; }
        }
        protected Camera renderCamera;
        public Camera RenderCamera {
            get { return renderCamera; }
            protected set { renderCamera = value; }
        }

        private bool ScreenshotInternalQueued { get; set; }
        private float ScreenshotTempMultiplier { get; set; }
        private bool ScreenshotTempDownsamplerEnabled { get; set; }
        private Filter ScreenshotTempDownsampler { get; set; }
        private float ScreenshotTempDownsamplerSharpness { get; set; }
        private float ScreenshotTempDownsamplerDistance { get; set; }
        private PostAntiAliasingMode ScreenshotTempPostAntiAliasing { get; set; }

        protected RenderTexture RtDownsampler { get; set; }
        protected RenderTexture RtPostAntiAliasing { get; set; }
        private RenderTexture RtGrabAlpha { get; set; }
        private RenderTexture RtApplyAlpha { get; set; }
        private RenderTexture RtScreenshotTarget { get; set; }
        private RenderTexture RtScreenshotOldTarget { get; set; }

        public CommandBuffer CbPostAntiAliasing { get; set; }
        public CommandBuffer CbGrabAlpha { get; set; }
        public CommandBuffer CbApplyAlpha { get; set; }
        protected CommandBuffer CbDownsampler { get; set; }

        protected RenderingPath CurrentCameraRenderPath { get; set; }
        protected RenderPipelineUtils.PipelineType Pipeline { get; set; }
        protected SsaaFramerateSampler PerfSampler { get; set; }

        private GameObject RenderCamGameObject { get; set; }
        protected bool Initialized { get; set; }

#if SSAA_URP && UNITY_2019_3_OR_NEWER // URP
        protected UniversalAdditionalCameraData UniversalCamDataCurrent { get; set; }
        protected UniversalAdditionalCameraData UniversalCamDataRender { get; set; }
        protected bool UniversalRenderPostProcess = false;
#elif SSAA_URP // LWRP
        protected LWRPAdditionalCameraData LwrpCamDataCurrent { get; set; }
        protected LWRPAdditionalCameraData LwrpCamDataRender { get; set; }
#elif SSAA_HDRP // HDRP
        protected HDAdditionalCameraData HdCamDataCurrent { get; set; }
        protected HDAdditionalCameraData HdCamDataRender { get; set; }
        protected bool HdCamRenderRegistered { get; set; }
#endif
        #endregion
        #endregion

        #region MonoBehaviour Event Methods
        private void Start() {
            OnInitializeProps();
            StartCoroutine(UpdateAdaptiveRes());
            StartCoroutine(OnFinishCameraRender());
        }
        private void Update() {
            // in case 3rd party systems tried to remove the target texture
            if (!renderCamera || !renderCamera.targetTexture) {
                Refresh();
                return;
            }
            // Update performance sampler for adaptive supersampling
            PerfSampler.Update();

            // Check for filter changes
            OnMainFilterChanged(DownsamplerFilter);

            // Check for renderpath changes
            if (CurrentCameraRenderPath != currentCamera.actualRenderingPath && Pipeline == RenderPipelineUtils.PipelineType.BuiltInPipeline) {
                ClearCommandBuffer(renderCamera, CbGrabAlpha, CurrentCameraRenderPath == RenderingPath.Forward ? constCamEventGrabAlphaForward : constCamEventGrabAlphaDeferred);
                CurrentCameraRenderPath = currentCamera.actualRenderingPath;
                Refresh();
            }

            // Check for pipeline changes
            if (Pipeline != RenderPipelineUtils.DetectPipeline()) {
                this.enabled = false;
                Pipeline = RenderPipelineUtils.DetectPipeline();
                this.enabled = true;
            }

#if SSAA_URP && UNITY_2019_3_OR_NEWER
            if(!UniversalCamDataRender || !UniversalCamDataCurrent) {
                Refresh();
                return;
            }
            UniversalCamDataRender.requiresColorOption = CameraOverrideOption.On;
            UniversalCamDataRender.stopNaN = true;
            // copy
            UniversalCamDataRender.volumeLayerMask = UniversalCamDataCurrent.volumeLayerMask;
            UniversalCamDataRender.volumeTrigger = UniversalCamDataCurrent.volumeTrigger;
            UniversalCamDataRender.requiresDepthOption = UniversalCamDataCurrent.requiresDepthOption;
            UniversalCamDataRender.renderShadows = UniversalCamDataCurrent.renderShadows;
            UniversalCamDataRender.renderPostProcessing = true;
            UniversalCamDataCurrent.renderPostProcessing = false;
            UniversalCamDataRender.dithering = UniversalCamDataCurrent.dithering;

#elif SSAA_URP
            if(!LwrpCamDataRender) {
                Refresh();
                return;
            }
            LwrpCamDataRender.requiresColorOption = CameraOverrideOption.On;
            // copy
            LwrpCamDataRender.requiresDepthOption = LwrpCamDataCurrent.requiresDepthOption;
            LwrpCamDataRender.renderShadows = LwrpCamDataCurrent.renderShadows;
#elif SSAA_HDRP
            if (!HdCamDataRender || !HdCamDataCurrent) {
                Refresh();
                return;
            }
            HdCamDataCurrent.fullscreenPassthrough = false;
            var antiAliasing = HdCamDataRender.antialiasing;
            HdCamDataCurrent.CopyTo(HdCamDataRender);
            HdCamDataRender.stopNaNs = true;
            HdCamDataRender.antialiasing = antiAliasing;
#endif
            // handle extensions
            ExtensionIPointerEvents.OnUpdate(this);
            ExtensionPostProcessingStack.OnUpdate(this);
        }
        private void OnEnable() {
            if (!Initialized) return;
            Start();
        }
        private void OnDisable() {
            if (renderCamera == null) return;
            var currentOutput = CurrentCamera.targetTexture;

            if (renderCamera.targetTexture) renderCamera.targetTexture.Release();
            renderCamera.targetTexture = null;
            renderCamera.enabled = false;
            renderCamera.tag = "Untagged";
#if SSAA_HDRP
            if (HdCamDataCurrent != null) HdCamDataCurrent.customRender -= OnHDPipelineRenderImage;
#elif SSAA_URP && UNITY_2019_3_OR_NEWER
            UniversalCamDataCurrent.renderPostProcessing = UniversalRenderPostProcess;
#else
            ClearCommandBuffer(renderCamera, CbGrabAlpha, currentCamera.actualRenderingPath == RenderingPath.Forward ? constCamEventGrabAlphaForward : constCamEventGrabAlphaDeferred);
            ClearCommandBuffer(renderCamera, CbApplyAlpha, constCamEventApplyAlpha);
            ClearCommandBuffer(renderCamera, CbPostAntiAliasing, constCamEventPostAntiAliasing);
            ClearCommandBuffer(currentCamera, CbDownsampler, constCamEventDownsampler);
#endif
#if UNITY_2019_1_OR_NEWER
            // Prerender on SRP
            if (Pipeline == RenderPipelineUtils.PipelineType.UniversalPipeline || Pipeline == RenderPipelineUtils.PipelineType.HDPipeline) {
                RenderPipelineManager.beginCameraRendering -= OnPreRenderSRP;
            }
#endif

            // Reset Camera values 
            var depth = currentCamera.depth;
            currentCamera.CopyFrom(renderCamera);
            currentCamera.depth = depth;
            currentCamera.targetTexture = currentOutput;

            // Deinitialize extensions
            ExtensionIPointerEvents.OnDeinitialize(this);
            ExtensionPostProcessingStack.OnDeinitialize(this);
            ExtensionCinemachine.OnDeinitialize(this);
        }
        private void OnPreRender() {
            // Handle rendering
            OnBeginCameraRender(currentCamera);
        }
#if UNITY_2019_1_OR_NEWER
        private void OnPreRenderSRP(ScriptableRenderContext context, Camera camera) {
#if SSAA_HDRP
            if (camera != renderCamera) return;
#elif SSAA_URP
            if (camera != currentCamera) return;
#endif
            OnBeginCameraRender(currentCamera);
        }
#endif

        #endregion

        #region MadGoat SSAA Event Implementation
        /// <summary>
        /// Event fired when initializing the supersampling component
        /// </summary>
        protected virtual void OnInitialize() {
            // Setup Render Camera 
            var childCams = new List<Camera>(GetComponentsInChildren<Camera>());
            if (childCams.Find(x => x.name == gameObject.name + "_SSAA") == null) {
                RenderCamGameObject = new GameObject(gameObject.name + "_SSAA");
                RenderCamGameObject.transform.SetParent(transform);
                RenderCamGameObject.transform.localPosition = Vector3.zero;
                RenderCamGameObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
                renderCamera = RenderCamGameObject.AddComponent<Camera>();
            }
            // Grab existing camera
            else {
                renderCamera = childCams.Find(x => x.name == gameObject.name + "_SSAA");
                RenderCamGameObject = renderCamera.gameObject;
            }
            var cacheTag = currentCamera.tag;
            currentCamera.tag = "Untagged";
            renderCamera.tag = cacheTag;
            currentCamera.tag = cacheTag;

            // handle culling mask on component add
            if (!cameraFirstTimeSetup) {
                cameraFirstTimeSetup = true;
                InternalRenderLayerMask = currentCamera.cullingMask;
            }

            // Setup current camera
            currentCamera.rect = new Rect(0, 0, 1, 1);
            if (Pipeline == RenderPipelineUtils.PipelineType.BuiltInPipeline) {
                currentCamera.cullingMask = 0;
            }

            // Setup render camera
            renderCamera.enabled = true;
            renderCamera.CopyFrom(currentCamera);

#if SSAA_URP && UNITY_2019_3_OR_NEWER
            UniversalCamDataCurrent = GetComponent<UniversalAdditionalCameraData>();
            UniversalCamDataRender = renderCamera.GetComponent<UniversalAdditionalCameraData>();

            if (!UniversalCamDataCurrent) UniversalCamDataCurrent = gameObject.AddComponent<UniversalAdditionalCameraData>();
            if (!UniversalCamDataRender) UniversalCamDataCurrent = renderCamera.gameObject.AddComponent<UniversalAdditionalCameraData>();

            // Disable MSAA on URP - also fixes for URP Preview cam errors
            ((UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset).msaaSampleCount = 1;

            // Get last known post process status
            UniversalRenderPostProcess = UniversalCamDataCurrent.renderPostProcessing;
#elif SSAA_URP
            LwrpCamDataCurrent = GetComponent<LWRPAdditionalCameraData>();
            LwrpCamDataRender = renderCamera.GetComponent<LWRPAdditionalCameraData>();

            if (!LwrpCamDataCurrent) LwrpCamDataCurrent = gameObject.AddComponent<LWRPAdditionalCameraData>();
            if (!LwrpCamDataRender) LwrpCamDataRender = renderCamera.gameObject.AddComponent<LWRPAdditionalCameraData>();

#elif SSAA_HDRP
            HdCamDataCurrent = GetComponent<HDAdditionalCameraData>();
            HdCamDataRender = renderCamera.GetComponent<HDAdditionalCameraData>();

            if (!HdCamDataCurrent) HdCamDataCurrent = gameObject.AddComponent<HDAdditionalCameraData>();
            if (!HdCamDataRender) HdCamDataRender = renderCamera.gameObject.AddComponent<HDAdditionalCameraData>();

            // Subscribe to HD renderer
            HdCamDataCurrent.customRender -= OnHDPipelineRenderImage; // Fix for black screen on disable after switching pipelines
            HdCamDataCurrent.customRender += OnHDPipelineRenderImage;
#endif

#if UNITY_2019_1_OR_NEWER
            // Prerender on SRP
            if (Pipeline == RenderPipelineUtils.PipelineType.UniversalPipeline || Pipeline == RenderPipelineUtils.PipelineType.HDPipeline) {
                RenderPipelineManager.beginCameraRendering -= OnPreRenderSRP;
                RenderPipelineManager.beginCameraRendering += OnPreRenderSRP;
            }
#endif

            // Setup render target
            if (!RtDownsampler) RtDownsampler = new RenderTexture(1024, 1024, 24, InternalTextureFormat);
            RenderCamera.targetTexture = RtDownsampler;

            // Setup others
            if (!RtGrabAlpha) RtGrabAlpha = new RenderTexture(renderCamera.targetTexture.width, renderCamera.targetTexture.height, 1, InternalTextureFormat);
            if (!RtApplyAlpha) RtApplyAlpha = new RenderTexture(renderCamera.targetTexture.width, renderCamera.targetTexture.height, 1, InternalTextureFormat);
            if (!RtPostAntiAliasing) RtPostAntiAliasing = new RenderTexture(renderCamera.targetTexture.width, renderCamera.targetTexture.height, 1, InternalTextureFormat);

            // setup downsampler pass material
            MaterialCurrent = MaterialDefault;

            // extensions
            ExtensionIPointerEvents.OnInitialize(this);
            ExtensionPostProcessingStack.OnInitialize(this);
            ExtensionCinemachine.OnInitialize(this);

            // Set initialized flag
            Initialized = true;
        }
        /// <summary>
        /// Event fired when initializing the supersampling component. Used for initialization of shared props
        /// </summary>
        protected virtual void OnInitializeProps() {
            if (PerfSampler == null) PerfSampler = new SsaaFramerateSampler();
            CameraTargetWidth = 1024;
            CameraTargetHeight = 1024;

            // Get pipeline  
            var currentPipeline = Pipeline; // Fix for issues with pipeline changing blue screens. Only change pipelines if needed
            Pipeline = RenderPipelineUtils.DetectPipeline();

            // Setup defines
            if (currentPipeline != Pipeline && Pipeline == RenderPipelineUtils.PipelineType.Unsupported) {
                this.enabled = false;
                Debug.LogError("Unsupported Render Pipeline. SSAA is disabled");
                SymbolDefineUtils.RemoveDefine("SSAA_URP");
                SymbolDefineUtils.RemoveDefine("SSAA_HDRP");
                Refresh();
                return;
            }
            else if (currentPipeline != Pipeline && Pipeline == RenderPipelineUtils.PipelineType.HDPipeline) {
                SymbolDefineUtils.AddDefine("SSAA_HDRP");
                SymbolDefineUtils.RemoveDefine("SSAA_URP");
                Refresh();
            }
            else if (currentPipeline != Pipeline && Pipeline == RenderPipelineUtils.PipelineType.UniversalPipeline) {
                SymbolDefineUtils.AddDefine("SSAA_URP");
                SymbolDefineUtils.RemoveDefine("SSAA_HDRP");
                Refresh();
            }
            else if (currentPipeline != Pipeline && Pipeline == RenderPipelineUtils.PipelineType.BuiltInPipeline) {
                SymbolDefineUtils.RemoveDefine("SSAA_URP");
                SymbolDefineUtils.RemoveDefine("SSAA_HDRP");
                Refresh();
            }

            // Setup shader files 
#if UNITY_EDITOR && !SSAA_HDRP
            var shaders = Shader.Find("Hidden/SSAA_Def");
            var shadersPath = AssetDatabase.GetAssetPath(shaders).Replace("STD_SSAA_Def.shader", string.Empty);
            if (File.Exists(shadersPath + "/SRP_SSAA_Bicubic.shader")) {
                if (File.Exists(shadersPath + "/SRP_SSAA_Bicubic.shader.hd")) FileUtil.DeleteFileOrDirectory(shadersPath + "/SRP_SSAA_Bicubic.shader.hd");
                FileUtil.MoveFileOrDirectory(shadersPath + "/SRP_SSAA_Bicubic.shader", shadersPath + "/SRP_SSAA_Bicubic.shader.hd");
                FileUtil.DeleteFileOrDirectory(shadersPath + "/SRP_SSAA_Bicubic.shader.meta");
                AssetDatabase.Refresh();
                Refresh();
            }
            if (File.Exists(shadersPath + "/SRP_SSAA_Bilinear.shader")) {
                if (File.Exists(shadersPath + "/SRP_SSAA_Bilinear.shader.hd")) FileUtil.DeleteFileOrDirectory(shadersPath + "/SRP_SSAA_Bilinear.shader.hd");
                FileUtil.MoveFileOrDirectory(shadersPath + "/SRP_SSAA_Bilinear.shader", shadersPath + "/SRP_SSAA_Bilinear.shader.hd");
                FileUtil.DeleteFileOrDirectory(shadersPath + "/SRP_SSAA_Bilinear.shader.meta");
                AssetDatabase.Refresh();
                Refresh();
            }
            if (File.Exists(shadersPath + "/SRP_SSAA_Def.shader")) {
                if (File.Exists(shadersPath + "/SRP_SSAA_Def.shader.hd")) FileUtil.DeleteFileOrDirectory(shadersPath + "/SRP_SSAA_Def.shader.hd");
                FileUtil.MoveFileOrDirectory(shadersPath + "/SRP_SSAA_Def.shader", shadersPath + "/SRP_SSAA_Def.shader.hd");
                FileUtil.DeleteFileOrDirectory(shadersPath + "/SRP_SSAA_Def.shader.meta");
                AssetDatabase.Refresh();
                Refresh();
            }
#elif UNITY_EDITOR && SSAA_HDRP
            var shaders = Shader.Find("Hidden/SSAA_Def");
            var shadersPath = AssetDatabase.GetAssetPath(shaders).Replace("STD_SSAA_Def.shader", string.Empty);
            if (File.Exists(shadersPath + "/SRP_SSAA_Bicubic.shader.hd")) {
                if (File.Exists(shadersPath + "/SRP_SSAA_Bicubic.shader")) FileUtil.DeleteFileOrDirectory(shadersPath + "/SRP_SSAA_Bicubic.shader");
                FileUtil.MoveFileOrDirectory(shadersPath + "/SRP_SSAA_Bicubic.shader.hd", shadersPath + "/SRP_SSAA_Bicubic.shader");
                FileUtil.DeleteFileOrDirectory(shadersPath + "/SRP_SSAA_Bicubic.shader.hd.meta");
                AssetDatabase.Refresh();
                Refresh();
            }
            if (File.Exists(shadersPath + "/SRP_SSAA_Bilinear.shader.hd")) {
                if (File.Exists(shadersPath + "/SRP_SSAA_Bilinear.shader")) FileUtil.DeleteFileOrDirectory(shadersPath + "/SRP_SSAA_Bilinear.shader");
                FileUtil.MoveFileOrDirectory(shadersPath + "/SRP_SSAA_Bilinear.shader.hd", shadersPath + "/SRP_SSAA_Bilinear.shader");
                FileUtil.DeleteFileOrDirectory(shadersPath + "/SRP_SSAA_Bilinear.shader.hd.meta");
                AssetDatabase.Refresh();
                Refresh();
            }
            if (File.Exists(shadersPath + "/SRP_SSAA_Def.shader.hd")) {
                if (File.Exists(shadersPath + "/SRP_SSAA_Def.shader")) FileUtil.DeleteFileOrDirectory(shadersPath + "/SRP_SSAA_Def.shader");
                FileUtil.MoveFileOrDirectory(shadersPath + "/SRP_SSAA_Def.shader.hd", shadersPath + "/SRP_SSAA_Def.shader");
                FileUtil.DeleteFileOrDirectory(shadersPath + "/SRP_SSAA_Def.shader.hd.meta");
                AssetDatabase.Refresh();
                Refresh();
            }
#endif
            // command buffers are only needed on Built-in SRP
#if !SSAA_HDRP && !SSAA_URP
            if (CbPostAntiAliasing == null) CbPostAntiAliasing = new CommandBuffer();
            if (CbGrabAlpha == null) CbGrabAlpha = new CommandBuffer();
            if (CbApplyAlpha == null) CbApplyAlpha = new CommandBuffer();
            if (CbDownsampler == null) CbDownsampler = new CommandBuffer();
#endif

            // initialize downsampler shaders. Set even if null to avoid issues when changing pipelines
#if SSAA_HDRP   // IF HDRP
            ShaderBilinear = Shader.Find("Hidden/SSAA_Bilinear_HDRP");
            ShaderBicubic = Shader.Find("Hidden/SSAA_Bicubic_HDRP");
            ShaderDefault = Shader.Find("Hidden/SSAA_Def_HDRP");
#else           // IF BUILT IN PIPELINE
            ShaderBilinear = Shader.Find("Hidden/SSAA_Bilinear");
            ShaderBicubic = Shader.Find("Hidden/SSAA_Bicubic");
            ShaderDefault = Shader.Find("Hidden/SSAA_Def");
#endif

            // initialize misc shaders
            if (ShaderFXAA == null) ShaderFXAA = Shader.Find("Hidden/SSAA/FSS");
            if (ShaderAlpha == null) ShaderAlpha = Shader.Find("Hidden/SSAA_Alpha");

            // initialize downsampler materials
            MaterialBilinear = new Material(ShaderBilinear);
            MaterialBicubic = new Material(ShaderBicubic);
            MaterialDefault = new Material(ShaderDefault);

            // initialize misc materials
            if (MaterialFXAA == null) MaterialFXAA = new Material(ShaderFXAA);
            if (MaterialAlpha == null) MaterialAlpha = new Material(ShaderAlpha);

            // Current Camera
            if (currentCamera == null) currentCamera = GetComponent<Camera>();

            // Initialize SSAA
            OnInitialize();
        }
        /// <summary>
        /// Event fired when the render camera begins to render a frame
        /// </summary>
        /// <param name="cam"></param>
        public virtual void OnBeginCameraRender(Camera cam) {
            if (cam != currentCamera || !enabled || RenderCamera == null || renderCamera.targetTexture == null) return;

            // store values for refresh
            var targetTexture = renderCamera.targetTexture;
            // update our render camera to match current camera
            currentCamera.cullingMask = 0; //1 << 5; // ui?
            renderCamera.CopyFrom(currentCamera, targetTexture);
            renderCamera.depth = currentCamera.depth + 0.1f;
            renderCamera.enabled = currentCamera.enabled;
            renderCamera.cullingMask = InternalRenderLayerMask;
            renderCamera.clearFlags = currentCamera.clearFlags;
            renderCamera.targetTexture.filterMode = (DownsamplerFilter == Filter.POINT && DownsamplerEnabled) ? FilterMode.Point : FilterMode.Trilinear;
            ExtensionCinemachine.OnUpdate(this);

            if (ScreenshotCaptureSettings.takeScreenshot) {
                SetupScreenshotRender();
            }

            CameraTargetWidth = currentCamera.targetTexture != null ? currentCamera.targetTexture.width : Screen.width;
            CameraTargetHeight = currentCamera.targetTexture != null ? currentCamera.targetTexture.height : Screen.height;

            // Setup the aspect ratio
            currentCamera.aspect = (CameraTargetWidth * currentCamera.rect.width) / (CameraTargetHeight * currentCamera.rect.height);
            renderCamera.aspect = (CameraTargetWidth * renderCamera.rect.width) / (CameraTargetHeight * renderCamera.rect.height);

            if ((int)(CameraTargetWidth * InternalRenderMultiplier) != renderCamera.targetTexture.width
                || (int)(CameraTargetHeight * (InternalRenderMode == RenderMode.PerAxisScale ? InternalRenderMultiplierVertical : InternalRenderMultiplier)) != renderCamera.targetTexture.height) {

                // ssaa multiplier has changed. refresh target texture and commandbuffers 
                if (RenderTexture.active == renderCamera.targetTexture) RenderTexture.active = null;
                UpdateRtSizes();
#if !SSAA_HDRP && !SSAA_URP
                renderCamera.Render();
#endif
            }
            if (Pipeline == RenderPipelineUtils.PipelineType.BuiltInPipeline) {
                // update alpha and post aa command buffers on built in pipeline 
                UpdateDownsamplerCommandBuffer(currentCamera);
                UpdateAlphaCommandBuffer(renderCamera);
            }


            // With raytracing enabled we need the temporal filter to clean up noise
#if ENABLE_RAYTRACING && SSAA_HDRP
            HdCamDataRender.antialiasing = HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing;
#else
            if (InternalRenderMultiplier > 1 && PostAntiAliasing == PostAntiAliasingMode.FSSAA && InternalRenderMode != RenderMode.AdaptiveResolution) {
                // for built-in - set camera to FSSAA
                if (Pipeline == RenderPipelineUtils.PipelineType.BuiltInPipeline) UpdatePostAntiAliasingCommandBuffer(renderCamera);

                // for HDRP - set camera to FSSAA
#if SSAA_HDRP
                HdCamDataRender.antialiasing = HDAdditionalCameraData.AntialiasingMode.FastApproximateAntialiasing;
#endif
                // for URP - set camera to FSSAA. Only 2019.3 has built in FSSAA support.
#if SSAA_URP && UNITY_2019_3_OR_NEWER
                UniversalCamDataRender.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
                UniversalCamDataRender.renderPostProcessing = true;
#endif
            }

            // for HDRP - set camera to TSSAA. Only HDRP has built in TSSAA support
#if SSAA_HDRP
            else if (InternalRenderMultiplier > 1 && PostAntiAliasing == PostAntiAliasingMode.TSSAA && InternalRenderMode != RenderMode.AdaptiveResolution) {
                HdCamDataRender.antialiasing = HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing;
            }
#endif
            else {
                // for built-in - set camera to off
                if (Pipeline == RenderPipelineUtils.PipelineType.BuiltInPipeline) ClearCommandBuffer(renderCamera, CbPostAntiAliasing, constCamEventPostAntiAliasing);
                // for HDRP - set camera to off
#if SSAA_HDRP
                HdCamDataRender.antialiasing = HDAdditionalCameraData.AntialiasingMode.None;
#endif
                // for URP - set camera to OFF. Only 2019.3 has built in FSSAA support.
#if SSAA_URP && UNITY_2019_3_OR_NEWER
                UniversalCamDataRender.antialiasing = AntialiasingMode.None;
#endif
            }
#endif
        }
        /// <summary>
        /// Event fired when the render camera finish to render a frame
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator OnFinishCameraRender() {
            while (enabled) {
                yield return new WaitForEndOfFrame();
                if (ScreenshotCaptureSettings.takeScreenshot) {
                    yield return new WaitForEndOfFrame();
#if SSAA_URP
                    yield return new WaitForEndOfFrame();
#elif !SSAA_HDRP
                    yield return new WaitForEndOfFrame();
#endif
                    HandleScreenshot();
                }
            }
        }
        /// <summary>
        /// Event fired when SSAA filter is changed
        /// </summary>
        /// <param name="Type"></param>
        public void OnMainFilterChanged(Filter Type) {
            MaterialOld = MaterialCurrent;

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
            if (MaterialCurrent != MaterialOld) {
                MaterialOld = MaterialCurrent;
                // Refresh the command buffer on material change
                ClearCommandBuffer(currentCamera, CbDownsampler, constCamEventDownsampler);
            }
        }
#if SSAA_HDRP
        protected virtual void OnHDPipelineRenderImage(ScriptableRenderContext context, HDCamera camera) {
            if (!enabled) return;

            var cmd = CommandBufferPool.Get("SSAA_HDRP_DOWNSAMPLER");
            cmd.SetViewport(new Rect(0, 0, CameraTargetWidth, CameraTargetHeight));

            CoreUtils.SetRenderTarget(cmd, BuiltinRenderTextureType.CameraTarget);
            CoreUtils.ClearRenderTarget(cmd, ClearFlag.Depth, Color.clear);

            MaterialCurrent.SetTexture("_MainTex", renderCamera.targetTexture);
            var rt = currentCamera.targetTexture;
            var rtid = rt != null ?
                new RenderTargetIdentifier(rt) :
                new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget);

            // fix black screen
            MaterialCurrent.SetOverrideTag("RenderType", "Opaque");
            MaterialCurrent.SetInt("_SrcBlend", (int)BlendMode.One);
            MaterialCurrent.SetInt("_DstBlend", (int)BlendMode.Zero);
            MaterialCurrent.SetInt("_ZWrite", 1);
            MaterialCurrent.renderQueue = -1;

            // update values
            MaterialCurrent.SetFloat("_ResizeWidth", CameraTargetWidth);
            MaterialCurrent.SetFloat("_ResizeHeight", CameraTargetHeight);
            MaterialCurrent.SetFloat("_Sharpness", DownsamplerSharpness);
            MaterialCurrent.SetFloat("_SampleDistance", DownsamplerDistance);

            CoreUtils.DrawFullScreen(cmd, MaterialCurrent, rtid);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
#endif
        #endregion

        #region MadGoat SSAA Standard Pipeline Core Implementation
        /// <summary>
        /// Setup and update the FSSAA command buffer 
        /// </summary>
        protected void UpdatePostAntiAliasingCommandBuffer(Camera hookCamera) {
            if (Pipeline != RenderPipelineUtils.PipelineType.BuiltInPipeline) return;

            MaterialFXAA.SetVector("_QualitySettings", new Vector3(1.0f, 0.063f, 0.0312f));
            MaterialFXAA.SetVector("_ConsoleSettings", new Vector4(0.5f, 2.0f, 0.125f, 0.04f));
            MaterialFXAA.SetFloat("_Intensity", 1);

            // Setup command buffer only if needed
            if ((new List<CommandBuffer>(hookCamera.GetCommandBuffers(constCamEventPostAntiAliasing))).Find(x => x.name == "SSAA_FSS") == null) {
                // Clear buffer
                CbPostAntiAliasing.Clear();

                // Setup render targets
                CbPostAntiAliasing.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                var idFxaaFlip = new RenderTargetIdentifier(RtPostAntiAliasing);

                // Setup fssaa commands
                CbPostAntiAliasing.Blit(BuiltinRenderTextureType.CameraTarget, idFxaaFlip);
                CbPostAntiAliasing.Blit(idFxaaFlip, BuiltinRenderTextureType.CameraTarget, MaterialFXAA, 0);

                // Hook the command buffer into the camera   
                CbPostAntiAliasing.name = "SSAA_FSS";
                hookCamera.AddCommandBuffer(constCamEventPostAntiAliasing, CbPostAntiAliasing);
            }
        }
        /// <summary>
        /// Setup and update the Alpha command buffers 
        /// </summary>
        protected void UpdateAlphaCommandBuffer(Camera hookCamera) {
            if (Pipeline != RenderPipelineUtils.PipelineType.BuiltInPipeline) return;

            // Setup command buffer only if needed
            if ((new List<CommandBuffer>(hookCamera.GetCommandBuffers(constCamEventApplyAlpha))).Find(x => x.name == "SSAA_Apply_Alpha") == null) {
                // Clear buffers
                CbGrabAlpha.Clear();
                CbApplyAlpha.Clear();

                // Setup render targets
                CbGrabAlpha.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                CbApplyAlpha.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                var idGrabAlphaRt = new RenderTargetIdentifier(RtGrabAlpha);
                var idPasteAlphaRT = new RenderTargetIdentifier(RtApplyAlpha);

                // Setup alpha grab commands
                CbGrabAlpha.Blit(BuiltinRenderTextureType.CameraTarget, idGrabAlphaRt, MaterialAlpha, 0);

                // Setup alpha apply commands
                CbApplyAlpha.SetGlobalTexture("_MainTexA", idGrabAlphaRt);
                CbApplyAlpha.Blit(BuiltinRenderTextureType.CameraTarget, idPasteAlphaRT);
                CbApplyAlpha.Blit(idPasteAlphaRT, BuiltinRenderTextureType.CameraTarget, MaterialAlpha, 1);

                // Hook the command buffers into the camera                
                CbGrabAlpha.name = "SSAA_Grab_Alpha";
                CbApplyAlpha.name = "SSAA_Apply_Alpha";
                hookCamera.AddCommandBuffer(currentCamera.actualRenderingPath == RenderingPath.Forward ? constCamEventGrabAlphaForward : constCamEventGrabAlphaDeferred, CbGrabAlpha);
                hookCamera.AddCommandBuffer(constCamEventApplyAlpha, CbApplyAlpha);
            }
        }
        /// <summary>
        /// Setup and update the downsampler command buffer
        /// </summary>
        /// <param name="hookCamera"></param>
        protected void UpdateDownsamplerCommandBuffer(Camera hookCamera) {
            if (Pipeline != RenderPipelineUtils.PipelineType.BuiltInPipeline) return;

            // update values
            MaterialCurrent.SetFloat("_ResizeWidth", CameraTargetWidth);
            MaterialCurrent.SetFloat("_ResizeHeight", CameraTargetHeight);
            MaterialCurrent.SetFloat("_Sharpness", DownsamplerSharpness);
            MaterialCurrent.SetFloat("_SampleDistance", DownsamplerDistance);

            // --- Fix for deferred stacking
            // Layers with skyboxes or solid color clearflags should alwasy render opaque to avoid
            // alpha testing issues on deferred
            if (hookCamera.clearFlags == CameraClearFlags.Color || hookCamera.clearFlags == CameraClearFlags.Skybox) {
                MaterialCurrent.SetOverrideTag("RenderType", "Opaque");
                MaterialCurrent.SetInt("_SrcBlend", (int)BlendMode.One);
                MaterialCurrent.SetInt("_DstBlend", (int)BlendMode.Zero);
                MaterialCurrent.SetInt("_ZWrite", 1);
                MaterialCurrent.renderQueue = -1;
            }
            else {
                MaterialCurrent.SetOverrideTag("RenderType", "Transparent");
                MaterialCurrent.SetInt("_SrcBlend", (int)BlendMode.One);
                MaterialCurrent.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                MaterialCurrent.SetInt("_ZWrite", 0);
                MaterialCurrent.renderQueue = 3000;
            }

            // Setup command buffer only if needed
            if ((new List<CommandBuffer>(currentCamera.GetCommandBuffers(constCamEventDownsampler))).Find(x => x.name == "SSAA_Downsampler") == null) {
                // Clear buffers
                CbDownsampler.Clear();

                // Setup render targets
                CbDownsampler.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                var rtDownsamplerId = new RenderTargetIdentifier(RtDownsampler);

                // Setup downsampler commands 
                CbDownsampler.Blit(rtDownsamplerId, BuiltinRenderTextureType.CameraTarget, MaterialCurrent, 0);

                // Hook the command buffer into the camera   
                CbDownsampler.name = "SSAA_Downsampler";
                hookCamera.AddCommandBuffer(constCamEventDownsampler, CbDownsampler);
            }
        }
        /// <summary>
        /// Clears a given comman buffer from a specific camera on a specific camera event
        /// </summary>
        /// <param name="hookCamera"></param>
        /// <param name="commandBuffer"></param>
        /// <param name="cameraEvent"></param>
        protected void ClearCommandBuffer(Camera hookCamera, CommandBuffer commandBuffer, CameraEvent cameraEvent) {
            if (Pipeline != RenderPipelineUtils.PipelineType.BuiltInPipeline) return;

            // clear command
            commandBuffer.Clear();
            hookCamera.RemoveCommandBuffer(cameraEvent, commandBuffer);
        }
        #endregion

        #region MadGoat SSAA Private API Implementation
        /// <summary>
        /// Updates the multiplier based on a framerate threshold
        /// </summary>
        /// <returns></returns>
        protected IEnumerator UpdateAdaptiveRes() {
            yield return new WaitForSeconds(UnityEngine.Random.Range(2, 5));
            if (InternalRenderMode == RenderMode.AdaptiveResolution) {
                var compFramerate = AdaptiveResTargetVsync ? Screen.currentResolution.refreshRate : AdaptiveResTargetFps;
                if (PerfSampler.CurrentFps < compFramerate - 5) {
                    InternalRenderMultiplier = Mathf.Clamp(InternalRenderMultiplier - 0.1f, AdaptiveResMinMultiplier, AdaptiveResMaxMultiplier);
                }
                else if (PerfSampler.CurrentFps > compFramerate + 10) {
                    InternalRenderMultiplier = Mathf.Clamp(InternalRenderMultiplier + 0.1f, AdaptiveResMinMultiplier, AdaptiveResMaxMultiplier);
                }
            }

            if (enabled) StartCoroutine(UpdateAdaptiveRes());
        }
        /// <summary>
        /// Setup for SSAA renderer
        /// </summary>
        private void UpdateRtSizes() {
            try {
#if SSAA_HDRP || SSAA_URP
                // fix for flickering on resolution change
                var rtBuff = RenderTexture.GetTemporary(renderCamera.targetTexture.descriptor);
                Graphics.Blit(renderCamera.targetTexture, rtBuff);
#endif
                renderCamera.targetTexture.Release();
                RtDownsampler.antiAliasing = 1;
                renderCamera.targetTexture.width = (int)(CameraTargetWidth * InternalRenderMultiplier);
                renderCamera.targetTexture.height = (int)(CameraTargetHeight * (InternalRenderMode == RenderMode.PerAxisScale ? InternalRenderMultiplierVertical : InternalRenderMultiplier));
                renderCamera.targetTexture.Create();

#if SSAA_HDRP || SSAA_URP
                // fix for flickering on resolution change
                Graphics.Blit(rtBuff, renderCamera.targetTexture);
                RenderTexture.ReleaseTemporary(rtBuff);
#endif

                RtGrabAlpha.Release();
                RtGrabAlpha.width = renderCamera.targetTexture.width;
                RtGrabAlpha.height = renderCamera.targetTexture.height;
                RtGrabAlpha.Create();

                RtApplyAlpha.Release();
                RtApplyAlpha.width = renderCamera.targetTexture.width;
                RtApplyAlpha.height = renderCamera.targetTexture.height;
                RtApplyAlpha.Create();

                RtPostAntiAliasing.Release();
                RtPostAntiAliasing.width = renderCamera.targetTexture.width;
                RtPostAntiAliasing.height = renderCamera.targetTexture.height;
                RtPostAntiAliasing.Create();
            }
            catch (Exception ex) {
                Debug.LogError("Something went wrong. SSAA has been set to off");
                Debug.LogError(ex);
                SetAsSSAA(SSAAMode.SSAA_OFF);
            }
        }
        /// <summary>
        /// Setup for ScreenShot Render
        /// </summary>
        /// <param name="mul"></param>
        /// <param name="compatibilityMode"></param>
        private void SetupScreenshotRender() {
            try {
                if (ScreenshotInternalQueued) return;
                if (RtScreenshotTarget == null) {
                    RtScreenshotTarget = new RenderTexture((int)ScreenshotCaptureSettings.outputResolution.x,
                      (int)ScreenshotCaptureSettings.outputResolution.y, 24, internalTextureFormat);
                }
                else {
                    RtScreenshotTarget.Release();
                    RtScreenshotTarget.width = (int)ScreenshotCaptureSettings.outputResolution.x;
                    RtScreenshotTarget.height = (int)ScreenshotCaptureSettings.outputResolution.y;
                }
                RtScreenshotTarget.Create();

                // store defaults
                RtScreenshotOldTarget = currentCamera.targetTexture;
                ScreenshotTempDownsamplerEnabled = downsamplerEnabled;
                ScreenshotTempDownsampler = downsamplerFilter;
                ScreenshotTempDownsamplerSharpness = downsamplerSharpness;
                ScreenshotTempDownsamplerDistance = downsamplerDistance;
                ScreenshotTempMultiplier = internalRenderMultiplier;
                ScreenshotTempPostAntiAliasing = postAntiAliasing;

                // setup overrides
                currentCamera.targetTexture = RtScreenshotTarget;
                downsamplerEnabled = screenshotCaptureSettings.downsamplerEnabled;
                downsamplerFilter = screenshotCaptureSettings.downsamplerFilter;
                downsamplerSharpness = screenshotCaptureSettings.downsamplerSharpness;
                downsamplerDistance = screenshotCaptureSettings.downsamplerDistance;
                internalRenderMultiplier = screenshotCaptureSettings.screenshotMultiplier;
                postAntiAliasing = screenshotCaptureSettings.postAntiAliasing;

                ScreenshotInternalQueued = true;
            }
            catch (Exception ex) { Debug.LogError(ex.ToString()); }
        }
        /// <summary>
        /// Handle grabbing current frame for screenshot
        /// </summary>
        private void HandleScreenshot() {
            if (currentCamera.targetTexture == null) return;

            // buffer to store texture
            Material screenshotMat = new Material(Shader.Find("Hidden/SSAA_Bilinear"));
            RenderTexture buff = new RenderTexture(currentCamera.targetTexture.width, currentCamera.targetTexture.height, 24, RenderTextureFormat.ARGB32);

            // enable srgb conversion for blit - fixes the color issue
            bool sRGBWrite = GL.sRGBWrite;
            GL.sRGBWrite = true;

            // setup shader
            if (ScreenshotCaptureSettings.downsamplerEnabled) {
                screenshotMat.SetFloat("_ResizeWidth", (int)ScreenshotCaptureSettings.outputResolution.x);
                screenshotMat.SetFloat("_ResizeHeight", (int)ScreenshotCaptureSettings.outputResolution.y);
                screenshotMat.SetFloat("_Sharpness", 0.85f);
                screenshotMat.SetFloat("_SampleDistance", 1f);
                Graphics.Blit(currentCamera.targetTexture, buff);
            }
            else // or blit as it is
            {
                Graphics.Blit(currentCamera.targetTexture, buff);
            }
            RenderTexture.active = buff;

            // Copy from active texture to buffer
            Texture2D screenshotBuffer = new Texture2D(RenderTexture.active.width, RenderTexture.active.height, TextureFormat.RGBA32, false);
            screenshotBuffer.ReadPixels(new Rect(0, 0, RenderTexture.active.width, RenderTexture.active.height), 0, 0);

            // Create path if not available and write the screenshot to disk
            (new FileInfo(screenshotPath)).Directory.Create();

            var name = (screenshotPrefixIsProduct ? Application.productName : screenshotPrefix) + "_" +
                DateTime.Now.ToString("yyyyMMdd_HHmmssff") + "_" +
                ScreenshotCaptureSettings.outputResolution.y.ToString() + "p";

            if (screenshotFormat == ImageFormat.PNG)
                File.WriteAllBytes(screenshotPath + name + ".png", screenshotBuffer.EncodeToPNG());
            else if (screenshotFormat == ImageFormat.JPG)
                File.WriteAllBytes(screenshotPath + name + ".jpg", screenshotBuffer.EncodeToJPG(screenshotQuality));
#if UNITY_5_6_OR_NEWER
            else
                File.WriteAllBytes(screenshotPath + name + ".exr", screenshotBuffer.EncodeToEXR(screenshotExr32 ? Texture2D.EXRFlags.OutputAsFloat : Texture2D.EXRFlags.None));
#endif

            // Clean stuff
            RenderTexture.active = null;
            buff.Release();

            // restore the sRGBWrite to older state so it doesn't interfere with user's setting
            GL.sRGBWrite = sRGBWrite;

            DestroyImmediate(screenshotBuffer);
            ScreenshotCaptureSettings.takeScreenshot = false;
            ScreenshotInternalQueued = false;

            // revert overrides
            currentCamera.targetTexture = RtScreenshotOldTarget;
            downsamplerEnabled = ScreenshotTempDownsamplerEnabled;
            downsamplerFilter = ScreenshotTempDownsampler;
            downsamplerSharpness = ScreenshotTempDownsamplerSharpness;
            internalRenderMultiplier = ScreenshotTempMultiplier;
#if SSAA_URP && UNITY_2019_3_OR_NEWER
            UniversalCamDataCurrent.cameraOutput = CameraOutput.Camera;
#endif
            RtScreenshotTarget.Release();
        }
        #endregion

        #region MadGoat SSAA Public API Implementation
        #region Instance API
        /// <summary>
        /// Reinitialize the whole SSAA system.
        /// </summary>
        public void Refresh() {
            this.enabled = false;
            this.enabled = true;

            var vrComponent = GetComponent<MadGoatSSAA_VR>();
            if (vrComponent && vrComponent != this) vrComponent.Refresh();
        }

        /// <summary>
        /// Set rendering mode to given SSAA mode
        /// </summary>
        public void SetAsSSAA(SSAAMode mode) {
            InternalRenderMode = RenderMode.SSAA;
            SsaaMode = mode;
            switch (mode) {
                case SSAAMode.SSAA_OFF:
                    InternalRenderMultiplier = 1f;
                    DownsamplerEnabled = false;
                    break;
                case SSAAMode.SSAA_HALF:
                    InternalRenderMultiplier = SsaaProfileHalf.multiplier;
                    DownsamplerEnabled = SsaaProfileHalf.useFilter;
                    DownsamplerSharpness = SsaaProfileHalf.sharpness;
                    DownsamplerFilter = SsaaProfileHalf.filterType;
                    DownsamplerDistance = SsaaProfileHalf.sampleDistance;
                    break;
                case SSAAMode.SSAA_X2:
                    InternalRenderMultiplier = SsaaProfileX2.multiplier;
                    DownsamplerEnabled = SsaaProfileX2.useFilter;
                    DownsamplerSharpness = SsaaProfileX2.sharpness;
                    DownsamplerFilter = SsaaProfileX2.filterType;
                    DownsamplerDistance = SsaaProfileX2.sampleDistance;
                    break;
                case SSAAMode.SSAA_X4:
                    InternalRenderMultiplier = SsaaProfileX4.multiplier;
                    DownsamplerEnabled = SsaaProfileX4.useFilter;
                    DownsamplerSharpness = SsaaProfileX4.sharpness;
                    DownsamplerFilter = SsaaProfileX4.filterType;
                    DownsamplerDistance = SsaaProfileX4.sampleDistance;
                    break;
            }

            var vrComponent = GetComponent<MadGoatSSAA_VR>();
            if (vrComponent && vrComponent != this) vrComponent.SetAsSSAA(mode);
        }

        /// <summary>
        /// Set a custom resolution multiplier
        /// </summary>
        public void SetAsScale(float multiplier) {
            // check for invalid values
            if (multiplier < 0.1f) multiplier = 0.1f;

            InternalRenderMode = RenderMode.Custom;
            InternalRenderMultiplier = multiplier;

            SetDownsamplingSettings(false);

            var vrComponent = GetComponent<MadGoatSSAA_VR>();
            if (vrComponent && vrComponent != this) vrComponent.SetAsScale(multiplier);
        }
        /// <summary>
        /// Set a custom resolution multiplier, and use custom downsampler settings
        /// </summary>
        public void SetAsScale(float multiplier, Filter filterType, float sharpness, float sampleDistance) {
            // check for invalid values
            if (multiplier < 0.1f) multiplier = 0.1f;

            InternalRenderMode = RenderMode.Custom;
            InternalRenderMultiplier = multiplier;

            SetDownsamplingSettings(filterType, sharpness, sampleDistance);

            var vrComponent = GetComponent<MadGoatSSAA_VR>();
            if (vrComponent && vrComponent != this) vrComponent.SetAsScale(multiplier, filterType, sharpness, sampleDistance);
        }
        /// <summary>
        /// Set the resolution scale to a given percent
        /// </summary>
        public void SetAsScale(int percent) {
            // check for invalid values
            percent = Mathf.Clamp(percent, 50, 200);

            InternalRenderMode = RenderMode.ResolutionScale;
            InternalRenderMultiplier = percent / 100f;

            SetDownsamplingSettings(false);

            var vrComponent = GetComponent<MadGoatSSAA_VR>();
            if (vrComponent && vrComponent != this) vrComponent.SetAsScale(percent);
        }
        /// <summary>
        /// Set the resolution scale to a given percent, and use custom downsampler settings
        /// </summary>
        public void SetAsScale(int percent, Filter filterType, float sharpness, float sampleDistance) {
            // check for invalid values
            percent = Mathf.Clamp(percent, 50, 200);

            InternalRenderMode = RenderMode.ResolutionScale;
            InternalRenderMultiplier = percent / 100f;

            SetDownsamplingSettings(filterType, sharpness, sampleDistance);

            var vrComponent = GetComponent<MadGoatSSAA_VR>();
            if (vrComponent && vrComponent != this) vrComponent.SetAsScale(percent, filterType, sharpness, sampleDistance);
        }

        /// <summary>
        /// Set the operation mode as adaptive with screen refresh rate as target frame rate
        /// </summary>
        /// <param name="minMultiplier"></param>
        /// <param name="maxMultiplier"></param>
        public void SetAsAdaptive(float minMultiplier, float maxMultiplier) {
            // check for invalid values
            if (minMultiplier < 0.1f) minMultiplier = 0.1f;
            if (maxMultiplier < minMultiplier) maxMultiplier = minMultiplier + 0.1f;

            this.AdaptiveResMinMultiplier = minMultiplier;
            this.AdaptiveResMaxMultiplier = maxMultiplier;
            AdaptiveResTargetVsync = true;
            SetDownsamplingSettings(false);

            var vrComponent = GetComponent<MadGoatSSAA_VR>();
            if (vrComponent && vrComponent != this) vrComponent.SetAsAdaptive(minMultiplier, maxMultiplier);
        }
        /// <summary>
        /// Set the operation mode as adaptive with target frame rate
        /// </summary>
        /// <param name="minMultiplier"></param>
        /// <param name="maxMultiplier"></param>
        /// <param name="targetFramerate"></param>
        public void SetAsAdaptive(float minMultiplier, float maxMultiplier, int targetFramerate) {
            // check for invalid values
            if (minMultiplier < 0.1f) minMultiplier = 0.1f;
            if (maxMultiplier < minMultiplier) maxMultiplier = minMultiplier + 0.1f;

            this.AdaptiveResMinMultiplier = minMultiplier;
            this.AdaptiveResMaxMultiplier = maxMultiplier;
            this.AdaptiveResTargetFps = targetFramerate;
            AdaptiveResTargetVsync = false;
            SetDownsamplingSettings(false);

            var vrComponent = GetComponent<MadGoatSSAA_VR>();
            if (vrComponent && vrComponent != this) vrComponent.SetAsAdaptive(minMultiplier, maxMultiplier, targetFramerate);
        }
        /// <summary>
        /// Set the operation mode as adaptive with target frame rate and use downsampling filter.
        /// </summary>
        /// <param name="minMultiplier"></param>
        /// <param name="maxMultiplier"></param>
        /// <param name="targetFramerate"></param>
        /// <param name="filterType"></param>
        /// <param name="sharpness"></param>
        /// <param name="sampleDistance"></param>
        public void SetAsAdaptive(float minMultiplier, float maxMultiplier, int targetFramerate, Filter filterType, float sharpness, float sampleDistance) {
            // check for invalid values
            if (minMultiplier < 0.1f) minMultiplier = 0.1f;
            if (maxMultiplier < minMultiplier) maxMultiplier = minMultiplier + 0.1f;

            this.AdaptiveResMinMultiplier = minMultiplier;
            this.AdaptiveResMaxMultiplier = maxMultiplier;
            this.AdaptiveResTargetFps = targetFramerate;
            AdaptiveResTargetVsync = false;

            SetDownsamplingSettings(filterType, sharpness, sampleDistance);

            var vrComponent = GetComponent<MadGoatSSAA_VR>();
            if (vrComponent && vrComponent != this) vrComponent.SetAsAdaptive(minMultiplier, maxMultiplier, targetFramerate, filterType, sharpness, sampleDistance);
        }
        /// <summary>
        /// Set the operation mode as adaptive with screen refresh rate as target frame rate and use downsampling filter.
        /// </summary>
        /// <param name="minMultiplier"></param>
        /// <param name="maxMultiplier"></param>
        /// <param name="filterType"></param>
        /// <param name="sharpness"></param>
        /// <param name="sampleDistance"></param>
        public void SetAsAdaptive(float minMultiplier, float maxMultiplier, Filter filterType, float sharpness, float sampleDistance) {
            // check for invalid values
            if (minMultiplier < 0.1f) minMultiplier = 0.1f;
            if (maxMultiplier < minMultiplier) maxMultiplier = minMultiplier + 0.1f;

            this.AdaptiveResMinMultiplier = minMultiplier;
            this.AdaptiveResMaxMultiplier = maxMultiplier;
            AdaptiveResTargetVsync = true;

            SetDownsamplingSettings(filterType, sharpness, sampleDistance);

            var vrComponent = GetComponent<MadGoatSSAA_VR>();
            if (vrComponent && vrComponent != this) vrComponent.SetAsAdaptive(minMultiplier, maxMultiplier, filterType, sharpness, sampleDistance);
        }

        /// <summary>
        /// Set the multiplier of each screen axis independently. does not use downsampling filter.
        /// </summary>
        /// <param name="multiplierX"></param>
        /// <param name="multiplierY"></param>
        public virtual void SetAsAxisBased(float multiplierX, float multiplierY) {
            // check for invalid values
            if (multiplierX < 0.1f) multiplierX = 0.1f;
            if (multiplierY < 0.1f) multiplierY = 0.1f;

            InternalRenderMode = RenderMode.PerAxisScale;
            InternalRenderMultiplier = multiplierX;
            InternalRenderMultiplierVertical = multiplierY;

            SetDownsamplingSettings(false);

            var vrComponent = GetComponent<MadGoatSSAA_VR>();
            if (vrComponent && vrComponent != this) vrComponent.SetAsAxisBased(multiplierX, multiplierY);
        }
        /// <summary>
        /// Set the multiplier of each screen axis independently while using the downsampling filter.
        /// </summary>
        /// <param name="multiplierX"></param>
        /// <param name="multiplierY"></param>
        public virtual void SetAsAxisBased(float multiplierX, float multiplierY, Filter filterType, float sharpness, float sampleDistance) {
            // check for invalid values
            if (multiplierX < 0.1f) multiplierX = 0.1f;
            if (multiplierY < 0.1f) multiplierY = 0.1f;

            InternalRenderMode = RenderMode.PerAxisScale;
            InternalRenderMultiplier = multiplierX;
            InternalRenderMultiplierVertical = multiplierY;

            SetDownsamplingSettings(filterType, sharpness, sampleDistance);

            var vrComponent = GetComponent<MadGoatSSAA_VR>();
            if (vrComponent && vrComponent != this) vrComponent.SetAsAxisBased(multiplierX, multiplierY, filterType, sharpness, sampleDistance);
        }

        /// <summary>
        /// Set the downsampling shader parameters. If the case, this should be called after setting the mode, otherwise it might get overrided. (ex: SSAA)
        /// </summary>
        public void SetDownsamplingSettings(bool useFilter) {
            DownsamplerEnabled = useFilter;
            DownsamplerFilter = useFilter ? Filter.BILINEAR : Filter.POINT;
            DownsamplerSharpness = useFilter ? 0.85f : 0; // 0.85 should work fine for any resolution 
            DownsamplerDistance = useFilter ? 0.9f : 0; // 0.9 should work fine for any res

            var vrComponent = GetComponent<MadGoatSSAA_VR>();
            if (vrComponent) vrComponent.SetDownsamplingSettings(useFilter);

        }
        /// <summary>
        /// Set the downsampling shader parameters. If the case, this should be called after setting the mode, otherwise it might get overrided. (ex: SSAA)
        /// </summary>
        public void SetDownsamplingSettings(Filter FilterType, float sharpness, float sampledist) {
            DownsamplerEnabled = true;
            DownsamplerFilter = FilterType;
            DownsamplerSharpness = Mathf.Clamp(sharpness, 0, 1);
            DownsamplerDistance = Mathf.Clamp(sampledist, 0.5f, 1.5f);

            var vrComponent = GetComponent<MadGoatSSAA_VR>();
            if (vrComponent && vrComponent != this) vrComponent.SetDownsamplingSettings(FilterType, sharpness, sampledist);
        }
        /// <summary>
        /// Enable or disable the ultra mode for super sampling.(FSS)
        /// </summary>
        /// <param name="postAntiAliasing"></param>
        public void SetPostAAMode(PostAntiAliasingMode postAntiAliasing) {
            PostAntiAliasing = postAntiAliasing;

            var vrComponent = GetComponent<MadGoatSSAA_VR>();
            if (vrComponent && vrComponent != this) vrComponent.SetPostAAMode(postAntiAliasing);
        }

        /// <summary>
        /// Take a screenshot of resolution Size (x is width, y is height) rendered at a higher resolution given by the multiplier. The screenshot is saved at the given path in PNG format.
        /// </summary>
        public virtual void TakeScreenshot(string path, Vector2 Size, int multiplier) {
            // Take screenshot with default settings
            ScreenshotCaptureSettings.takeScreenshot = true;
            ScreenshotCaptureSettings.outputResolution = Size;
            ScreenshotCaptureSettings.screenshotMultiplier = multiplier;
            ScreenshotPath = path;

            ScreenshotCaptureSettings.downsamplerEnabled = false;
        }
        /// <summary>
        /// Take a screenshot of resolution Size (x is width, y is height) rendered at a higher resolution given by the multiplier. The screenshot is saved at the given path in PNG format. Uses given post process AA method on top of SSAA
        /// </summary>
        public virtual void TakeScreenshot(string path, Vector2 Size, int multiplier, PostAntiAliasingMode postAntiAliasing) {
            // Take screenshot with default settings
            ScreenshotCaptureSettings.takeScreenshot = true;
            ScreenshotCaptureSettings.outputResolution = Size;
            ScreenshotCaptureSettings.screenshotMultiplier = multiplier;
            ScreenshotPath = path;

            ScreenshotCaptureSettings.downsamplerEnabled = false;
            ScreenshotCaptureSettings.postAntiAliasing = postAntiAliasing;
        }
        /// <summary>
        /// Take a screenshot of resolution Size (x is width, y is height) rendered at a higher resolution given by the multiplier. The screenshot is saved at the given path in PNG format.
        /// </summary>
        public virtual void TakeScreenshot(string path, Vector2 size, int multiplier, Filter filterType, float sharpness, float sampleDistance) {
            ScreenshotCaptureSettings.takeScreenshot = true;
            ScreenshotCaptureSettings.outputResolution = size;
            ScreenshotCaptureSettings.screenshotMultiplier = multiplier;
            ScreenshotPath = path;
            ScreenshotCaptureSettings.downsamplerEnabled = true;
            screenshotCaptureSettings.downsamplerFilter = filterType;
            ScreenshotCaptureSettings.downsamplerSharpness = Mathf.Clamp(sharpness, 0, 1);
            ScreenshotCaptureSettings.downsamplerDistance = Mathf.Clamp(sampleDistance, 0, 2);
            ScreenshotCaptureSettings.postAntiAliasing = PostAntiAliasingMode.Off;
        }
        /// <summary>
        /// Take a screenshot of resolution Size (x is width, y is height) rendered at a higher resolution given by the multiplier. The screenshot is saved at the given path in PNG format. Uses given post process AA method on top of SSAA.
        /// </summary>
        public virtual void TakeScreenshot(string path, Vector2 size, int multiplier, Filter filterType, float sharpness, float sampleDistance, PostAntiAliasingMode postAntiAliasing) {
            ScreenshotCaptureSettings.takeScreenshot = true;
            ScreenshotCaptureSettings.outputResolution = size;
            ScreenshotCaptureSettings.screenshotMultiplier = multiplier;
            ScreenshotPath = path;
            ScreenshotCaptureSettings.downsamplerEnabled = true;
            screenshotCaptureSettings.downsamplerFilter = filterType;
            ScreenshotCaptureSettings.downsamplerSharpness = Mathf.Clamp(sharpness, 0, 1);
            ScreenshotCaptureSettings.downsamplerDistance = Mathf.Clamp(sampleDistance, 0, 2);
            ScreenshotCaptureSettings.postAntiAliasing = postAntiAliasing;
        }

        /// <summary>
        /// Sets up the screenshot module to use the PNG image format. This enables transparency in output images.
        /// </summary>
        public virtual void SetScreenshotModuleToPNG() {
            this.ScreenshotFormat = ImageFormat.PNG;
        }
        /// <summary>
        /// Sets up the screenshot module to use the JPG image format. Quality is parameter from 1 to 100 and represents the compression quality of the JPG file. Incorrect quality values will be clamped.
        /// </summary>
        /// <param name="quality"></param>
        public virtual void SetScreenshotModuleToJPG(int quality) {
            this.ScreenshotFormat = ImageFormat.JPG;
            this.ScreenshotQuality = Mathf.Clamp(1, 100, quality);
        }
#if UNITY_5_6_OR_NEWER
        /// <summary>
        /// Sets up the screenshot module to use the EXR image format. The EXR32 bool parameter dictates whether to use or not 32 bit exr encoding. This method is only available in Unity 5.6 and newer.
        /// </summary>
        /// <param name="EXR32"></param>
        public virtual void SetScreenshotModuleToEXR(bool EXR32) {
            this.ScreenshotFormat = ImageFormat.EXR;
            this.ScreenshotExr32 = EXR32;
        }
#endif
        /// <summary>
        /// Return string with current internal resolution
        /// </summary>
        /// <returns></returns>
        public virtual string GetResolution() {
            return (int)(CameraTargetWidth * InternalRenderMultiplier) + "x" + (int)(CameraTargetHeight * InternalRenderMultiplier);
        }
        #endregion

        #region Global API
        /// <summary>
        /// Set rendering mode to given SSAA mode
        /// </summary>
        public static void SetAllAsSSAA(SSAAMode mode) {
            foreach (MadGoatSSAA ssaa in FindObjectsOfType<MadGoatSSAA>())
                ssaa.SetAsSSAA(mode);
        }
        /// <summary>
        /// Set a custom resolution multiplier
        /// </summary>
        public static void SetAllAsScale(float multiplier) {
            foreach (MadGoatSSAA ssaa in FindObjectsOfType<MadGoatSSAA>())
                ssaa.SetAsScale(multiplier);
        }
        /// <summary>
        /// Set a custom resolution multiplier, and use custom downsampler settings
        /// </summary>
        public static void SetAllAsScale(float multiplier, Filter filterType, float sharpness, float sampleDistance) {
            foreach (MadGoatSSAA ssaa in FindObjectsOfType<MadGoatSSAA>())
                ssaa.SetAsScale(multiplier, filterType, sharpness, sampleDistance);
        }
        /// <summary>
        /// Set the resolution scale to a given percent
        /// </summary>
        public static void SetAllAsScale(int percent) {
            foreach (MadGoatSSAA ssaa in FindObjectsOfType<MadGoatSSAA>())
                ssaa.SetAsScale(percent);
        }
        /// <summary>
        /// Set the resolution scale to a given percent, and use custom downsampler settings
        /// </summary>
        public static void SetAllAsScale(int percent, Filter filterType, float sharpness, float sampleDistance) {
            foreach (MadGoatSSAA ssaa in FindObjectsOfType<MadGoatSSAA>())
                ssaa.SetAsScale(percent, filterType, sharpness, sampleDistance);
        }

        /// <summary>
        /// Set the operation mode as adaptive with target frame rate
        /// </summary>
        /// <param name="minMultiplier"></param>
        /// <param name="maxMultiplier"></param>
        /// <param name="targetFramerate"></param>
        public static void SetAllAsAdaptive(float minMultiplier, float maxMultiplier, int targetFramerate) {
            foreach (MadGoatSSAA ssaa in FindObjectsOfType<MadGoatSSAA>())
                ssaa.SetAsAdaptive(minMultiplier, maxMultiplier, targetFramerate);
        }
        /// <summary>
        /// Set the operation mode as adaptive with screen refresh rate as target frame rate
        /// </summary>
        /// <param name="minMultiplier"></param>
        /// <param name="maxMultiplier"></param>
        public static void SetAllAsAdaptive(float minMultiplier, float maxMultiplier) {
            foreach (MadGoatSSAA ssaa in FindObjectsOfType<MadGoatSSAA>())
                ssaa.SetAsAdaptive(minMultiplier, maxMultiplier);
        }
        /// <summary>
        /// Set the operation mode as adaptive with target frame rate and use downsampling filter.
        /// </summary>
        /// <param name="minMultiplier"></param>
        /// <param name="maxMultiplier"></param>
        /// <param name="targetFramerate"></param>
        /// <param name="filterType"></param>
        /// <param name="sharpness"></param>
        /// <param name="sampleDistance"></param>
        public static void SetAllAsAdaptive(float minMultiplier, float maxMultiplier, int targetFramerate, Filter filterType, float sharpness, float sampleDistance) {
            foreach (MadGoatSSAA ssaa in FindObjectsOfType<MadGoatSSAA>())
                ssaa.SetAsAdaptive(minMultiplier, maxMultiplier, targetFramerate, filterType, sharpness, sampleDistance);
        }
        /// <summary>
        /// Set the operation mode as adaptive with screen refresh rate as target frame rate and use downsampling filter.
        /// </summary>
        /// <param name="minMultiplier"></param>
        /// <param name="maxMultiplier"></param>
        /// <param name="filterType"></param>
        /// <param name="sharpness"></param>
        /// <param name="sampleDistance"></param>
        public static void SetAllAsAdaptive(float minMultiplier, float maxMultiplier, Filter filterType, float sharpness, float sampleDistance) {
            foreach (MadGoatSSAA ssaa in FindObjectsOfType<MadGoatSSAA>())
                ssaa.SetAsAdaptive(minMultiplier, maxMultiplier, filterType, sharpness, sampleDistance);
        }

        /// <summary>
        /// Set the multiplier of each screen axis independently. does not use downsampling filter.
        /// </summary>
        /// <param name="multiplierX"></param>
        /// <param name="multiplierY"></param>
        public static void SetAllAsAxisBased(float multiplierX, float multiplierY) {
            foreach (MadGoatSSAA ssaa in FindObjectsOfType<MadGoatSSAA>())
                ssaa.SetAsAxisBased(multiplierX, multiplierY);
        }
        /// <summary>
        ///  Set the multiplier of each screen axis independently while using the downsampling filter.
        /// </summary>
        public static void SetAllAsAxisBased(float multiplierX, float multiplierY, Filter filterType, float sharpness, float sampleDistance) {
            foreach (MadGoatSSAA ssaa in FindObjectsOfType<MadGoatSSAA>())
                ssaa.SetAsAxisBased(multiplierX, multiplierY, filterType, sharpness, sampleDistance);
        }

        /// <summary>
        /// Set the downsampling shader parameters. If the case, this should be called after setting the mode, otherwise it might get overrided. (ex: SSAA)
        /// </summary>
        public static void SetAllDownsamplingSettings(bool use) {
            foreach (MadGoatSSAA ssaa in FindObjectsOfType<MadGoatSSAA>())
                ssaa.SetDownsamplingSettings(use);
        }
        /// <summary>
        /// Set the downsampling shader parameters. If the case, this should be called after setting the mode, otherwise it might get overrided. (ex: SSAA)
        /// </summary>
        public static void SetAllDownsamplingSettings(Filter filterType, float sharpness, float sampleDistance) {
            foreach (MadGoatSSAA ssaa in FindObjectsOfType<MadGoatSSAA>())
                ssaa.SetDownsamplingSettings(filterType, sharpness, sampleDistance);
        }
        /// <summary>
        /// Enable or disable the ultra mode for super sampling.(FSS)
        /// </summary>
        /// <param name="mode"></param>
        public static void SetAllPostAAMode(PostAntiAliasingMode mode) {
            foreach (MadGoatSSAA ssaa in FindObjectsOfType<MadGoatSSAA>())
                ssaa.SetPostAAMode(mode);
        }
        #endregion

        #region Deprecated - Will be removed in the future
        #region Instance API
        /// <summary>
        /// Set a custom resolution multiplier
        /// </summary>
        [System.Obsolete("SSAA SetAsCustom() has been deprecated. Use SetAsScale instead")]
        public void SetAsCustom(float Multiplier) {
            // check for invalid values
            if (Multiplier < 0.1f) Multiplier = 0.1f;

            InternalRenderMode = RenderMode.Custom;
            InternalRenderMultiplier = Multiplier;

            SetDownsamplingSettings(false);
        }
        /// <summary>
        /// Set a custom resolution multiplier, and use custom downsampler settings
        /// </summary>
        [System.Obsolete("SSAA SetAsCustom() has been deprecated. Use SetAsScale instead")]
        public void SetAsCustom(float Multiplier, Filter FilterType, float sharpnessfactor, float sampledist) {
            // check for invalid values
            if (Multiplier < 0.1f) Multiplier = 0.1f;

            InternalRenderMode = RenderMode.Custom;
            InternalRenderMultiplier = Multiplier;

            SetDownsamplingSettings(FilterType, sharpnessfactor, sampledist);
        }
        /// <summary>
        /// Take a screenshot of resolution Size (x is width, y is height) rendered at a higher resolution given by the multiplier and use the bicubic downsampler. The screenshot is saved at the given path in PNG format. 
        /// </summary>
        [System.Obsolete()]
        public virtual void TakeScreenshot(string path, Vector2 Size, int multiplier, float sharpness) {
            // Take screenshot with custom settings
            TakeScreenshot(path, Size, multiplier, Filter.BICUBIC, sharpness, 1);
        }
        /// <summary>
        /// Returns a ray from a given screenpoint
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [Obsolete("SSAA ScreenPointToRay has been deprecated. Use Camera's API instead")]
        public virtual Ray ScreenPointToRay(Vector3 position) {
            return renderCamera.ScreenPointToRay(position);
        }
        /// <summary>
        /// Transforms postion from screen space into viewport space.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [Obsolete("SSAA ScreenToViewportPoint has been deprecated. Use Camera's API instead")]
        public virtual Vector3 ScreenToViewportPoint(Vector3 position) {
            return renderCamera.ScreenToViewportPoint(position);
        }
        /// <summary>
        /// Transforms position from screen space into world space
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [Obsolete("SSAA ScreenToWorldPoint has been deprecated. Use Camera's API instead")]
        public virtual Vector3 ScreenToWorldPoint(Vector3 position) {
            return renderCamera.ScreenToWorldPoint(position);
        }
        /// <summary>
        /// Transforms position from world space to screen space
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [Obsolete("SSAA WorldToScreenPoint has been deprecated. Use Camera's API instead")]
        public virtual Vector3 WorldToScreenPoint(Vector3 position) {
            return renderCamera.WorldToScreenPoint(position);
        }
        /// <summary>
        /// Transforms position from viewport space to screen space
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        [Obsolete("SSAA ViewportToScreenPoint has been deprecated. Use Camera's API instead")]
        public virtual Vector3 ViewportToScreenPoint(Vector3 position) {
            return renderCamera.ViewportToScreenPoint(position);
        }
        #endregion

        #region Global API
        /// <summary>
        /// Set a custom resolution multiplier
        /// </summary>
        [System.Obsolete("SetAsCustom() has been deprecated. Use SetAsScale instead")]
        public static void SetAllAsCustom(float Multiplier) {
            foreach (MadGoatSSAA ssaa in FindObjectsOfType<MadGoatSSAA>())
                ssaa.SetAsCustom(Multiplier);
        }
        /// <summary>
        /// Set a custom resolution multiplier, and use custom downsampler settings
        /// </summary>
        [System.Obsolete("SetAsCustom() has been deprecated. Use SetAsScale instead")]
        public static void SetAllAsCustom(float Multiplier, Filter FilterType, float sharpnessfactor, float sampledist) {
            foreach (MadGoatSSAA ssaa in FindObjectsOfType<MadGoatSSAA>())
                ssaa.SetAsCustom(Multiplier, FilterType, sharpnessfactor, sampledist);
        }
        #endregion
        #endregion
        #endregion
    }
}

