using System;
using UnityEngine;

namespace CustomizableCharacters
{
    /// <summary>
    /// Same as DirectionalSpritesSet, but also contains the detail sprite.
    /// </summary>
    [Serializable]
    public class CustomizationSpriteSet : DirectionalSpritesSet
    {
        [SerializeField] private CustomizationLocation _customizationLocation;

        [Space]
        [SerializeField] private DirectionalSpritesSet[] _detailSpriteSets = Array.Empty<DirectionalSpritesSet>();
        [Tooltip(
            "A location that this sprite set will be drawn with a higher sorting order. For example if a hat should cover the ears.")]
        [SerializeField] private CustomizationLocation[] _sortOverLocations = Array.Empty<CustomizationLocation>();

        /// <summary>
        /// The location that should be showing this customization. 
        /// </summary>
        public CustomizationLocation CustomizationLocation => _customizationLocation;
        /// <summary>
        /// The sprites that will be used as details.
        /// </summary>
        public DirectionalSpritesSet[] DetailSpriteSets => _detailSpriteSets;
        /// <summary>
        /// A location that this sprite set will be drawn with a higher sorting order. For example if a hat should cover the ears.
        /// </summary>
        public CustomizationLocation[] SortOverLocations => _sortOverLocations;

        /// <summary>
        /// Whether there is any detail sprites or not.
        /// </summary>
        public bool HasDetailSprites => _detailSpriteSets.Length > 0;
        
        /// <summary>
        /// The amount of detail sprites.
        /// </summary>
        public int AmountOfDetailSpritesSets => _detailSpriteSets.Length;
    }
}