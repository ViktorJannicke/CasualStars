using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MadGoat.Core.DemoUtils {
    public class SimpleCameraRotate : MonoBehaviour {
        #region Fields
        public float speed = 10f;
        public bool showGUI = true;
        #endregion

        #region MonoBehaviour Interface Implementation
        void Update() {
            transform.Rotate(new Vector3(0, speed * Time.deltaTime, 0));
        }
        void OnGUI() {
            if (!showGUI) return;
            GUI.Label(new Rect(20, Screen.height - 70, 100, 100), "Camera speed");
            speed = -GUI.HorizontalSlider(new Rect(20, Screen.height - 50, 100, 20), -speed, 0, 50);
        }
        #endregion
    }
}
