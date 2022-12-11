using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public enum ResolutionType
    {
        Total = 0,
        PerUnit = 1,
    }

    public class CaptureAreaBounds : MonoBehaviour
    {
        private const float Padding = 0.05f;

        [SerializeField] private SpriteRenderer _centerPointPrefab;
        [SerializeField] private SpriteRenderer _borderOutlinePrefab;
        [SerializeField] private TextMesh _dimensionsTextPrefab;
        [SerializeField] private InputField _resolutionInput;
        [SerializeField] private InputField _centerXInputField;
        [SerializeField] private InputField _centerYInputField;
        [SerializeField] private InputField _sizeInputField;
        [SerializeField] private Dropdown _resolutionDropdown;

        private SpriteRenderer _centerPoint;
        private SpriteRenderer _borderOutline;
        private TextMesh _dimensionsText;
        private ResolutionType _resolutionType;
        private Vector2 _center;
        private float _size;
        private Bounds _bounds;
        private Vector2Int _resolution;
        public Vector2Int Resolution => _resolution;
        public Bounds Bounds => _bounds;

        public event Action ResolutionChanged;

        #region Unity Methods

        private void Awake()
        {
            _resolutionInput.onValueChanged.AddListener(OnResolutionChanged);
            _resolutionDropdown.onValueChanged.AddListener(OnResolutionDropdownChanged);
            _centerXInputField.onValueChanged.AddListener(OnBoundsSettingInputChanged);
            _centerYInputField.onValueChanged.AddListener(OnBoundsSettingInputChanged);
            _sizeInputField.onValueChanged.AddListener(OnBoundsSettingInputChanged);

            CreateVisuals();
            SetHidden(true);
        }

        private void OnDestroy()
        {
            _resolutionInput.onEndEdit.RemoveListener(OnResolutionChanged);
            _resolutionDropdown.onValueChanged.RemoveListener(OnResolutionDropdownChanged);
            _centerXInputField.onValueChanged.RemoveListener(OnBoundsSettingInputChanged);
            _centerYInputField.onValueChanged.RemoveListener(OnBoundsSettingInputChanged);
            _sizeInputField.onValueChanged.RemoveListener(OnBoundsSettingInputChanged);

            DestroyVisuals();
        }

        #endregion

        public void SetHidden(bool hidden)
        {
            _borderOutline.gameObject.SetActive(!hidden);
            _dimensionsText.gameObject.SetActive(!hidden);
            _centerPoint.gameObject.SetActive(!hidden);
        }

        public void DestroyVisuals()
        {
            if (_borderOutline != null)
                Destroy(_borderOutline.gameObject);
            if (_dimensionsText != null)
                Destroy(_dimensionsText.gameObject);
            if (_centerPoint != null)
                Destroy(_centerPoint.gameObject);
        }

        // Not working as intended due to rotation of the sprites and bounds not taking this in to account
        // public void CreateBoundsFromClips(SpriteExporterClip[] exporterClips, Animation animation)
        // {
        //     var bounds = new Bounds();
        //
        //     var progress = 0;
        //     for (int i = 0; i < exporterClips.Length; i++)
        //     {
        //         // DisplayProgressBar("Calculating Bounds", "Calculating bounds for the capture area...",
        //         //     progress / total);
        //
        //         var clip = exporterClips[i].AnimationClip;
        //         var clipName = clip.name;
        //         clip = CreateLegacyClip(clip);
        //         animation.AddClip(clip, clipName);
        //         var time = 0f;
        //         var timeIncrement = 0.1f;
        //         var state = animation[clipName];
        //         state.enabled = true;
        //         state.weight = 1;
        //         animation.enabled = false;
        //
        //         while (time < state.length)
        //         {
        //             state.time = time;
        //             animation.Sample();
        //             time += timeIncrement;
        //
        //             var frameBounds = GetBoundsFromChildren(exporterClips[i].RigGameObject);
        //             var extends = frameBounds.extents;
        //
        //             if (bounds.extents.x > extends.x)
        //                 extends.x = bounds.extents.x;
        //             if (bounds.extents.y > extends.y)
        //                 extends.y = bounds.extents.y;
        //
        //             bounds.extents = extends;
        //         }
        //
        //         // make square aspect ratio
        //         bounds.extents = bounds.extents.x > bounds.extents.y
        //             ? new Vector2(bounds.extents.x, bounds.extents.x)
        //             : new Vector2(bounds.extents.y, bounds.extents.y);
        //
        //         animation.RemoveClip(clip.name);
        //         progress++;
        //     }
        //
        //     SetBounds(bounds.center, bounds.size.x);
        //
        //     if (_resolutionType == ResolutionType.PerUnit)
        //     {
        //         var resolution = int.Parse(_resolutionInput.text);
        //         var newResolution = Mathf.RoundToInt(resolution * bounds.size.x);
        //         SetResolution(newResolution);
        //     }
        //
        //     UpdateVisuals();
        //     UpdateInputFields();
        //     // ClearProgressBar();
        // }

        public void UpdateBounds()
        {
            UpdateSettings();
            SetBounds(_center, _size);
            UpdateVisuals();
        }

        private void UpdateSettings()
        {
            if (string.IsNullOrEmpty(_resolutionInput.text))
                _resolutionInput.text = "0";
            if (string.IsNullOrEmpty(_centerXInputField.text))
                _centerXInputField.text = "0";
            if (string.IsNullOrEmpty(_centerYInputField.text))
                _centerYInputField.text = "0";
            if (string.IsNullOrEmpty(_sizeInputField.text))
                _sizeInputField.text = "0";

            var resolution = int.Parse(_resolutionInput.text, CultureInfo.InvariantCulture.NumberFormat);
            var size = float.Parse(_sizeInputField.text, CultureInfo.InvariantCulture.NumberFormat);

            if (_resolutionType == ResolutionType.PerUnit)
            {
                var newResolution = resolution * size;
                resolution = Mathf.RoundToInt(newResolution);
            }

            SetResolution(resolution);
            _size = size;
            _center.x = float.Parse(_centerXInputField.text, CultureInfo.InvariantCulture.NumberFormat);
            _center.y = float.Parse(_centerYInputField.text, CultureInfo.InvariantCulture.NumberFormat);
        }

        private void CreateVisuals()
        {
            _borderOutline = Instantiate(_borderOutlinePrefab);
            _borderOutline.gameObject.hideFlags = HideFlags.HideInHierarchy;

            _dimensionsText = Instantiate(_dimensionsTextPrefab);
            _dimensionsText.gameObject.hideFlags = HideFlags.HideInHierarchy;

            _centerPoint = Instantiate(_centerPointPrefab);
            _centerPoint.gameObject.hideFlags = HideFlags.HideInHierarchy;
        }

        private void UpdateVisuals()
        {
            _borderOutline.transform.position = _bounds.center;
            _borderOutline.size = _bounds.extents * 2;
            _dimensionsText.text = _resolution.x + " x " + _resolution.y + " px";
            var dimensionsPosition = _bounds.center;
            dimensionsPosition.x -= _bounds.extents.x;
            dimensionsPosition.y += _bounds.extents.y;
            _dimensionsText.transform.position = dimensionsPosition;
            _centerPoint.transform.position = _center;
        }

        private void SetBounds(Vector2 center, float size)
        {
            _bounds = new Bounds(center, new Vector3(size, size, 0));
        }

        private void UpdateInputFields()
        {
            _centerXInputField.SetTextWithoutNotify(_bounds.center.x.ToString());
            _centerYInputField.SetTextWithoutNotify(_bounds.center.y.ToString());
            _sizeInputField.SetTextWithoutNotify(_bounds.size.x.ToString());
        }

        private AnimationClip CreateLegacyClip(AnimationClip clip)
        {
            var legacyClip = Instantiate(clip);
            legacyClip.legacy = true;
            legacyClip.wrapMode = WrapMode.Loop;
            legacyClip.name = legacyClip.name.Replace("(Clone)", "");
            return legacyClip;
        }

        // private Bounds GetBoundsFromChildren(GameObject target)
        // {
        //     var boundsResult = new Bounds();
        //     boundsResult.center = target.transform.position;
        //
        //     var renderers = target.GetComponentsInChildren<SpriteRenderer>(true);
        //
        //     for (int i = 0; i < renderers.Length; i++)
        //     {
        //         var currentExtents = boundsResult.extents;
        //
        //         var sprite = renderers[i].sprite;
        //         if (sprite == null)
        //             continue;
        //
        //         Rect croppedRect = new Rect(
        //             (sprite.textureRectOffset.x + sprite.textureRect.width / 2f) / sprite.pixelsPerUnit,
        //             (sprite.textureRectOffset.y + sprite.textureRect.height / 2f) / sprite.pixelsPerUnit,
        //             sprite.textureRect.width / sprite.pixelsPerUnit,
        //             sprite.textureRect.height / sprite.pixelsPerUnit);
        //
        //         var pivotOffset = croppedRect.position - (sprite.pivot / sprite.pixelsPerUnit);
        //         pivotOffset *= renderers[i].transform.lossyScale;
        //         var centerOffset = (renderers[i].transform.position + (Vector3)pivotOffset) - boundsResult.center;
        //         var size = croppedRect.size * renderers[i].transform.lossyScale;
        //         size.x += Padding;
        //         size.y += Padding;
        //
        //         if ((centerOffset.x + (size.x / 2)) > currentExtents.x
        //             || Mathf.Abs(centerOffset.x - (size.x / 2)) > currentExtents.x)
        //             currentExtents.x = Mathf.Abs(centerOffset.x) + (size.x / 2);
        //
        //         if (centerOffset.y + (size.y / 2) > currentExtents.y
        //             || Mathf.Abs((centerOffset.y) - (size.y / 2)) > currentExtents.y)
        //             currentExtents.y = Mathf.Abs(centerOffset.y) + (size.y / 2);
        //
        //         boundsResult.extents = currentExtents;
        //     }
        //
        //     return boundsResult;
        // }

        private void SetResolution(int resolution)
        {
            if (resolution <= 0)
                resolution = 1;

            _resolution.x = resolution;
            _resolution.y = resolution;
            ResolutionChanged?.Invoke();
        }

        #region Event Handlers

        private void OnResolutionDropdownChanged(int value)
        {
            _resolutionType = (ResolutionType)value;
            UpdateBounds();
        }

        private void OnResolutionChanged(string value)
        {
            UpdateBounds();
        }

        private void OnBoundsSettingInputChanged(string value)
        {
            UpdateSettings();
            SetBounds(_center, _size);
            UpdateVisuals();
        }

        #endregion
    }
}