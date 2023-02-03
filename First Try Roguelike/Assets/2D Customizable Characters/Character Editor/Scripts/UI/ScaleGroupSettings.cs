using System;
using UnityEngine;
using UnityEngine.UI;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class ScaleGroupSettings : MonoBehaviour
    {
        [SerializeField] private Slider _scaleSlider;
        [SerializeField] private Slider _widthSlider;
        [SerializeField] private Slider _lengthSlider;
        [SerializeField] private Text _scaleValueText;
        [SerializeField] private Text _widthValueText;
        [SerializeField] private Text _lengthValueText;
        [SerializeField] private Button _scaleResetButton;
        [SerializeField] private Button _widthResetButton;
        [SerializeField] private Button _lengthResetButton;
        [SerializeField] private Text _labelText;

        public ScaleGroup ScaleGroup { get; private set; }
        public event Action SliderChanged;
        private const float DefaultValue = 1f;

        private void Awake()
        {
            _scaleSlider.onValueChanged.AddListener(OnScaleSliderValueChanged);
            _widthSlider.onValueChanged.AddListener(OnWidthSliderValueChanged);
            _lengthSlider.onValueChanged.AddListener(OnLengthSliderValueChanged);

            _scaleResetButton.onClick.AddListener(OnScaleResetClicked);
            _widthResetButton.onClick.AddListener(OnWidthResetClicked);
            _lengthResetButton.onClick.AddListener(OnLengthResetClicked);
        }

        public void Bind(ScaleGroup scaleGroup, float scaleDeviation)
        {
            ScaleGroup = scaleGroup;
            _labelText.text = scaleGroup.GroupName.ToUpper() + " SCALE";
            SetSliderRanges(scaleDeviation);
            UpdateSliderValues();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void ResetAll()
        {
            ScaleGroup.SetScale(DefaultValue);
            ScaleGroup.SetWidth(DefaultValue);
            ScaleGroup.SetLength(DefaultValue);
            OnSliderValueChanged();
            UpdateSliderValues();
        }

        private void OnScaleResetClicked()
        {
            ScaleGroup.SetScale(DefaultValue);
            OnSliderValueChanged();
            UpdateSliderValues();
        }

        private void OnWidthResetClicked()
        {
            ScaleGroup.SetWidth(DefaultValue);
            OnSliderValueChanged();
            UpdateSliderValues();
        }

        private void OnLengthResetClicked()
        {
            ScaleGroup.SetLength(DefaultValue);
            OnSliderValueChanged();
            UpdateSliderValues();
        }

        private void SetSliderRanges(float scaleDeviation)
        {
            var min = DefaultValue - scaleDeviation;
            var max = DefaultValue + scaleDeviation;

            _scaleSlider.minValue = min;
            _scaleSlider.maxValue = max;

            _widthSlider.minValue = min;
            _widthSlider.maxValue = max;

            _lengthSlider.minValue = min;
            _lengthSlider.maxValue = max;
        }

        public void UpdateSliderValues()
        {
            _scaleSlider.SetValueWithoutNotify(ScaleGroup.Scale);
            _widthSlider.SetValueWithoutNotify(ScaleGroup.Width);
            _lengthSlider.SetValueWithoutNotify(ScaleGroup.Length);
            _scaleValueText.text = ScaleGroup.Scale.ToString("0.00");
            _widthValueText.text = ScaleGroup.Width.ToString("0.00");
            _lengthValueText.text = ScaleGroup.Length.ToString("0.00");
        }

        private void OnSliderValueChanged()
        {
            SliderChanged?.Invoke();
        }

        private void OnScaleSliderValueChanged(float value)
        {
            ScaleGroup.SetScale(value);
            _scaleValueText.text = value.ToString("0.00");
            OnSliderValueChanged();
        }

        private void OnLengthSliderValueChanged(float value)
        {
            ScaleGroup.SetLength(value);
            _lengthValueText.text = value.ToString("0.00");
            OnSliderValueChanged();
        }

        private void OnWidthSliderValueChanged(float value)
        {
            ScaleGroup.SetWidth(value);
            _widthValueText.text = value.ToString("0.00");
            OnSliderValueChanged();
        }
    }
}