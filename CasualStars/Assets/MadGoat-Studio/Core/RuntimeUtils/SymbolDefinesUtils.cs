using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MadGoat.Core.Utils {
    public static class SymbolDefineUtils {
        public static void AddDefine(string define) {
#if UNITY_EDITOR
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var targetPlatform = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetPlatform);

            // no need to add something that already exists
            if (defines.Contains(define)) return;
            else {
                defines += ";" + define;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetPlatform, defines);
            }
#endif
        }
        public static void RemoveDefine(string define) {
#if UNITY_EDITOR
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var targetPlatform = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetPlatform);

            // no need to add something that already exists
            if (defines.Contains(";" + define)) {
                defines = defines.Replace(";" + define, "");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetPlatform, defines);
            }
            else if (defines.Contains(define + ";")) {
                defines = defines.Replace(";" + define, "");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetPlatform, defines);
            }
#endif
        }
    }
}