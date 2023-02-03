// Modified version of: https://github.com/mmaletin/UnityColorPicker
//
// MIT License
//
// Copyright (c) 2019 Max Maletin
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.Mathf;
using Random = UnityEngine.Random;

namespace PlaymodeColorPicker
{
    public class ColorPicker : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        private const float recip2Pi = 0.159154943f;
        public const string ColorPickerShaderName = "UI/ColorPicker";

        private static readonly int _HSV = Shader.PropertyToID(nameof(_HSV));

        private static readonly int _HueCircleInner = Shader.PropertyToID(nameof(_HueCircleInner));
        private static readonly int _SVSquareSize = Shader.PropertyToID(nameof(_SVSquareSize));

        [SerializeField] private Image image;
        [SerializeField] private Button _blocker;
        [SerializeField] private ColorSwatches _swatches;

        private RectTransform imageRectTransform;
        private Color _color;
        public Image Image => image;

        private enum PointerDownLocation
        {
            HueCircle,
            SVSquare,
            Outside
        }

        private PointerDownLocation pointerDownLocation = PointerDownLocation.Outside;
        float h, s, v, alpha;

        public Color color
        {
            get
            {
                var newColor = Color.HSVToRGB(h, s, v);
                newColor.a = alpha;
                return newColor;
            }
            set
            {
                Color.RGBToHSV(value, out h, out s, out v);
                alpha = value.a;
                ApplyColor();
            }
        }

        public event Action Closed;
        public event Action<Color> onColorChanged;

        #region Unity Methods

        private void Awake()
        {
            imageRectTransform = image.transform as RectTransform;
            _swatches.SwatchPicked += x => color = x;
            Close();
        }

        private void OnDestroy()
        {
            _swatches.SwatchPicked += x => color = x;
            Close();
        }

        #endregion

        #region Public Methods

        public void Open(Color startColor, Vector2 position, Vector2 pivot)
        {
            var rect = GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.position = position;
            rect.pivot = pivot;

            _blocker.transform.SetParent(transform.parent);
            _blocker.transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
            _blocker.onClick.AddListener(Close);
            _blocker.gameObject.SetActive(true);

            _swatches.AdjustPivotAndPosition(pivot, rect);

            color = startColor;
            gameObject.SetActive(true);
        }

        public void Close()
        {
            Closed?.Invoke();
            _blocker.onClick.RemoveListener(Close);
            _blocker.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        public bool WrongShader() => image?.material?.shader?.name != ColorPickerShaderName;

        public void SetColorSwatched(ColorGroup[] colorGroups, bool clearPrevious)
        {
            if (colorGroups == null)
                return;

            if (clearPrevious)
                _swatches.ClearSwatches();
            _swatches.BindGroups(colorGroups);
        }

        public Color GetRandomColor()
        {
            Color color = new Color(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f));
            return color;
        }

        #endregion

        #region Input Handlers

        public void OnDrag(PointerEventData eventData)
        {
            if (WrongShader()) return;

            var pos = GetRelativePosition(eventData);

            if (pointerDownLocation == PointerDownLocation.HueCircle)
            {
                h = (Atan2(pos.y, pos.x) * recip2Pi + 1) % 1;
                ApplyColor();
            }

            if (pointerDownLocation == PointerDownLocation.SVSquare)
            {
                var size = image.material.GetFloat(_SVSquareSize);

                s = InverseLerp(-size, size, pos.x);
                v = InverseLerp(-size, size, pos.y);
                ApplyColor();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (WrongShader()) return;

            var pos = GetRelativePosition(eventData);

            float r = pos.magnitude;

            if (r < .5f && r > image.material.GetFloat(_HueCircleInner))
            {
                pointerDownLocation = PointerDownLocation.HueCircle;
                h = (Atan2(pos.y, pos.x) * recip2Pi + 1) % 1;
                ApplyColor();
            }
            else
            {
                var size = image.material.GetFloat(_SVSquareSize);

                // s -> x, v -> y
                if (pos.x >= -size && pos.x <= size && pos.y >= -size && pos.y <= size)
                {
                    pointerDownLocation = PointerDownLocation.SVSquare;
                    s = InverseLerp(-size, size, pos.x);
                    v = InverseLerp(-size, size, pos.y);
                    ApplyColor();
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            pointerDownLocation = PointerDownLocation.Outside;
        }

        #endregion

        #region Private Methods

        private void ApplyColor()
        {
            image.material.SetVector(_HSV, new Vector3(h, s, v));
            onColorChanged?.Invoke(color);
        }

        /// <summary>
        /// Returns position in range -0.5..0.5 when it's inside color picker square area
        /// </summary>
        private Vector2 GetRelativePosition(PointerEventData eventData)
        {
            var rect = GetSquaredRect();

            Vector2 rtPos;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(imageRectTransform, eventData.position,
                eventData.pressEventCamera, out rtPos);

            return new Vector2(InverseLerpUnclamped(rect.xMin, rect.xMax, rtPos.x),
                InverseLerpUnclamped(rect.yMin, rect.yMax, rtPos.y)) - Vector2.one * 0.5f;
        }

        private Rect GetSquaredRect()
        {
            var rect = imageRectTransform.rect;
            var smallestDimension = Min(rect.width, rect.height);
            return new Rect(rect.center - Vector2.one * smallestDimension * 0.5f, Vector2.one * smallestDimension);
        }

        private float InverseLerpUnclamped(float min, float max, float value)
        {
            return (value - min) / (max - min);
        }

        #endregion
    }
}