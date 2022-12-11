using System;
using UnityEngine;
using UnityEngine.UI;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class ScaleGroupButton : MonoBehaviour
    {
        [SerializeField] private Toggle _toggleButton;
        [SerializeField] private Text _text;
        [SerializeField] private Text _selectedText;
        [SerializeField] private ScaleGroupSettings _scaleGroupSettings;

        public event Action<ScaleGroup> ScaleGroupValueChanged;

        private ScaleGroup _scaleGroup;
        private float _scaleDeviation;

        private void Awake()
        {
            _toggleButton.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void OnDestroy()
        {
            _toggleButton.onValueChanged.RemoveListener(OnToggleValueChanged);
        }

        private void OnGroupSettingsChanged()
        {
            ScaleGroupValueChanged?.Invoke(_scaleGroup);
        }

        private void OnToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                _scaleGroupSettings.Bind(_scaleGroup, _scaleDeviation);
                _scaleGroupSettings.Show();
                _scaleGroupSettings.SliderChanged += OnGroupSettingsChanged;
            }
            else
            {
                _scaleGroupSettings.SliderChanged -= OnGroupSettingsChanged;
                _scaleGroupSettings.Hide();
            }
        }

        public void Bind(ScaleGroup scaleGroup, float scaleDeviation)
        {
            _scaleGroup = scaleGroup;
            _text.text = scaleGroup.GroupName;
            _selectedText.text = scaleGroup.GroupName;
            _scaleDeviation = scaleDeviation;
        }
    }
}