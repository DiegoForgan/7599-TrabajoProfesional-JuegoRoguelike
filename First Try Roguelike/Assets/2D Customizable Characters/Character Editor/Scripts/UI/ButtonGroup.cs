using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class ButtonGroup : ButtonDropdown
    {
        [SerializeField] private Transform _buttonParent;
        [SerializeField] private GroupButton _buttonPrefab;

        private List<GroupButton> _buttons = new List<GroupButton>();

        public event Action<int> ButtonClicked;

        protected override void Awake()
        {
            base.Awake();
            if (_buttonPrefab != null)
                _buttonPrefab.gameObject.SetActive(false);

            UpdateInteractable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveButtons();
        }

        public void ClearButtons()
        {
            RemoveButtons();
            ResizeDropdown();
        }

        public void AddButton(string label)
        {
            var button = Instantiate(_buttonPrefab, _buttonParent);
            button.gameObject.SetActive(true);
            button.GetComponentInChildren<Text>().text = label;
            button.Clicked += OnButtonClicked;
            _buttons.Add(button);
            button.SetGroupIndex(_buttons.Count - 1);
            ResizeDropdown();
            UpdateInteractable();
        }

        private void RemoveButtons()
        {
            for (int i = _buttons.Count - 1; i >= 0; i--)
            {
                var button = _buttons[i];
                button.Clicked -= OnButtonClicked;
                Destroy(button.gameObject);
            }

            _buttons.Clear();
            UpdateInteractable();
        }

        private void UpdateInteractable()
        {
            _toggle.interactable = _buttons.Count > 0;
        }

        private void ResizeDropdown()
        {
            var rect = _dropdown.GetComponent<RectTransform>();
            var height = _buttonPrefab.GetComponent<RectTransform>().rect.size.y * _buttons.Count;
            height = Mathf.Clamp(height, 0, 600);
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);
        }

        private void OnButtonClicked(int index)
        {
            ButtonClicked?.Invoke(index);
            _toggle.isOn = false;
        }
    }
}