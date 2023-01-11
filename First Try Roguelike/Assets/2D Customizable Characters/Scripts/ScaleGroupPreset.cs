using System;
using UnityEngine;

namespace CustomizableCharacters
{
    /// <summary>
    /// Contains scale values for a scale group.
    /// </summary>
    [Serializable]
    public class ScaleGroupPreset
    {
        public ScaleGroupPreset(string groupName, float scale, float width, float length)
        {
            _groupName = groupName;
            _scale = scale;
            _width = width;
            _length = length;
        }

        [SerializeField] private string _groupName;
        [SerializeField] private float _scale;
        [SerializeField] private float _width;
        [SerializeField] private float _length;

        public string GroupName => _groupName;
        public float Scale => _scale;
        public float Width => _width;
        public float Length => _length;
    }
}