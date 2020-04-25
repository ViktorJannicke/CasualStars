using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MadGoat.SSAA {
    [System.Serializable]
    public class SSAAExtensionPointerEventsSupport : SSAAExtensionBase {
        public static string description = "Enables support for Unity's built in pointer events. Select the layers to be affected below. " +
            "\n(Note: selecting everything or default can cause performance issues)";
        public LayerMask eventsLayerMask;
        public override void OnUpdate(MadGoatSSAA ssaaInstance) {
            if (!enabled) return;
            base.OnUpdate(ssaaInstance);
            var currentRenderMask = ssaaInstance.CurrentCamera.cullingMask;
            currentRenderMask |= eventsLayerMask.value;
            ssaaInstance.CurrentCamera.cullingMask = currentRenderMask; 
        }
    }
}