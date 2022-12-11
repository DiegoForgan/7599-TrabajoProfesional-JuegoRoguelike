using System;
using PlaymodeColorPicker;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CustomizableCharacters.CharacterEditor
{
    [Serializable]
    public class CharacterEditorCustomization
    {
        [HideInInspector] public string DisplayName;

        [Tooltip("Icon of this customization. If none is set the UI will pick the first down sprite of the SpriteSets.")]
        [SerializeField] private Sprite _icon;
        [SerializeField] private CustomizationData _data;

        [Tooltip("Colors that will be suggested for the main color by the color picker and also used for randomization")]
        [SerializeField] private ColorGroup[] _suggestedMainColorGroups;

        [Tooltip("Colors that will be suggested for the detail color by the color picker and also used for randomization")]
        [SerializeField] private ColorGroup[] _suggestedDetailColorGroups;

        public ColorGroup[] SuggestedMainColorGroups => _suggestedMainColorGroups;
        public ColorGroup[] SuggestedDetailColorGroups => _suggestedDetailColorGroups;

        public Sprite Icon => _icon;
        public CustomizationData Data => _data;

        public bool HasSuggestedMainColors() => _suggestedMainColorGroups.Length > 0;
        public bool HasSuggestedDetailColors() => _suggestedDetailColorGroups.Length > 0;
        public Color GetRandomMainColor() => GetRandomColor(_suggestedMainColorGroups);
        public Color GetRandomDetailColor() => GetRandomColor(_suggestedDetailColorGroups);

        private Color GetRandomColor(ColorGroup[] groups)
        {
            var randomIndex = Random.Range(0, groups.Length);
            var randomGroup = groups[randomIndex];
            var randomColor = randomGroup.GetRandomColor();
            return randomColor;
        }
    }
}