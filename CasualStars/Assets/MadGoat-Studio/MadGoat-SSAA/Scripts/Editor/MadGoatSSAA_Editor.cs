using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MadGoat.Core.Utils;
using MadGoat.Core.EditorUtils;

namespace MadGoat.SSAA {
    [CustomEditor(typeof(MadGoatSSAA))]
    public class MadGoatSSAA_Editor : Editor {

        #region Utils
        public enum TextureMode {
            Half,
            Float
        }
        protected void SetupStyles() {
            if (styleNormal == null) styleNormal = Core.EditorUtils.EditorStyles.NewFooterStyle(Application.HasProLicense() ? colorNormalPro : colorNormalBasic);
            if (styleTitle == null) styleTitle = Core.EditorUtils.EditorStyles.NewTitleStyle(colorAccentPro);
            if (styleHeader == null) styleHeader = Core.EditorUtils.EditorStyles.NewHeaderStyle(colorAccentPro);

            if (styleFoldout == null) styleFoldout = Core.EditorUtils.EditorStyles.NewFoldoutStyle();
            if (styleRichLabel == null) styleRichLabel = Core.EditorUtils.EditorStyles.NewRichLabelStyle();
            if (styleContentBox == null) styleContentBox = Core.EditorUtils.EditorStyles.NewContentBoxStyle();
        }
        protected int Getmode() {
            return (int)(target as MadGoatSSAA).SsaaMode;
        }
        #endregion

        #region Fields for Serialization
        // target
        private MadGoatSSAA targetAsSSAA;
        private SerializedObject serializedTarget;

        // Render settings
        private SerializedProperty internalTextureFormat;
        private SerializedProperty internalRenderLayerMask;
        private SerializedProperty internalRenderMode;
        private SerializedProperty internalRenderMultiplier;
        private SerializedProperty internalRenderMultiplierVertical;

        // Adaptive Resolution
        private SerializedProperty adaptiveResMinMultiplier;
        private SerializedProperty adaptiveResMaxMultiplier;
        private SerializedProperty adaptiveResTargetFps;
        private SerializedProperty adaptiveResTargetVsync;

        // Supersampling AA
        private SerializedProperty ssaaProfileHalf;
        private SerializedProperty ssaaProfileX2;
        private SerializedProperty ssaaProfileX4;

        // Post AA
        private SerializedProperty postAntiAliasing;

        // Downsampler
        private SerializedProperty downsamplerEnabled;
        private SerializedProperty downsamplerFilter;
        private SerializedProperty downsamplerSharpness;
        private SerializedProperty downsamplerDistance;

        // Screenshots
        protected SerializedProperty screenshotCaptureSettings;
        protected SerializedProperty screenshotPanoramaSettings;
        protected SerializedProperty screenshotPath;
        protected SerializedProperty screenshotPrefix;
        protected SerializedProperty screenshotPrefixIsProduct;
        protected SerializedProperty screenshotQuality;
        protected SerializedProperty screenshotExr32;
        protected SerializedProperty screenshotFormat;

        protected SerializedProperty extensionIPointerEvents;
        protected SerializedProperty extensionPostProcessingStack;
        protected SerializedProperty extensionCinemachine;

        // inspector only
        protected SerializedProperty inspectorShowHelp;
        #endregion

        #region Fields for UI 
        // UI Control fields
        private int mainTab;
        private int ssaaMode;
        private static string[] ssaaModeStrings = new string[] { "Off", "0.5x", "2x", "4x" };

        private bool ssaaHalfUnfold = false;
        private bool ssaaX2Unfold = false;
        private bool ssaaX4Unfold = false;

        private bool extendedMultiplier;
        private TextureMode currentMode = TextureMode.Float;

        // UI Style fields
        protected static Color colorAccentBasic = new Color(0.5f, 0.1f, 0.1f);
        protected static Color colorAccentPro = new Color(0, .7f, 1);

        protected static Color colorNormalBasic = Color.black;
        protected static Color colorNormalPro = new Color(.75f, .75f, .75f);

        protected GUIStyle styleNormal;
        protected GUIStyle styleTitle;
        protected GUIStyle styleHeader;

        protected GUIStyle styleFoldout;
        protected GUIStyle styleRichLabel;
        protected GUIStyle styleContentBox;
        #endregion

        #region Editor Implementation
        public void OnEnable() {
            serializedTarget = new SerializedObject(target);
            targetAsSSAA = (target as MadGoatSSAA);

            // Render settings
            internalTextureFormat = serializedTarget.FindProperty("internalTextureFormat");
            internalRenderMode = serializedTarget.FindProperty("internalRenderMode");
            internalRenderMultiplier = serializedTarget.FindProperty("internalRenderMultiplier");
            internalRenderMultiplierVertical = serializedTarget.FindProperty("internalRenderMultiplierVertical");
            internalRenderLayerMask = serializedTarget.FindProperty("internalRenderLayerMask");

            // Supersampling AA
            ssaaProfileHalf = serializedTarget.FindProperty("ssaaProfileHalf");
            ssaaProfileX2 = serializedTarget.FindProperty("ssaaProfileX2");
            ssaaProfileX4 = serializedTarget.FindProperty("ssaaProfileX4");

            // Post AA
            postAntiAliasing = serializedTarget.FindProperty("postAntiAliasing");

            // Downsampler
            downsamplerEnabled = serializedTarget.FindProperty("downsamplerEnabled");
            downsamplerFilter = serializedTarget.FindProperty("downsamplerFilter");
            downsamplerSharpness = serializedTarget.FindProperty("downsamplerSharpness");
            downsamplerDistance = serializedTarget.FindProperty("downsamplerDistance");

            // Adaptive Resolution
            adaptiveResTargetFps = serializedTarget.FindProperty("adaptiveResTargetFps");
            adaptiveResTargetVsync = serializedTarget.FindProperty("adaptiveResTargetVsync");
            adaptiveResMinMultiplier = serializedTarget.FindProperty("adaptiveResMinMultiplier");
            adaptiveResMaxMultiplier = serializedTarget.FindProperty("adaptiveResMaxMultiplier");

            // Screenshots
            screenshotPath = serializedTarget.FindProperty("screenshotPath");
            screenshotPrefix = serializedTarget.FindProperty("screenshotPrefix");
            screenshotPrefixIsProduct = serializedTarget.FindProperty("screenshotPrefixIsProduct");
            screenshotQuality = serializedTarget.FindProperty("screenshotQuality");
            screenshotFormat = serializedTarget.FindProperty("screenshotFormat");
            screenshotExr32 = serializedTarget.FindProperty("screenshotExr32");
            screenshotCaptureSettings = serializedTarget.FindProperty("screenshotCaptureSettings");
            screenshotPanoramaSettings = serializedTarget.FindProperty("screenshotPanoramaSettings");

            extensionIPointerEvents = serializedTarget.FindProperty("extensionIPointerEvents");
            extensionPostProcessingStack = serializedTarget.FindProperty("extensionPostProcessingStack");
            extensionCinemachine = serializedTarget.FindProperty("extensionCinemachine");

            // Inspector only
            inspectorShowHelp = serializedTarget.FindProperty("inspectorShowHelp");
        }
        public override void OnInspectorGUI() {
            SetupStyles();

            serializedTarget.Update();

            // Draw Title "MadGoat SSAA & Resolution Scale 2"
            EditorMacros.DrawTitle("MadGoat SSAA & Resolution Scale 2", styleTitle);

            var pipeline = RenderPipelineUtils.DetectPipeline();
            if (pipeline != RenderPipelineUtils.PipelineType.Unsupported) {

                // Draw Main Tab
                mainTab = GUILayout.Toolbar(mainTab, new string[] { "Super Sampling", "Screenshot", "Integrations & Misc" });
                EditorGUILayout.Separator();

                // Get current render resolution
                int width = 0;
                int height = 0;
                if (targetAsSSAA.GetType() != typeof(MadGoatSSAA_VR)) {
                    if (targetAsSSAA.RenderCamera && targetAsSSAA.RenderCamera.targetTexture) {
                        width = targetAsSSAA.RenderCamera.targetTexture.width;
                        height = targetAsSSAA.RenderCamera.targetTexture.height;
                    }
                }
                else {
#if UNITY_2017_2_OR_NEWER
                    width = (int)(UnityEngine.XR.XRSettings.eyeTextureWidth * targetAsSSAA.InternalRenderMultiplier);
                    height = (int)(UnityEngine.XR.XRSettings.eyeTextureHeight * targetAsSSAA.InternalRenderMultiplier);
#else
                    width = (int)(UnityEngine.VR.VRSettings.eyeTextureWidth * targetAsSSAA.InternalRenderMultiplier);
                    height = (int)(UnityEngine.VR.VRSettings.eyeTextureHeight * targetAsSSAA.InternalRenderMultiplier);
#endif
                }
                var resolution = width + " x " + height + " (x" + targetAsSSAA.InternalRenderMultiplier + ")";
                if (targetAsSSAA != null && targetAsSSAA.GetType() != typeof(MadGoatSSAA_VR) && targetAsSSAA.CurrentCamera != null && targetAsSSAA.CurrentCamera.targetTexture != null)
                    resolution += " - Output is texture";

                // Draw Content Header
                EditorMacros.DrawHeader(new string[] { "Super Sampling Settings - " + resolution, "Screenshot Settings", "Integrations" }[mainTab], styleHeader);

                // Draw Content
                switch (mainTab) {
                    case 0:
                        DrawSectionSuperSampling();
                        break;
                    case 1:
                        DrawSectionScreenshot();
                        break;
                    case 2:
                        DrawSectionMisc();
                        break;
                }
            }
            else {
                EditorGUILayout.HelpBox("<b>Unsupported RenderPipeline detected.</b> \n\n" +
                   "Pipeline Requirements: \n" +
                   "   - HDRP: Unity 2019.1 or newer \n" +
                   "   - HDRP + DXR: Unity 2019.3 or newer \n" +
                   "   - Universal Pipeline: Unity 2019.3 or newer \n" +
                   "   - Built-in: Unity 5 or newer", MessageType.Error);
            }

            // Draw Version Footer
            var versionStr = RenderPipelineUtils.DetectPipeline().ToString();
#if UNITY_2019_3_OR_NEWER && SSAA_HDRP && ENABLE_RAYTRACING
            versionStr += " <b>(DXR Enabled)</b>";
#endif
            EditorMacros.DrawVersion(SsaaUtils.ssaaversion, versionStr, styleNormal);

            // Apply modifications
            serializedTarget.ApplyModifiedProperties();
        }
        #endregion

        public virtual void DrawSectionSuperSampling() {
#if UNITY_2017_2_OR_NEWER
            EditorApplication.QueuePlayerLoopUpdate();
#endif
            EditorGUILayout.BeginVertical(styleContentBox);
            // SSAA tab 
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(internalRenderMode, new GUIContent("Operation mode"), true);
            if (targetAsSSAA.GetType() != typeof(MadGoatSSAA_VR)) {
                EditorGUILayout.PropertyField(internalRenderLayerMask, new GUIContent("Internal Render Culling Mask"));
            }
            // Draw content based on supersampling type
            if (internalRenderMode.intValue == 0)       // SSAA presets
                DrawSSAA();
            else if (internalRenderMode.intValue == 1)  // Resolution scale
                DrawResScale();
            else if (internalRenderMode.intValue == 2)  // per axis
                DrawPerAxis();
            else if (internalRenderMode.intValue == 4)  // Custom 
                DrawCustom();
            else                                        // Adaptive
                DrawAdaptive();

            // Draw downsampler settings
            if (internalRenderMode.intValue != 0) {


                // Title
                styleTitle.fontSize = 12;
                EditorMacros.DrawHeader("Downsampling", styleHeader);
                EditorGUILayout.BeginVertical(styleContentBox);
                var unsupportedFilter = false;

#if SSAA_HDRP || SSAA_URP
                if (targetAsSSAA.GetType() == typeof(MadGoatSSAA_VR)) {
                    EditorGUI.indentLevel--;
                    EditorGUILayout.HelpBox("Downsampling Filtering on VR under Scriptable Render Pipelines is currently not supported", MessageType.Warning);
                    EditorGUI.indentLevel++;
                    unsupportedFilter = true;
                }
#endif

                if (!unsupportedFilter) {
                    // Hint box
                    if (inspectorShowHelp.boolValue) {
                        EditorGUI.indentLevel--;
                        EditorGUILayout.HelpBox("If using image filtering, the render image will be passed through a custom downsampling filter. If not, it will be resized as is.", MessageType.Info);
                        EditorGUI.indentLevel++;
                    }

                    // Settings
                    downsamplerEnabled.boolValue = EditorGUILayout.Toggle("Use Filter", downsamplerEnabled.boolValue);
                    if (downsamplerEnabled.boolValue) {
                        EditorGUILayout.PropertyField(downsamplerFilter, new GUIContent("Downsampler Filter"));
                        downsamplerSharpness.floatValue = EditorGUILayout.Slider("Downsampler Sharpness", downsamplerSharpness.floatValue, 0f, 1f);

                        if (downsamplerFilter.intValue == 1)
                            downsamplerDistance.floatValue = EditorGUILayout.Slider("Distance Between Samples", downsamplerDistance.floatValue, 0.5f, 2f);
                    }
                    EditorGUILayout.Separator();
                }
                EditorGUILayout.EndVertical();

            }
            // Dpdate vertical for later usage in ui
            if (internalRenderMode.intValue != 2)
                internalRenderMultiplierVertical.floatValue = internalRenderMultiplier.floatValue;
            EditorGUI.indentLevel--;
        }
        public virtual void DrawSectionScreenshot() // Screenshot Tab 
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical(styleContentBox);
            EditorGUILayout.PropertyField(screenshotPath, new GUIContent("Save path"));
            EditorGUILayout.PropertyField(screenshotPrefixIsProduct);
            if (!screenshotPrefixIsProduct.boolValue)
                EditorGUILayout.PropertyField(screenshotPrefix, new GUIContent("File Name Prefix"));


            EditorGUILayout.Space();
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(screenshotCaptureSettings.FindPropertyRelative("outputResolution"), new GUIContent("Screenshot Resolution"));

            // the screenshot module
            EditorGUILayout.PropertyField(screenshotFormat, new GUIContent("Output Image Format"));

            EditorGUI.indentLevel++;
            if (screenshotFormat.enumValueIndex == 0)
                EditorGUILayout.PropertyField(screenshotQuality, new GUIContent("JPG Quality"));
            if (screenshotFormat.enumValueIndex == 2)
                EditorGUILayout.PropertyField(screenshotExr32, new GUIContent("32-bit EXR"));
            EditorGUI.indentLevel--;

            EditorGUILayout.Separator();
            EditorGUILayout.EndVertical();

            var res = targetAsSSAA.ScreenshotCaptureSettings.outputResolution * targetAsSSAA.ScreenshotCaptureSettings.screenshotMultiplier;
            var width = (int)res.x;
            var height = (int)res.y;

            EditorMacros.DrawHeader("Render Resolution - " + width + " x " + height, styleHeader);
            //GUILayout.Label(, accent_style);

            EditorGUILayout.BeginVertical(styleContentBox);
            EditorGUILayout.PropertyField(screenshotCaptureSettings.FindPropertyRelative("screenshotMultiplier"), new GUIContent("Render Resolution Multiplier"));
            EditorGUILayout.PropertyField(screenshotCaptureSettings.FindPropertyRelative("postAntiAliasing"));
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(screenshotCaptureSettings.FindPropertyRelative("downsamplerEnabled"), new GUIContent("Use Filter"));
            if (screenshotCaptureSettings.FindPropertyRelative("downsamplerEnabled").boolValue) {
                EditorGUILayout.PropertyField(screenshotCaptureSettings.FindPropertyRelative("downsamplerFilter"));
                EditorGUI.indentLevel++;
                if (screenshotCaptureSettings.FindPropertyRelative("downsamplerFilter").enumValueIndex == 2)
                    screenshotCaptureSettings.FindPropertyRelative("downsamplerSharpness").floatValue = EditorGUILayout.Slider("Sharpness", screenshotCaptureSettings.FindPropertyRelative("downsamplerSharpness").floatValue, 0, 1);
                else if (screenshotCaptureSettings.FindPropertyRelative("downsamplerFilter").enumValueIndex == 1) {
                    screenshotCaptureSettings.FindPropertyRelative("downsamplerSharpness").floatValue = EditorGUILayout.Slider("Sharpness", screenshotCaptureSettings.FindPropertyRelative("downsamplerSharpness").floatValue, 0, 1);
                    screenshotCaptureSettings.FindPropertyRelative("downsamplerDistance").floatValue = EditorGUILayout.Slider("Sample Distance", screenshotCaptureSettings.FindPropertyRelative("downsamplerDistance").floatValue, 0, 1);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Separator();
            EditorGUILayout.EndVertical();
            if (GUILayout.Button(Application.isPlaying ? "Take Screenshot" : "Only available in play mode")) {
                if (Application.isPlaying) {

                    if (screenshotCaptureSettings.FindPropertyRelative("downsamplerEnabled").boolValue)
                        targetAsSSAA.TakeScreenshot(
                            targetAsSSAA.ScreenshotPath,
                            targetAsSSAA.ScreenshotCaptureSettings.outputResolution,
                            targetAsSSAA.ScreenshotCaptureSettings.screenshotMultiplier,
                            targetAsSSAA.ScreenshotCaptureSettings.downsamplerFilter,
                            targetAsSSAA.ScreenshotCaptureSettings.downsamplerSharpness,
                            targetAsSSAA.ScreenshotCaptureSettings.downsamplerDistance,
                            targetAsSSAA.ScreenshotCaptureSettings.postAntiAliasing
                            );
                    else
                        targetAsSSAA.TakeScreenshot(
                            targetAsSSAA.ScreenshotPath,
                            targetAsSSAA.ScreenshotCaptureSettings.outputResolution,
                            targetAsSSAA.ScreenshotCaptureSettings.screenshotMultiplier,
                            targetAsSSAA.ScreenshotCaptureSettings.postAntiAliasing
                            );
                }
            }
            EditorGUI.indentLevel--;
        }
        public virtual void DrawSectionMisc() // General Tab 
        {
            #region Extension IPointer Events
            bool extensionIPointerEventsSupport = targetAsSSAA.ExtensionIPointerEvents.IsSupported();
            if (extensionIPointerEventsSupport) GUI.color = extensionIPointerEvents.FindPropertyRelative("enabled").boolValue ? new Color(.5f, 1f, .5f) : new Color(.8f, .8f, .8f);
            else GUI.color = new Color(1f, .3f, .3f);
            EditorGUILayout.BeginHorizontal(styleHeader);
            {
                GUI.color = Color.white;
                EditorGUI.indentLevel++;
                extensionIPointerEvents.FindPropertyRelative("inspectorFoldout").boolValue = EditorGUILayout.Foldout(
                    extensionIPointerEvents.FindPropertyRelative("inspectorFoldout").boolValue,
                    "Unity IPointerEvents Integration",
                    true,
                    styleFoldout);
                EditorGUI.indentLevel--;
                GUI.color = colorAccentPro;
                if (GUILayout.Button(extensionIPointerEvents.FindPropertyRelative("enabled").boolValue ? "Disable" : "Enable", GUILayout.MaxWidth(100), GUILayout.MinWidth(100))) {
                    targetAsSSAA.enabled = false;
                    targetAsSSAA.ExtensionIPointerEvents.enabled = !targetAsSSAA.ExtensionIPointerEvents.enabled;
                    targetAsSSAA.enabled = true;
                }
                GUI.color = Color.white;
            }
            EditorGUILayout.EndHorizontal();
            if (extensionIPointerEvents.FindPropertyRelative("inspectorFoldout").boolValue) {
                EditorGUILayout.BeginVertical(styleContentBox);
                EditorGUILayout.HelpBox(SSAAExtensionPointerEventsSupport.description, MessageType.Info);
                EditorGUILayout.PropertyField(extensionIPointerEvents.FindPropertyRelative("eventsLayerMask"));
                EditorGUILayout.EndVertical();
            }
            #endregion

            #region Extension Post Processing Stack 2
            bool extensionPostProcessingStackSupport = targetAsSSAA.ExtensionPostProcessingStack.IsSupported();
            if (extensionPostProcessingStackSupport) GUI.color = extensionPostProcessingStack.FindPropertyRelative("enabled").boolValue ? new Color(.5f, 1f, .5f) : new Color(.8f, .8f, .8f);
            else GUI.color = new Color(1f, .3f, .3f);
            EditorGUILayout.BeginHorizontal(styleHeader);
            {
                GUI.color = Color.white;
                EditorGUI.indentLevel++;
                extensionPostProcessingStack.FindPropertyRelative("inspectorFoldout").boolValue = EditorGUILayout.Foldout(
                    extensionPostProcessingStack.FindPropertyRelative("inspectorFoldout").boolValue,
                    "Unity PostProcessing Stack V2 Integration",
                    true,
                    styleFoldout);

                EditorGUI.indentLevel--;
                GUI.color = colorAccentPro;
                if (extensionPostProcessingStackSupport) {
                    if (GUILayout.Button(extensionPostProcessingStack.FindPropertyRelative("enabled").boolValue ? "Disable" : "Enable", GUILayout.MaxWidth(100), GUILayout.MinWidth(100))) {
                        targetAsSSAA.enabled = false;
                        targetAsSSAA.ExtensionPostProcessingStack.enabled = !targetAsSSAA.ExtensionPostProcessingStack.enabled;
                        targetAsSSAA.enabled = true;
                    }
                }
                GUI.color = Color.white;
            }
            EditorGUILayout.EndHorizontal();
            if (extensionPostProcessingStack.FindPropertyRelative("inspectorFoldout").boolValue) {
                EditorGUILayout.BeginVertical(styleContentBox);
                if (extensionPostProcessingStackSupport) {
                    EditorGUILayout.HelpBox(SSAAExtensionPostProcessingStack2.description, MessageType.Info);
                    EditorGUILayout.PropertyField(extensionPostProcessingStack.FindPropertyRelative("updateFromOriginal"));
                    EditorGUILayout.PropertyField(extensionPostProcessingStack.FindPropertyRelative("lwrpLegacySupport"));

                }
                else EditorGUILayout.HelpBox(SSAAExtensionPostProcessingStack2.requirement, MessageType.Error);
                EditorGUILayout.EndVertical();
            }
            #endregion

            #region Extension Cinemachine
            bool extensionCinemachineSupport = targetAsSSAA.ExtensionCinemachine.IsSupported();
            if (extensionCinemachineSupport) GUI.color = extensionCinemachine.FindPropertyRelative("enabled").boolValue ? new Color(.5f, 1f, .5f) : new Color(.8f, .8f, .8f);
            else GUI.color = new Color(1f, .3f, .3f);
            EditorGUILayout.BeginHorizontal(styleHeader);
            {
                GUI.color = Color.white;
                EditorGUI.indentLevel++;
                extensionCinemachine.FindPropertyRelative("inspectorFoldout").boolValue = EditorGUILayout.Foldout(
                    extensionCinemachine.FindPropertyRelative("inspectorFoldout").boolValue,
                    "Unity Cinemachine Integration",
                    true,
                    styleFoldout);

                EditorGUI.indentLevel--;
                GUI.color = colorAccentPro;
                if (extensionCinemachineSupport) {
                    if (GUILayout.Button(extensionCinemachine.FindPropertyRelative("enabled").boolValue ? "Disable" : "Enable", GUILayout.MaxWidth(100), GUILayout.MinWidth(100))) {
                        targetAsSSAA.enabled = false;
                        targetAsSSAA.ExtensionCinemachine.enabled = !targetAsSSAA.ExtensionCinemachine.enabled;
                        if (targetAsSSAA.ExtensionCinemachine.enabled) {
                            SymbolDefineUtils.AddDefine("SSAA_CINEMACHINE");
                        }
                        else {
                            SymbolDefineUtils.RemoveDefine("SSAA_CINEMACHINE");
                        }
                        targetAsSSAA.enabled = true;
                    }
                }
                GUI.color = Color.white;
            }
            EditorGUILayout.EndHorizontal();
            if (extensionCinemachine.FindPropertyRelative("inspectorFoldout").boolValue) {
                EditorGUILayout.BeginVertical(styleContentBox);
                if (extensionCinemachineSupport) {
                    EditorGUILayout.HelpBox(SSAAExtensionCinemachine.description, MessageType.Info);
                    EditorGUILayout.PropertyField(extensionCinemachine.FindPropertyRelative("updateFromOriginal"));
                }
                else EditorGUILayout.HelpBox(SSAAExtensionCinemachine.requirement, MessageType.Error);
                EditorGUILayout.EndVertical();
            }
            #endregion

            // Draw Extensions
            EditorMacros.DrawHeader("Misc", styleHeader);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical(styleContentBox);
            // Handle render texture precision mode
            if (!Application.isPlaying) {
                currentMode = internalTextureFormat.enumValueIndex == (int)RenderTextureFormat.ARGBFloat ? TextureMode.Float : TextureMode.Half;
                var oldMode = currentMode;
                currentMode = (TextureMode)EditorGUILayout.EnumPopup(new GUIContent("Render Texture Precision"), currentMode);
                internalTextureFormat.enumValueIndex = currentMode == TextureMode.Half ? (int)RenderTextureFormat.ARGBHalf : (int)RenderTextureFormat.ARGBFloat;
                if (oldMode != currentMode) targetAsSSAA.Refresh();
            }
            EditorGUILayout.PropertyField(inspectorShowHelp);
            EditorGUILayout.Separator();
            EditorGUI.indentLevel--;
            if (GUILayout.Button("Open online documentation"))
                Application.OpenURL("https://github.com/MadGoat-Studio/MadGoat-SSAA-Resolution-Scale/wiki");

            EditorGUILayout.EndVertical();

            // Draw Social Media
            EditorMacros.DrawSocial(styleHeader, styleContentBox);
        }

        public virtual void DrawSSAA() {

            // Draw Hint Box
            if (inspectorShowHelp.boolValue) {
                EditorGUI.indentLevel--;
                EditorGUILayout.HelpBox("Conventional SSAA settings. Higher settings produces better quality at the cost of performance. x0.5 boost the performance, but reduces the resolution.", MessageType.Info);
                EditorGUI.indentLevel++;
            }

            // Draw SSAA Settings
            ssaaMode = Getmode();
            ssaaMode = EditorGUILayout.Popup("SSAA Mode", ssaaMode, ssaaModeStrings);
            targetAsSSAA.SetAsSSAA((SSAAMode)ssaaMode);
            if (ssaaMode > 1) DrawFSSAA();

            EditorGUILayout.Separator();
            EditorGUILayout.EndVertical();

            // Draw SSAA Presets Title
            EditorMacros.DrawHeader("Edit SSAA Presets", styleHeader);
            var unsupportedFilter = false;
#if SSAA_HDRP || SSAA_URP
            if (targetAsSSAA.GetType() == typeof(MadGoatSSAA_VR)) {
                EditorGUI.indentLevel--;
                EditorGUILayout.HelpBox("Downsampling Filtering on VR under Scriptable Render Pipelines is currently not supported", MessageType.Warning);
                EditorGUI.indentLevel++; 
                unsupportedFilter = true;
            }
#endif
            EditorGUILayout.Separator();



            // Draw Profiles
            GUI.color = new Color(.8f, .8f, .8f);
            EditorGUILayout.BeginHorizontal(styleHeader);
            {
                GUI.color = Color.white;

                var str = "Multiplier: <b>" + targetAsSSAA.SsaaProfileHalf.multiplier + "x</b>  -  Filter: <b>"
                   + (targetAsSSAA.SsaaProfileHalf.useFilter ? targetAsSSAA.SsaaProfileHalf.filterType.ToString().ToLower() : "off") + "</b>";
                if (unsupportedFilter) str = "";
                ssaaHalfUnfold = EditorGUILayout.Foldout(ssaaHalfUnfold, "SSAA x0.5", true, styleFoldout);
                styleRichLabel.alignment = TextAnchor.MiddleCenter;
                EditorGUILayout.LabelField(str, styleRichLabel, GUILayout.MaxWidth(2560));
                styleRichLabel.alignment = TextAnchor.MiddleLeft;
                GUI.color = colorAccentPro;
                if (GUILayout.Button("Reset", GUILayout.MaxWidth(100), GUILayout.MinWidth(100))) {
                    targetAsSSAA.SsaaProfileHalf = new SsaaProfile(.5f, true, Filter.POINT, 0.8f, .95f);
                }
                GUI.color = Color.white;
            }

            EditorGUILayout.EndHorizontal();

            if (ssaaHalfUnfold) {
                EditorGUILayout.BeginVertical(styleContentBox);
                EditorGUILayout.PropertyField(ssaaProfileHalf.FindPropertyRelative("multiplier"));
                if (!unsupportedFilter) {
                    EditorGUILayout.PropertyField(ssaaProfileHalf.FindPropertyRelative("useFilter"));
                    EditorGUI.indentLevel += 1;
                    if (ssaaProfileHalf.FindPropertyRelative("useFilter").boolValue) {
                        EditorGUILayout.PropertyField(ssaaProfileHalf.FindPropertyRelative("filterType"));
                        if (ssaaProfileHalf.FindPropertyRelative("filterType").enumValueIndex == 2)
                            EditorGUILayout.PropertyField(ssaaProfileHalf.FindPropertyRelative("sharpness"));
                        else if (ssaaProfileHalf.FindPropertyRelative("filterType").enumValueIndex == 1) {
                            EditorGUILayout.PropertyField(ssaaProfileHalf.FindPropertyRelative("sharpness"));
                            EditorGUILayout.PropertyField(ssaaProfileHalf.FindPropertyRelative("sampleDistance"));
                        }
                    }
                    EditorGUI.indentLevel -= 1;
                }
                EditorGUILayout.Separator();
                EditorGUILayout.EndVertical();
            }

            GUI.color = new Color(.8f, .8f, .8f);
            EditorGUILayout.BeginHorizontal(styleHeader);
            {
                GUI.color = Color.white;

                var str = "Multiplier: <b>" + targetAsSSAA.SsaaProfileX2.multiplier + "x</b>  -  Filter: <b>"
                   + (targetAsSSAA.SsaaProfileX2.useFilter ? targetAsSSAA.SsaaProfileX2.filterType.ToString().ToLower() : "off") + "</b>";
                if (unsupportedFilter) str = "";
                ssaaX2Unfold = EditorGUILayout.Foldout(ssaaX2Unfold, "SSAA x2", true, styleFoldout);
                styleRichLabel.alignment = TextAnchor.MiddleCenter;
                EditorGUILayout.LabelField(str, styleRichLabel, GUILayout.MaxWidth(2560));
                styleRichLabel.alignment = TextAnchor.MiddleLeft;
                GUI.color = colorAccentPro;
                if (GUILayout.Button("Reset", GUILayout.MaxWidth(100), GUILayout.MinWidth(100))) {
                    targetAsSSAA.SsaaProfileX2 = new SsaaProfile(1.4f, true, Filter.BICUBIC, 0.8f, .95f);
                }
                GUI.color = Color.white;
            }
            EditorGUILayout.EndHorizontal();
            if (ssaaX2Unfold) {
                EditorGUILayout.BeginVertical(styleContentBox);
                EditorGUILayout.PropertyField(ssaaProfileX2.FindPropertyRelative("multiplier"));
                if (!unsupportedFilter) {
                    EditorGUILayout.PropertyField(ssaaProfileX2.FindPropertyRelative("useFilter"));
                    EditorGUI.indentLevel += 1;
                    if (ssaaProfileX2.FindPropertyRelative("useFilter").boolValue) {
                        EditorGUILayout.PropertyField(ssaaProfileX2.FindPropertyRelative("filterType"));
                        if (ssaaProfileX2.FindPropertyRelative("filterType").enumValueIndex == 2)
                            EditorGUILayout.PropertyField(ssaaProfileX2.FindPropertyRelative("sharpness"));
                        else if (ssaaProfileX2.FindPropertyRelative("filterType").enumValueIndex == 1) {
                            EditorGUILayout.PropertyField(ssaaProfileX2.FindPropertyRelative("sharpness"));
                            EditorGUILayout.PropertyField(ssaaProfileX2.FindPropertyRelative("sampleDistance"));
                        }
                    }
                    EditorGUI.indentLevel -= 1;
                }
                EditorGUILayout.Separator();
                EditorGUILayout.EndVertical();
            }

            GUI.color = new Color(.8f, .8f, .8f);
            EditorGUILayout.BeginHorizontal(styleHeader);
            {
                GUI.color = Color.white;
                var str = "Multiplier: <b>" + targetAsSSAA.SsaaProfileX4.multiplier + "x</b>  -  Filter: <b>"
                    + (targetAsSSAA.SsaaProfileX4.useFilter ? targetAsSSAA.SsaaProfileX4.filterType.ToString().ToLower() : "off") + "</b>";
                if (unsupportedFilter) str = "";
                ssaaX4Unfold = EditorGUILayout.Foldout(ssaaX4Unfold, "SSAA x4", true, styleFoldout);
                styleRichLabel.alignment = TextAnchor.MiddleCenter;
                EditorGUILayout.LabelField(str, styleRichLabel, GUILayout.MaxWidth(2560));
                styleRichLabel.alignment = TextAnchor.MiddleLeft;
                GUI.color = colorAccentPro;
                if (GUILayout.Button("Reset", GUILayout.MaxWidth(100), GUILayout.MinWidth(100))) {
                    targetAsSSAA.SsaaProfileX4 = new SsaaProfile(2.0f, true, Filter.BICUBIC, 0.8f, .95f);
                }
                GUI.color = Color.white;
            }
            EditorGUILayout.EndHorizontal();
            if (ssaaX4Unfold) {
                EditorGUILayout.BeginVertical(styleContentBox);
                EditorGUILayout.PropertyField(ssaaProfileX4.FindPropertyRelative("multiplier"));
                if (!unsupportedFilter) {
                    EditorGUILayout.PropertyField(ssaaProfileX4.FindPropertyRelative("useFilter"));
                    EditorGUI.indentLevel += 1;
                    if (ssaaProfileX4.FindPropertyRelative("useFilter").boolValue) {
                        EditorGUILayout.PropertyField(ssaaProfileX4.FindPropertyRelative("filterType"));
                        if (ssaaProfileX4.FindPropertyRelative("filterType").enumValueIndex == 2)
                            EditorGUILayout.PropertyField(ssaaProfileX4.FindPropertyRelative("sharpness"));
                        else if (ssaaProfileX4.FindPropertyRelative("filterType").enumValueIndex == 1) {
                            EditorGUILayout.PropertyField(ssaaProfileX4.FindPropertyRelative("sharpness"));
                            EditorGUILayout.PropertyField(ssaaProfileX4.FindPropertyRelative("sampleDistance"));
                        }
                    }
                    EditorGUI.indentLevel -= 1;
                }
                EditorGUILayout.Separator();
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Separator();
        }
        public virtual void DrawResScale() {

            if (inspectorShowHelp.boolValue) {
                EditorGUI.indentLevel--;
                EditorGUILayout.HelpBox("Rise or lower the render resolution by percent", MessageType.Info);
                EditorGUI.indentLevel++;
            }
            internalRenderMultiplier.floatValue = EditorGUILayout.Slider("Resolution Scale (%)", internalRenderMultiplier.floatValue * 100f, 50, 200) / 100f;
            if (internalRenderMultiplier.floatValue > 1) {
                DrawFSSAA();
            }
            EditorGUILayout.Separator();
            EditorGUILayout.EndVertical();
        }
        public virtual void DrawPerAxis() {

            if (inspectorShowHelp.boolValue) {
                EditorGUI.indentLevel--;
                EditorGUILayout.HelpBox("Values over 4 not recommended, higher values (depending on current screen size) may cause system instability or engine crashes.", MessageType.Warning);
                EditorGUI.indentLevel++;
            }

            extendedMultiplier = EditorGUILayout.Toggle("Don't limit the multiplier", extendedMultiplier);
            if (extendedMultiplier) EditorGUILayout.PropertyField(internalRenderMultiplier, new GUIContent("Resolution X Multiplier"), true);
            else internalRenderMultiplier.floatValue = EditorGUILayout.Slider("Resolution X Multiplier", internalRenderMultiplier.floatValue, 0.2f, 4f);

            if (extendedMultiplier) EditorGUILayout.PropertyField(internalRenderMultiplierVertical, new GUIContent("Resolution Y Multiplier"), true);
            else internalRenderMultiplierVertical.floatValue = EditorGUILayout.Slider("Resolution Y Multiplier", internalRenderMultiplierVertical.floatValue, 0.2f, 4f);

            if (internalRenderMultiplier.floatValue <= 0.1f) internalRenderMultiplier.floatValue = 0.1f;
            if (internalRenderMultiplierVertical.floatValue <= 0.1f) internalRenderMultiplierVertical.floatValue = 0.1f;

            if (internalRenderMultiplier.floatValue > 1 || internalRenderMultiplierVertical.floatValue > 1) {
                DrawFSSAA();
            }
            EditorGUILayout.Separator();
            EditorGUILayout.EndVertical();
        }
        public virtual void DrawAdaptive() {
            if (inspectorShowHelp.boolValue) {
                EditorGUI.indentLevel--;
                EditorGUILayout.HelpBox("Adaptive mode allows for automatic resolution adjustment based on a target frame rate.", MessageType.Info);
                EditorGUI.indentLevel++;
            }
            EditorGUILayout.PropertyField(adaptiveResTargetVsync, new GUIContent("Refresh Rate is target"));
            EditorGUI.indentLevel++;
            if (!adaptiveResTargetVsync.boolValue)
                EditorGUILayout.PropertyField(adaptiveResTargetFps, new GUIContent("Frame Rate Target"));
            EditorGUI.indentLevel--;
            float min = adaptiveResMinMultiplier.floatValue * 100, max = adaptiveResMaxMultiplier.floatValue * 100;
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Resolution scale range");
            EditorGUILayout.MinMaxSlider(ref min, ref max, 50, 150);
            styleNormal.fontStyle = FontStyle.Bold;
            styleNormal.fontSize = 11;
            EditorGUILayout.BeginHorizontal();
            styleNormal.alignment = TextAnchor.UpperLeft;
            EditorGUILayout.LabelField("Minimum: " + (int)min + "%", styleNormal);
            styleNormal.alignment = TextAnchor.UpperCenter;
            EditorGUILayout.LabelField("Current: " + (int)(internalRenderMultiplier.floatValue * 100f) + "%", styleNormal, GUILayout.MaxWidth(-30));
            styleNormal.alignment = TextAnchor.UpperRight;
            EditorGUILayout.LabelField("Maximum: " + (int)max + "%", styleNormal);
            EditorGUILayout.EndHorizontal();
            styleNormal.alignment = TextAnchor.MiddleLeft;
            styleNormal.fontStyle = FontStyle.Normal;
            adaptiveResMinMultiplier.floatValue = min / 100;
            adaptiveResMaxMultiplier.floatValue = max / 100;
            EditorGUILayout.Separator();
            EditorGUILayout.EndVertical();
        }
        public virtual void DrawCustom() {
            if (inspectorShowHelp.boolValue) {
                EditorGUI.indentLevel--;
                EditorGUILayout.HelpBox("Values over 4 not recommended, higher values (depending on current screen size) may cause system instability or engine crashes.", MessageType.Warning);
                EditorGUI.indentLevel++;
            }

            extendedMultiplier = EditorGUILayout.Toggle("Don't limit the multiplier", extendedMultiplier);
            if (extendedMultiplier) EditorGUILayout.PropertyField(internalRenderMultiplier, new GUIContent("Resolution Multiplier"), true);
            else internalRenderMultiplier.floatValue = EditorGUILayout.Slider("Resolution Multiplier", internalRenderMultiplier.floatValue, 0.2f, 4f);

            if (internalRenderMultiplier.floatValue <= 0.1f) internalRenderMultiplier.floatValue = 0.1f;
            if (internalRenderMultiplierVertical.floatValue <= 0.1f) internalRenderMultiplierVertical.floatValue = 0.1f;

            if (internalRenderMultiplier.floatValue > 1) {
                DrawFSSAA();
            }
            EditorGUILayout.Separator();
            EditorGUILayout.EndVertical();
        }

        protected void DrawFSSAA() {
#if UNITY_2019_3_OR_NEWER && SSAA_HDRP && ENABLE_RAYTRACING
            EditorGUI.indentLevel--;
            EditorGUILayout.HelpBox("Post Process Anti Aliasing is not available when DXR is enabled", MessageType.Warning);
            EditorGUI.indentLevel++;
#else
            EditorGUILayout.PropertyField(postAntiAliasing, new GUIContent("Post Process AA"));
#endif
        }
    }
}