using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlaymodeColorPicker
{
    public class UIColorGroup : MonoBehaviour
    {
        [SerializeField] private Text _groupLabel;
        [SerializeField] private Button _colorSwatchButton;
        [SerializeField] private Transform _buttonsParent;

        private readonly List<Button> _colorSwatchButtons = new List<Button>();
        private Color[] _colors;

        public event Action<Color> SwatchPicked;

        private void Awake()
        {
            _colorSwatchButton.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            DestroyColorSwatches();
        }

        public void Bind(string groupName, Color[] colors)
        {
            _groupLabel.text = groupName;
            _colors = colors;
            CreateColorSwatches();
        }

        private void CreateColorSwatches()
        {
            for (int i = 0; i < _colors.Length; i++)
            {
                var button = Instantiate(_colorSwatchButton, _buttonsParent);
                button.gameObject.SetActive(true);
                button.GetComponent<Image>().color = _colors[i];
                button.onClick.AddListener(() => OnColorOptionClicked(button.GetComponent<Image>().color));
                _colorSwatchButtons.Add(button);
            }
        }

        private void DestroyColorSwatches()
        {
            for (int i = _colorSwatchButtons.Count - 1; i >= 0; i--)
            {
                var button = _colorSwatchButtons[i];
                Destroy(button.gameObject);
            }

            _colorSwatchButtons.Clear();
        }

        private void OnColorOptionClicked(Color color) => SwatchPicked?.Invoke(color);
    }
}