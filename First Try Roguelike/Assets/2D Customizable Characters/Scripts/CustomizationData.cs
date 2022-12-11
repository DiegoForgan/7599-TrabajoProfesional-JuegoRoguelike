using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomizableCharacters
{
    /// <summary>
    /// Data about a customization. This is the main object used for customizations.
    /// </summary>
    [CreateAssetMenu(menuName = "2D Customizable Characters/Customization Data", fileName = "CustomizationData")]
    public class CustomizationData : ScriptableObject
    {
        [SerializeField] private CustomizationCategory _category;
        [Tooltip("Sprites for each direction of a location.")]
        [SerializeField] private CustomizationSpriteSet[] _spriteSets = Array.Empty<CustomizationSpriteSet>();
        [Tooltip("Makes any customization on these locations hide when this customization is added.")]
        [SerializeField] private CustomizationLocation[] _locationsToHide = Array.Empty<CustomizationLocation>();

        [Space]
        [Tooltip("The priority when two customizations is using the same slot.")]
        [SerializeField] private int _slotPriority = 0;
        [Tooltip(
            "Should customization be allowed to change color. Usually true when sprite is created in/close to grayscale colors.")]
        [SerializeField] private bool _canBeTinted = true;
        [Tooltip(
            "Should customization details be allowed to change color. Usually true when sprite is created in/close to grayscale colors.")]
        [SerializeField] private bool _canDetailsBeTinted = true;

        [Tooltip("Should the main color also be used for tinting the detail sprite. (this will ignore the detail color")]
        [SerializeField] private bool _useMainColorForDetail;
        public CustomizationCategory Category => _category;
        public CustomizationSpriteSet[] SpriteSets => _spriteSets;
        public int SlotPriority => _slotPriority;

        /// <summary>
        /// Customizations in locations that should be hidden when customization is added to character.
        /// </summary>
        public CustomizationLocation[] LocationsToHide => _locationsToHide;

        /// <summary>
        /// Whether the customization should be able to change color or not.
        /// </summary>
        public bool CanBeTinted => _canBeTinted;
        /// <summary>
        /// Whether the customization detail should be able to change color or not.
        /// </summary>
        public bool CanDetailsBeTinted => _canDetailsBeTinted;
        /// <summary>
        /// Whether the detail should use the main color.
        /// </summary>
        public bool UseMainColorForDetail => _useMainColorForDetail;

        /// <summary>
        /// Returns all the locations in this customization.
        /// </summary>
        /// <returns></returns>
        public List<CustomizationLocation> GetLocations()
        {
            var partTypes = new List<CustomizationLocation>();
            for (int i = 0; i < _spriteSets.Length; i++)
            {
                partTypes.Add(_spriteSets[i].CustomizationLocation);
            }

            return partTypes;
        }

        /// <summary>
        /// Returns whether the customization has any detail sprites or not.
        /// </summary>
        /// <returns></returns>
        public bool HasDetailSprites()
        {
            for (int i = 0; i < SpriteSets.Length; i++)
            {
                var element = SpriteSets[i];
                if (element.HasDetailSprites)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns whether any sprite is references at index or not.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool HasDetailSpriteAtIndex(int index)
        {
            for (int i = 0; i < _spriteSets.Length; i++)
            {
                var spriteSet = _spriteSets[i];
                if (spriteSet.DetailSpriteSets != null && spriteSet.DetailSpriteSets.Length >= index + 1)
                {
                    if (spriteSet.DetailSpriteSets[index].DownSprite != null
                        || spriteSet.DetailSpriteSets[index].SideSprite != null
                        || spriteSet.DetailSpriteSets[index].UpSprite != null)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the amount of detail sprites the customization has. Note that this is not the total of all spritesets combined, but the largest one found.
        /// </summary>
        /// <returns></returns>
        public int GetDetailSpriteCount()
        {
            var highestCount = 0;
            for (int i = 0; i < _spriteSets.Length; i++)
            {
                var count = _spriteSets[i].DetailSpriteSets.Length;
                if (count > highestCount)
                    highestCount = count;
            }

            return highestCount;
        }

        /// <summary>
        /// Whether the customization has a sprite in any of the SpriteSets or DetailSpriteSets.
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        public bool ContainSprite(Sprite sprite)
        {
            for (int i = 0; i < SpriteSets.Length; i++)
            {
                var spriteSet = SpriteSets[i];

                if (spriteSet.DownSprite == sprite
                    || spriteSet.SideSprite == sprite
                    || spriteSet.UpSprite == sprite)
                    return true;

                if (spriteSet.HasDetailSprites)
                    continue;

                for (int j = 0; j < spriteSet.DetailSpriteSets.Length; j++)
                {
                    var detailSpriteSet = spriteSet.DetailSpriteSets[j];

                    if (detailSpriteSet.DownSprite == sprite
                        || detailSpriteSet.SideSprite == sprite
                        || detailSpriteSet.UpSprite == sprite)
                        return true;
                }
            }

            return false;
        }
    }
}