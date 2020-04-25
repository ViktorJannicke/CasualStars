using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MadGoat.SSAA {
    [System.Serializable]
    public class SSAAExtensionBase {
        [SerializeField, HideInInspector]
        public bool inspectorFoldout = false;
        [SerializeField, HideInInspector]
        public bool enabled = false;

        public virtual bool IsSupported() {
            return true;
        }
        public virtual void OnInitialize(MadGoatSSAA ssaaInstance) {

        }
        public virtual void OnUpdate(MadGoatSSAA ssaaInstance) {

        }
        public virtual void OnDeinitialize(MadGoatSSAA ssaaInstance) {

        }
    }
}