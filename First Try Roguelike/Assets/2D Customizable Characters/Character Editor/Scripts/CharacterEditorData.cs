using System;
using System.Collections.Generic;
using PlaymodeColorPicker;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CustomizableCharacters.CharacterEditor
{
    [CreateAssetMenu(menuName = "2D Customizable Characters/Customization Editor Data",
        fileName = "CustomizationEditorData", order = 0)]
    public class CharacterEditorData : ScriptableObject
    {
        [SerializeField] private List<CustomizableCharacter> _customizableCharacters = new List<CustomizableCharacter>();
        [SerializeField] private ColorGroup[] _randomBodyColorGroups;
        [SerializeField] private CustomizationSet[] _customizationSets = Array.Empty<CustomizationSet>();
        [SerializeField] private CharacterPreset[] _characterPresets = Array.Empty<CharacterPreset>();
        [SerializeField] private CharacterEditorCustomization[] _appearances = Array.Empty<CharacterEditorCustomization>();
        [SerializeField] private CharacterEditorCustomization[] _equipment = Array.Empty<CharacterEditorCustomization>();
        [Tooltip("Clips that will reset all values. Put clips in the order of: Down, Side, Up")]
        [SerializeField] private AnimationClipGroup _defaultPoseClipGroup;
        [SerializeField] private AnimationClipGroup[] _animationClipGroups = Array.Empty<AnimationClipGroup>();

        public List<CustomizableCharacter> CustomizableCharacters => _customizableCharacters;
        public ColorGroup[] RandomBodyColorGroups => _randomBodyColorGroups;
        public CustomizationSet[] CustomizationSets => _customizationSets;
        public CharacterPreset[] CharacterPresets => _characterPresets;
        public CharacterEditorCustomization[] Appearances => _appearances;
        public CharacterEditorCustomization[] Equipment => _equipment;
        public AnimationClipGroup DefaultPoseClipGroup => _defaultPoseClipGroup;
        public AnimationClipGroup[] AnimationClipGroups => _animationClipGroups;

        public bool HasRandomBodyColors() => _randomBodyColorGroups.Length > 0;

        public Color GetRandomBodyColor()
        {
            var randomIndex = Random.Range(0, _randomBodyColorGroups.Length);
            var randomGroup = _randomBodyColorGroups[randomIndex];
            var randomColor = randomGroup.GetRandomColor();
            return randomColor;
        }

        public void AddCustomizableCharacter(CustomizableCharacter character)
        {
            _customizableCharacters.Add(character);
        }

        public void RemoveCustomizableCharacter(CustomizableCharacter character)
        {
            _customizableCharacters.Remove(character);
        }

        public void CleanupNullReferences()
        {
            for (int i = _customizableCharacters.Count - 1; i >= 0; i--)
            {
                if (_customizableCharacters[i] == null)
                    _customizableCharacters.RemoveAt(i);
            }
        }
    }
}