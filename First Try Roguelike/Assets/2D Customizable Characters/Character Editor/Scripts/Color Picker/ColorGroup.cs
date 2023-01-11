using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlaymodeColorPicker
{
    [CreateAssetMenu(menuName = "2D Customizable Characters/Color Group", fileName = "ColorGroup", order = 0)]
    public class ColorGroup : ScriptableObject
    {
        [SerializeField] private Color[] _colors = Array.Empty<Color>();

        public string GroupName => this.name;
        public Color[] Colors => _colors;

        public Color GetRandomColor()
        {
            var randomIndex = Random.Range(0, _colors.Length);
            var randomColor = _colors[randomIndex];
            return randomColor;
        }
    }
}