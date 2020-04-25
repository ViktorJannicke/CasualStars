#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MadGoat.Core.EditorUtils {
    public static class EditorStyles {

        public static GUIStyle NewTitleStyle(Color accentColor) {
            var style = new GUIStyle("ShurikenModuleTitle");
            style.normal.textColor = accentColor;
            style.fontSize = 16;
            style.fontStyle = FontStyle.Bold;
            style.fixedHeight = 40;
            return style;
        }

        public static GUIStyle NewHeaderStyle(Color accentColor) {
            var style = new GUIStyle("ShurikenModuleTitle");
            style.normal.textColor = accentColor;
            style.fontSize = 12;
            style.fontStyle = FontStyle.Bold;
            style.fixedHeight = 30;
            return style;
        }

        public static GUIStyle NewFooterStyle(Color accentColor) {
            var style = new GUIStyle();
            style.fontSize = 12;
            style.richText = true;
            style.normal.textColor = accentColor;
            return style;
        }

        public static GUIStyle NewContentBoxStyle() {
            var style = new GUIStyle(UnityEditor.EditorStyles.helpBox);
            style.padding = new RectOffset(0, 0, 10, 4);
            style.margin = new RectOffset(0, 0, 0, 10);
            return style;
        }

        public static GUIStyle NewRichLabelStyle() {
            var style = new GUIStyle(UnityEditor.EditorStyles.label);
            style.richText = true;
            return style;
        }

        public static GUIStyle NewFoldoutStyle() {
            var style = new GUIStyle(UnityEditor.EditorStyles.foldout);
            style.fontSize = 12;
#if UNITY_2019_3_OR_NEWER
            style.margin = new RectOffset(5, -30, 2, 0);
            style.padding = new RectOffset(15, 0, 0, 0);
#else
            style.margin = new RectOffset(5, 0, 4, 0);
#endif
            return style;
        }
    }
}
#endif