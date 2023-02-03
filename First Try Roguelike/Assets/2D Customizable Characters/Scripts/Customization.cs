using System;
using UnityEngine;

namespace CustomizableCharacters
{
    /// <summary>
    /// A customization created from a CustomizationData.
    /// </summary>
    [Serializable]
    public class Customization
    {
        public Customization(CustomizationData customizationData)
        {
            _customizationData = customizationData;
        }

        public Customization(CustomizationData customizationData, Color mainColor, Color detailColor, int detailSpritesIndex)
        {
            _customizationData = customizationData;
            _mainColor = mainColor;
            _detailSpritesIndex = detailSpritesIndex;
            _detailColor = detailColor;
        }

        [SerializeField] private CustomizationData _customizationData;
        [SerializeField] private Color _mainColor = Color.white;
        [SerializeField] private int _detailSpritesIndex;
        [SerializeField] private Color _detailColor = Color.white;
        [SerializeField] private bool _isHidden;

        public CustomizationData CustomizationData => _customizationData;
        public int DetailSpritesIndex => _detailSpritesIndex;
        public Color MainColor => _mainColor;
        public Color DetailColor => _detailColor;

        /// <summary>
        /// If customization is hidden on the character.
        /// </summary>
        public bool IsHidden => _isHidden;

        /// <summary>
        /// Sets if the customization should be hidden or not. 
        /// </summary>
        /// <param name="hidden"></param>
        public void SetIsHidden(bool hidden) => _isHidden = hidden;

        /// <summary>
        /// Sets the sprite detail index of the customization.
        /// </summary>
        /// <param name="index"></param>
        public void SetDetailSpriteIndex(int index) => _detailSpritesIndex = index;

        /// <summary>
        /// Sets the main color of the customization.
        /// </summary>
        /// <param name="color"></param>
        public void SetMainColor(Color color) => _mainColor = color;

        /// <summary>
        /// Sets the detail color of the customization.
        /// </summary>
        /// <param name="color"></param>
        public void SetDetailColor(Color color) => _detailColor = color;
    }
}