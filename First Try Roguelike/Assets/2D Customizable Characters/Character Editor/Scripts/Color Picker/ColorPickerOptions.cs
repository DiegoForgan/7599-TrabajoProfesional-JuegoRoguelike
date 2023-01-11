using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace PlaymodeColorPicker
{
    public class ColorPickerOptions : MonoBehaviour
    {
        [SerializeField] private InputField _hexInputField;
        [SerializeField] private InputField _rInputField;
        [SerializeField] private InputField _gInputField;
        [SerializeField] private InputField _bInputField;
        [SerializeField] private InputField _aInputField;
        [SerializeField] private ColorPicker _picker;
        private StringBuilder _sb = new StringBuilder();

        private void Awake()
        {
            _picker.onColorChanged += OnColorPickerColorChanged;
            _hexInputField.onEndEdit.AddListener(OnHexChanged);
            _rInputField.onValueChanged.AddListener(x => OnRGBInputChanged());
            _gInputField.onValueChanged.AddListener(x => OnRGBInputChanged());
            _bInputField.onValueChanged.AddListener(x => OnRGBInputChanged());
            _aInputField.onValueChanged.AddListener(x => OnRGBInputChanged());
            OnColorPickerColorChanged(_picker.color);
        }

        private void OnDestroy()
        {
            _picker.onColorChanged -= OnColorPickerColorChanged;
            _hexInputField.onEndEdit.RemoveListener(OnHexChanged);
            _rInputField.onValueChanged.RemoveListener(x => OnRGBInputChanged());
            _gInputField.onValueChanged.RemoveListener(x => OnRGBInputChanged());
            _bInputField.onValueChanged.RemoveListener(x => OnRGBInputChanged());
            _aInputField.onValueChanged.RemoveListener(x => OnRGBInputChanged());
        }

        private void OnHexChanged(string hex)
        {
            _sb.Clear();
            _sb.Append("#");
            _sb.Append(hex);
            ColorUtility.TryParseHtmlString(_sb.ToString(), out var color);
            byte.TryParse(_aInputField.text, out var alpha);
            color = (Color32)color;
            color.a = alpha;
            SetColorPickerColor(color);
        }

        private void OnRGBInputChanged()
        {
            byte.TryParse(_rInputField.text, out var red);
            byte.TryParse(_gInputField.text, out var green);
            byte.TryParse(_bInputField.text, out var blue);
            byte.TryParse(_aInputField.text, out var alpha);
            var color = new Color32(red, green, blue, alpha);
            SetColorPickerColor(color);
        }

        private void OnColorPickerColorChanged(Color color)
        {
            Color32 newColor = color;
            var hex = ColorUtility.ToHtmlStringRGB(color);
            _hexInputField.SetTextWithoutNotify(hex);
            _rInputField.SetTextWithoutNotify(newColor.r.ToString());
            _gInputField.SetTextWithoutNotify(newColor.g.ToString());
            _bInputField.SetTextWithoutNotify(newColor.b.ToString());
            _aInputField.SetTextWithoutNotify(newColor.a.ToString());
        }

        private void SetColorPickerColor(Color color)
        {
            _picker.color = color;
        }
    }
}