using System;
using UnityEngine;

namespace CustomizableCharacters
{
    /// <summary>
    /// Contains body, customization and scale values.
    /// </summary>
    [CreateAssetMenu(menuName = "2D Customizable Characters/Character Preset", fileName = "CharacterPreset")]
    public class CharacterPreset : ScriptableObject
    {
        [SerializeField] private Color _bodyColor;
        [SerializeField] private Customization[] _customizations;
        [SerializeField] private ScaleGroupPreset[] _scaleGroups = Array.Empty<ScaleGroupPreset>();

        public Color BodyColor => _bodyColor;
        public Customization[] Customizations => _customizations;
        public ScaleGroupPreset[] ScaleGroups => _scaleGroups;

        public void SetBodyColor(Color color) => _bodyColor = color;
        public void SetCustomizations(Customization[] customizations) => _customizations = customizations;
        public void SetScaleGroups(ScaleGroupPreset[] scaleGroups) => _scaleGroups = scaleGroups;
    }
}