using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MadGoat.Core.Utils
{
    public class SsaaFramerateSampler
    {
        #region Fields
        private float newPeriod = 0;
        private int intervalTotalFrames = 0;
        private int intervalFrameSum = 0;
        #endregion

        #region Properties
        public int CurrentFps { get; private set; }
        public float UpdateInterval { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a framerate sampler with a default update interval of 1 second
        /// </summary>
        public SsaaFramerateSampler() {
            this.CurrentFps = 0;
            this.UpdateInterval = 1;
        }
        /// <summary>
        /// Creates a framerate sampler with a given update interval
        /// </summary>
        /// <param name="updateInterval"></param>
        public SsaaFramerateSampler(float updateInterval) {
            this.CurrentFps = 0;
            this.UpdateInterval = updateInterval;
        }
        /// <summary>
        /// Update method for the framerate sampler. Has to be called on MonoBehaviour Update for the framerate to be accurate
        /// </summary>
        public void Update()
        {
            intervalTotalFrames++;
            intervalFrameSum += (int)(1f / Time.deltaTime);
            if (Time.time > newPeriod)
            {
                CurrentFps = intervalFrameSum / intervalTotalFrames;
                intervalTotalFrames = 0;
                intervalFrameSum = 0;

                newPeriod += UpdateInterval;
            }
        }
        #endregion
    }
}