using System;
using PlaymodeColorPicker;
using UnityEngine;
using UnityEngine.UI;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class AssetOptionsPanel : MonoBehaviour
    {
        [SerializeField] private ColorOptionButton _mainColorOptionButton;
        [SerializeField] private ColorOptionButton _detailColorOptionButton;
        [SerializeField] private Button _detailIndexSubtractButton;
        [SerializeField] private Button _detailIndexAddButton;
        [SerializeField] private GameObject _detailParent;
        [SerializeField] private Text _indexText;

        public Color CurrentColor => _mainColorOptionButton.CurrentColor;
        public Color CurrentDetailColor => _detailColorOptionButton.CurrentColor;
        public ColorOptionButton MainColorOptionButton => _mainColorOptionButton;
        public ColorOptionButton DetailColorOptionButton => _detailColorOptionButton;

        private CustomizationData _customizationData;

        public event Action<CustomizationData, Color> PickedMainColor;
        public event Action<CustomizationData, Color> PickedDetailColor;
        public event Action<CustomizationData, int> ChangedDetailSpriteIndex;

        private void Awake()
        {
            _mainColorOptionButton.ColorPickerChangedColor += OnMainColorPickerChangedColor;
            _detailColorOptionButton.ColorPickerChangedColor += OnDetailColorPickerChangedColor;
            _detailIndexSubtractButton.onClick.AddListener(() => ChangeDetailIndex(-1));
            _detailIndexAddButton.onClick.AddListener(() => ChangeDetailIndex(+1));

            DisableOptions();
        }

        private void OnDestroy()
        {
            _mainColorOptionButton.ColorPickerChangedColor -= OnMainColorPickerChangedColor;
            _detailColorOptionButton.ColorPickerChangedColor -= OnDetailColorPickerChangedColor;
            _detailIndexSubtractButton.onClick.RemoveListener(() => ChangeDetailIndex(-1));
            _detailIndexAddButton.onClick.RemoveListener(() => ChangeDetailIndex(+1));
        }

        public void ShowOptionsFor(CustomizationData data, ColorGroup[] mainColors, ColorGroup[] detailColors)
        {
            if (data == null)
                return;

            _customizationData = data;

            var shouldShowMainColor = data.CanBeTinted;
            _mainColorOptionButton.SetEnabled(shouldShowMainColor);
            _mainColorOptionButton.BindSwatches(mainColors);

            var hasDetailSprites = _customizationData.HasDetailSprites();
            var canDetailBeTinted = data.CanDetailsBeTinted && !data.UseMainColorForDetail;
            var shouldShowDetailColorOption = hasDetailSprites && canDetailBeTinted;
            _detailColorOptionButton.SetEnabled(shouldShowDetailColorOption);
            _detailColorOptionButton.BindSwatches(detailColors);
            ShowDetailIndex(hasDetailSprites & _customizationData.GetDetailSpriteCount() > 1);
        }

        public void DisableOptions()
        {
            _customizationData = null;
            _mainColorOptionButton.SetEnabled(false);
            _detailColorOptionButton.SetEnabled(false);
            ShowDetailIndex(false);
        }

        public void SetColors(Color color, Color detailColor)
        {
            _mainColorOptionButton.SetColor(color);
            _detailColorOptionButton.SetColor(detailColor);
        }

        public void SetIndex(int index) => _indexText.text = index.ToString();

        private void ShowDetailIndex(bool show)
        {
            _detailParent.SetActive(show);
        }

        private void OnMainColorPickerChangedColor(Color color) => PickedMainColor?.Invoke(_customizationData, color);

        private void OnDetailColorPickerChangedColor(Color color) => PickedDetailColor?.Invoke(_customizationData, color);

        private void ChangeDetailIndex(int amount) => ChangedDetailSpriteIndex?.Invoke(_customizationData, amount);

        public void UpdateDetailColorEnabledForIndex(int index)
        {
            var canDetailBeTinted = _customizationData.CanDetailsBeTinted && !_customizationData.UseMainColorForDetail;
            var shouldShowDetailColorOption = canDetailBeTinted && _customizationData.HasDetailSpriteAtIndex(index);
            _detailColorOptionButton.SetEnabled(shouldShowDetailColorOption);
        }
    }
}