using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MadGoat.Core.Utils;
namespace MadGoat.SSAA {
    [System.Serializable]
    public class SsaaScreenshotProfile {
       // [HideInInspector]
        public bool takeScreenshot = false;

        [Range(1, 4)]
        public int screenshotMultiplier = 1;
        public Vector2 outputResolution = new Vector2(1920, 1080);

        public bool downsamplerEnabled = true;
        public Filter downsamplerFilter;
        [Range(0, 1)]
        public float downsamplerSharpness = 0.85f;
        public float downsamplerDistance = 1f;

        public PostAntiAliasingMode postAntiAliasing = PostAntiAliasingMode.Off;
    }
}