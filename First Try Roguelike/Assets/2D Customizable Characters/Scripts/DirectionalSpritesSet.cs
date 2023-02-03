using System;
using UnityEngine;

namespace CustomizableCharacters
{
    /// <summary>
    /// Sprites for each character direction.
    /// </summary>
    [Serializable]
    public class DirectionalSpritesSet
    {
        [SerializeField] private Sprite _downSprite;
        [SerializeField] private Sprite _sideSprite;
        [SerializeField] private Sprite _upSprite;

        public Sprite DownSprite => _downSprite;
        public Sprite SideSprite => _sideSprite;
        public Sprite UpSprite => _upSprite;

        public bool HasSprites => _downSprite != null || _sideSprite != null || _upSprite != null;
    }
}