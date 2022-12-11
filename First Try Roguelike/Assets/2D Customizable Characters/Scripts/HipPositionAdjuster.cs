using System;
using UnityEngine;

namespace CustomizableCharacters
{
    /// <summary>
    /// Used to make sure a character hip moves up and down when scale is changing. For example when legs get longer.
    /// </summary>
    [Serializable]
    public class HipPositionAdjuster
    {
        [SerializeField] private Transform _ground;
        [SerializeField] private Transform _hip;

        /// <summary>
        /// Adjusts the hip position based on scale change.
        /// </summary>
        /// <param name="fromYScale">Scale that was changed from.</param>
        /// <param name="toYScale">Scale that was changed to.</param>
        public void Adjust(float fromYScale, float toYScale)
        {
            if (_hip == null || _ground == null)
                return;

            var distanceToGround = (_hip.position.y - _ground.position.y);
            var distancePerUnit = distanceToGround / fromYScale;
            var scaleDifference = toYScale - fromYScale;
            var newDistanceDifference = distancePerUnit * scaleDifference;

            var position = _hip.transform.position;
            position.y += newDistanceDifference;
            _hip.transform.position = position;
        }
    }
}