using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if SSAA_CINEMACHINE
using Cinemachine;
#endif
#if UNITY_EDITOR && UNITY_2018_1_OR_NEWER
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
#endif
namespace MadGoat.SSAA {
    [System.Serializable]
    public class SSAAExtensionCinemachine : SSAAExtensionBase {
        public static string description = "Enables support for Unity's Cinemachine" +
            "\n   • updateFromOriginal - if enabled, the SSAA camera brain settings will " +
            "be synced with this cameras brain (if available)";
        public static string requirement = "- Requires Cinemachine package" +
            "\n- Requires Unity 2018.1 or newer";

        private bool cinemachineInstalled = false;
#if UNITY_EDITOR && UNITY_2018_1_OR_NEWER
        // from https://docs.unity3d.com/Manual/upm-api.html
        private ListRequest Request;
        private void CheckPackage() {
            Request = Client.List();    // List packages installed for the Project
            EditorApplication.update += CheckPackageProgress;
        }
        private void CheckPackageProgress() {
            if (Request.IsCompleted) {
                cinemachineInstalled = false;
                if (Request.Status == StatusCode.Success)
                    foreach (var package in Request.Result)
                        if (package.name == "com.unity.cinemachine") {
                            cinemachineInstalled = true;
                            break;
                        }
                EditorApplication.update -= CheckPackageProgress;
            }
        }
#endif
        public override bool IsSupported() {
            return cinemachineInstalled;
        }

        public bool updateFromOriginal = true;

#if SSAA_CINEMACHINE
        private CinemachineBrain cinemachineCurrent;
        private CinemachineBrain cinemachineRender;
#endif
        public override void OnInitialize(MadGoatSSAA ssaaInstance) {
#if UNITY_EDITOR && UNITY_2018_1_OR_NEWER
            CheckPackage();
#endif
            base.OnInitialize(ssaaInstance);
            if (!enabled) return;

#if SSAA_CINEMACHINE
            cinemachineCurrent = ssaaInstance.CurrentCamera.GetComponent<CinemachineBrain>();
            cinemachineRender = ssaaInstance.RenderCamera.GetComponent<CinemachineBrain>();
            if (cinemachineRender == null) cinemachineRender = ssaaInstance.RenderCamera.gameObject.AddComponent<CinemachineBrain>();

            // Handle enabled states
            if (cinemachineCurrent && cinemachineCurrent.enabled)  cinemachineCurrent.enabled = false;
            cinemachineRender.enabled = true; 
#endif
        }
        public override void OnUpdate(MadGoatSSAA ssaaInstance) {
            if (!enabled) return;

#if SSAA_CINEMACHINE
            if (cinemachineCurrent && cinemachineCurrent.enabled) cinemachineCurrent.enabled = false;

            if (updateFromOriginal && cinemachineCurrent) {
                cinemachineRender.m_CameraActivatedEvent = cinemachineCurrent.m_CameraActivatedEvent;
                cinemachineRender.m_CameraCutEvent = cinemachineCurrent.m_CameraCutEvent;
                cinemachineRender.m_CustomBlends = cinemachineCurrent.m_CustomBlends;
                cinemachineRender.m_DefaultBlend = cinemachineCurrent.m_DefaultBlend;

                cinemachineRender.m_IgnoreTimeScale = cinemachineCurrent.m_IgnoreTimeScale;
                cinemachineRender.m_ShowCameraFrustum = cinemachineCurrent.m_ShowCameraFrustum;
                cinemachineRender.m_ShowDebugText = cinemachineCurrent.m_ShowDebugText;
                cinemachineRender.m_UpdateMethod = cinemachineCurrent.m_UpdateMethod;
                cinemachineRender.m_WorldUpOverride = cinemachineCurrent.m_WorldUpOverride;
            }
#endif
        }
        public override void OnDeinitialize(MadGoatSSAA ssaaInstance) {
            base.OnDeinitialize(ssaaInstance);
            if (!enabled) return;
#if SSAA_CINEMACHINE
            // Handle enabled states
            cinemachineRender.enabled = false;
            if (cinemachineCurrent) cinemachineCurrent.enabled = true;
#endif
        }
    }
}