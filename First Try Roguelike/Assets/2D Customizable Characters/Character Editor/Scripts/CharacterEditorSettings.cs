using System;
using UnityEngine;

namespace CustomizableCharacters.CharacterEditor
{
    [CreateAssetMenu(menuName = "2D Customizable Characters/Character Editor Settings",
        fileName = "CharacterEditorSettings", order = 0)]
    public class CharacterEditorSettings : ScriptableObject
    {
        [Tooltip("CharacterEditorData that will be loaded and available")]
        [SerializeField] private CharacterEditorData[] _characterEditorDatas = Array.Empty<CharacterEditorData>();
        [Tooltip("How much the scale is allowed to deviate from 1. This sets the min and max value of the scaling sliders.")]
        [SerializeField] private float _scaleDeviation = 0.2f;
        public CharacterEditorData[] CharacterEditorDatas => _characterEditorDatas;
        public float ScaleDeviation => _scaleDeviation;
    }
}