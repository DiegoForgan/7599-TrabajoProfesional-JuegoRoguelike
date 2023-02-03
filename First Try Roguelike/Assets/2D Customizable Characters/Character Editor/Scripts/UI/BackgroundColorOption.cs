using System;
using UnityEngine;

namespace CustomizableCharacters.CharacterEditor.UI
{
    [Serializable]
    public class BackgroundColorOption
    {
        [SerializeField] private string _colorName;
        [SerializeField] private Color _color;

        public string ColorName => _colorName;
        public Color Color => _color;
    }
}