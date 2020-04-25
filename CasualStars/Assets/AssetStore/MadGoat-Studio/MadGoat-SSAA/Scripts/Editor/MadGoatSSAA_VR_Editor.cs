using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MadGoat.Core.EditorUtils;

namespace MadGoat.SSAA {
    [CustomEditor(typeof(MadGoatSSAA_VR))]
    public class MadGoatSSAA_VR_Editor : MadGoatSSAA_Editor {
        public override void DrawSectionScreenshot() {
            EditorGUILayout.HelpBox("Screenshot functionality is not available in VR mode.", MessageType.Error);
        }

        public override void DrawSectionMisc() // General Tab 
        {
            var subCat = new GUIStyle(UnityEditor.EditorStyles.helpBox);
            subCat.padding = new RectOffset(0, 0, 10, 5);
            subCat.margin = new RectOffset(0, 0, 0, 10);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical(subCat);
            // Handle render texture precision mode
            EditorGUILayout.PropertyField(inspectorShowHelp);
            EditorGUILayout.Separator();
            EditorGUI.indentLevel--;

            if (GUILayout.Button("Open online documentation"))
                Application.OpenURL("https://docs.google.com/document/d/1lhcYJwkueuVuHZnYdQVg2EqUb980VlH80_J9mvOwuPw/");

            EditorGUILayout.EndVertical();

            // Draw Social Media
            EditorMacros.DrawSocial(styleHeader, styleContentBox);
        }
        public override void DrawPerAxis() {
            EditorGUILayout.HelpBox("NOT SUPPORTED IN VR MODE.\nX axis will be used as global multiplier instead.", MessageType.Error);

            base.DrawPerAxis();
        }
    }
}
