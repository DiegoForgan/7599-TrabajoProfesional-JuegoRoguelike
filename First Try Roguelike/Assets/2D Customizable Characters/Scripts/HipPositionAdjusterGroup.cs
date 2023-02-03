using System;
using UnityEngine;

namespace CustomizableCharacters
{
    /// <summary>
    /// HipPositionAdjusters for each character direction and the name of the scale group that should affect the hip position.
    /// </summary>
    [Serializable]
    public class HipPositionAdjusterGroup
    {
        [SerializeField] private string _scaleGroupName;

        [SerializeField] private HipPositionAdjuster _downAdjuster;
        [SerializeField] private HipPositionAdjuster _sideAdjuster;
        [SerializeField] private HipPositionAdjuster _upAdjuster;

        public string ScaleGroupName => _scaleGroupName;
        public HipPositionAdjuster DownAdjuster => _downAdjuster;
        public HipPositionAdjuster SideAdjuster => _sideAdjuster;
        public HipPositionAdjuster UpAdjuster => _upAdjuster;
    }
}