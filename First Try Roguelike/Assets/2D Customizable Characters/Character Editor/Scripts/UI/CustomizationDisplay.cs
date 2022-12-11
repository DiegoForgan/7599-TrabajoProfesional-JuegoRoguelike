using System;
using UnityEngine;
using UnityEngine.UI;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class CustomizationDisplay : MonoBehaviour
    {
        [SerializeField] private Toggle _toggleButton;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _additionalOptionsImage;

        public CustomizationData CustomizationData => _customizationData;
        private CustomizationData _customizationData;

        public event Action<CustomizationDisplay> Selected;

        private void Awake()
        {
            _toggleButton.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void OnDestroy()
        {
            _toggleButton.onValueChanged.RemoveListener(OnToggleValueChanged);
        }

        public void Bind(CustomizationData data, Sprite icon)
        {
            if (data == null)
            {
                _customizationData = null;
                _additionalOptionsImage.enabled = false;
                return;
            }

            _customizationData = data;

            if (icon == null && data.SpriteSets[0].DownSprite != null)
                _iconImage.sprite = data.SpriteSets[0].DownSprite;
            else
                _iconImage.sprite = icon;

            _additionalOptionsImage.enabled = data.HasDetailSprites() && data.GetDetailSpriteCount() > 1;
        }

        public void SetToggleOn(bool isOn)
        {
            GetComponent<Toggle>().isOn = !isOn;
            GetComponent<Toggle>().isOn = isOn;
        }

        private void OnToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                Selected?.Invoke(this);
            }
        }
    }
}