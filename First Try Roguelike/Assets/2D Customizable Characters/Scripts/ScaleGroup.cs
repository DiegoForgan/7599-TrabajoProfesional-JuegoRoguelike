using System;
using UnityEngine;

namespace CustomizableCharacters
{
    /// <summary>
    /// Values for total scale, width and length. 
    /// </summary>
    [Serializable]
    public class ScaleGroup
    {
        [SerializeField] private string _groupName = "group name";
        [SerializeField] private float _scale = 1;
        [SerializeField] private float _width = 1;
        [SerializeField] private float _length = 1;

        [SerializeField] private Transform[] _transforms = Array.Empty<Transform>();
     
        public string GroupName => _groupName;
        public float Scale => _scale;
        public float Width => _width;
        public float Length => _length;

        public Transform[] Transforms => _transforms;

        public void SetScale(float value) => _scale = value;
        public void SetWidth(float value) => _width = value;
        public void SetLength(float value) => _length = value;

        public void Reset()
        {
            _scale = 1;
            _width = 1;
            _length = 1;
        }
    }
}