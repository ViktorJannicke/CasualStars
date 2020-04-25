using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MadGoat.SSAA {
    public enum RenderMode {
        SSAA,
        ResolutionScale,
        PerAxisScale,
        AdaptiveResolution,
        Custom
    }
    public enum SSAAMode {
        SSAA_OFF = 0,
        SSAA_HALF = 1,
        SSAA_X2 = 2,
        SSAA_X4 = 3
    }
    public enum Filter {
        POINT,
        BILINEAR,
        BICUBIC
    }
    public enum ImageFormat {
        JPG,
        PNG,
#if UNITY_5_6_OR_NEWER
        EXR
#endif
    }
    public enum PostAntiAliasingMode {
        Off,
#if SSAA_HDRP
        FSSAA,
        TSSAA
#else
        FSSAA
#endif
    }
    public static class SsaaUtils {
        // Don't forget to change when pushing updates!
        public const string ssaaversion = "2.0.7";

        /// <summary>
        /// Makes this camera's settings match the other camera and assigns a custom target texture
        /// </summary>
        public static void CopyFrom(this Camera current, Camera other, RenderTexture rt) {
            current.CopyFrom(other);
            current.targetTexture = rt;
        }
    }
}