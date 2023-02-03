using UnityEngine;

namespace PlaymodeColorPicker
{
    [ExecuteAlways]
    public class ColorPickerImagePreview : MonoBehaviour
    {
        private static readonly int _AspectRatio = Shader.PropertyToID(nameof(_AspectRatio));

        [SerializeField] private ColorPicker _picker;
        [SerializeField, HideInInspector] private Shader colorPickerShader;

        private RectTransform rectTransform;
        private Material generatedMaterial;

        private void Awake()
        {
            if (_picker == null)
                return;

            rectTransform = transform as RectTransform;
            if (_picker.WrongShader())
            {
                Debug.LogWarning($"Color picker requires image material with {ColorPicker.ColorPickerShaderName} shader.");

                if (Application.isPlaying && colorPickerShader != null)
                {
                    generatedMaterial = new Material(colorPickerShader);
                    generatedMaterial.hideFlags = HideFlags.HideAndDontSave;
                }

                _picker.Image.material = generatedMaterial;
            }
        }

        private void OnDestroy()
        {
            if (generatedMaterial != null)
                DestroyImmediate(generatedMaterial);
        }

        private void Reset() => colorPickerShader = Shader.Find(ColorPicker.ColorPickerShaderName);

        private void Update()
        {
            if (_picker == null || _picker.WrongShader())
                return;

            var rect = rectTransform.rect;
            _picker.Image.material.SetFloat(_AspectRatio, rect.width / rect.height);
        }
    }
}