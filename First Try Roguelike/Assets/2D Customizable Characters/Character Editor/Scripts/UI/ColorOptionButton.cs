using System;
using PlaymodeColorPicker;
using UnityEngine;
using UnityEngine.UI;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class ColorOptionButton : MonoBehaviour
    {
        [SerializeField] private ColorPicker _colorPicker;
        [SerializeField] private Button _button;
        [SerializeField] private Image _currentColorImage;
        [SerializeField] private Vector2 _pickerPivot;
        [SerializeField] private GameObject _enabledGameObject;
        [SerializeField] private GameObject _disabledGameObject;

        private ColorGroup[] _swatches;
        private Color _currentColor;
        public Color CurrentColor => _currentColor;
        public event Action<Color> ColorPickerChangedColor;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OpenColorPicker);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OpenColorPicker);
        }

        public void BindSwatches(ColorGroup[] colorGroups) => _swatches = colorGroups;

        public void SetEnabled(bool isEnabled)
        {
            _enabledGameObject.SetActive(isEnabled);
            _disabledGameObject.SetActive(!isEnabled);
            _button.interactable = isEnabled;
        }

        public void SetColor(Color color)
        {
            _currentColor = color;
            UpdateColor();
        }

        private void OpenColorPicker()
        {
            _colorPicker.onColorChanged += OnColorPickerChangedColor;
            _colorPicker.Closed += OnColorPickerClosed;
            _colorPicker.SetColorSwatched(_swatches, true);
            _colorPicker.Open(_currentColor, transform.position, _pickerPivot);
        }

        private void OnColorPickerChangedColor(Color color)
        {
            SetColor(color);
            ColorPickerChangedColor?.Invoke(color);
        }

        private void OnColorPickerClosed()
        {
            _colorPicker.onColorChanged -= OnColorPickerChangedColor;
        }

        private void UpdateColor()
        {
            _currentColorImage.color = _currentColor;
        }

        public Color GetRandomColor()
        {
            var color = _colorPicker.GetRandomColor();
            return color;
        }
    }
}