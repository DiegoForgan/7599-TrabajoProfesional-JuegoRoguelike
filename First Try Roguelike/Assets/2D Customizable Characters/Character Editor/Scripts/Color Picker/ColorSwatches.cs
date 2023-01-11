using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlaymodeColorPicker
{
    public class ColorSwatches : MonoBehaviour
    {
        [SerializeField] private UIColorGroup _colorGroupPrefab;
        [SerializeField] private Transform _groupsParent;

        private readonly List<UIColorGroup> _uiColorGroups = new List<UIColorGroup>();
        private ColorGroup[] _colorGroups;

        public event Action<Color> SwatchPicked;

        private void Awake()
        {
            _colorGroupPrefab.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            DestroyColorSwatches();
        }

        public void BindGroups(ColorGroup[] colorGroups)
        {
            _colorGroups = colorGroups;

            if (_colorGroups.Length > 0)
            {
                if (gameObject.activeSelf == false)
                    gameObject.SetActive(true);
                CreateGroups();
            }
            else
                gameObject.SetActive(false);
        }

        public void ClearSwatches()
        {
            DestroyColorSwatches();
        }

        public void AdjustPivotAndPosition(Vector2 pivot, RectTransform adjustToRect)
        {
            var recTransform = GetComponent<RectTransform>();
            var swatchesPivot = new Vector2(0, pivot.y);
            recTransform.pivot = swatchesPivot;

            var corners = new Vector3[4];
            adjustToRect.GetWorldCorners(corners);
            if (pivot.y < 0.5f)
                recTransform.position = corners[1];
            else
                recTransform.position = corners[0];
        }

        private void CreateGroups()
        {
            for (int i = 0; i < _colorGroups.Length; i++)
            {
                var group = Instantiate(_colorGroupPrefab, _groupsParent);
                group.gameObject.SetActive(true);
                group.Bind(_colorGroups[i].GroupName, _colorGroups[i].Colors);
                group.SwatchPicked += OnGroupSwatchPicked;
                _uiColorGroups.Add(group);
            }
        }

        private void DestroyColorSwatches()
        {
            for (int i = _uiColorGroups.Count - 1; i >= 0; i--)
            {
                var button = _uiColorGroups[i];
                Destroy(button.gameObject);
            }

            _uiColorGroups.Clear();
        }

        private void OnGroupSwatchPicked(Color color) => SwatchPicked?.Invoke(color);
    }
}