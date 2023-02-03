using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class CustomizationCategoryButton : MonoBehaviour
    {
        [SerializeField] private Toggle _toggleButton;
        [SerializeField] private Text _text;
        [SerializeField] private Text _selectedText;
        [SerializeField] private CustomizationDisplay _customizationDisplayPrefab;
        [SerializeField] private Transform _customizationButtonsParent;

        private List<CharacterEditorCustomization> _editorCustomizations;
        private readonly List<CustomizationDisplay> _customizationDisplays = new List<CustomizationDisplay>();

        public CustomizationCategory Category { get; private set; }

        public event Action<CustomizationCategoryButton> ChangedToShowing;
        public event Action<CustomizationCategoryButton> SelectedEmpty;
        public event Action<CustomizationData> SelectedCustomization;

        #region Unity Methods

        private void Awake()
        {
            _toggleButton.onValueChanged.AddListener(Show);
        }

        private void OnDestroy()
        {
            _toggleButton.onValueChanged.RemoveListener(Show);
        }

        #endregion

        public void Bind(List<CharacterEditorCustomization> data, CustomizationCategory category)
        {
            _customizationDisplayPrefab.gameObject.SetActive(false);
            _editorCustomizations = data;
            Category = category;
            _text.text = category.name;
            _selectedText.text = category.name;
            CreateButtons();
        }

        public void Unbind()
        {
            _editorCustomizations = null;
            DestroyButtons();
        }

        public void SelectCustomization(CustomizationData customizationData)
        {
            if (customizationData == null)
            {
                OnEmptyTurnedOn(null);
            }

            for (int i = 0; i < _customizationDisplays.Count; i++)
            {
                if (_customizationDisplays[i].CustomizationData == customizationData)
                {
                    _customizationDisplays[i].SetToggleOn(true);
                }
            }
        }

        private void ShowButtons()
        {
            for (int i = 0; i < _customizationDisplays.Count; i++)
            {
                _customizationDisplays[i].gameObject.SetActive(true);
            }
        }

        private void HideButtons()
        {
            if (_customizationDisplays.Count == 0)
                return;

            _customizationDisplays[0].GetComponent<Toggle>().group.SetAllTogglesOff();
            for (int i = 0; i < _customizationDisplays.Count; i++)
            {
                _customizationDisplays[i].gameObject.SetActive(false);
            }
        }

        private void CreateEmptyButton()
        {
            var equipmentButton = Instantiate(_customizationDisplayPrefab, _customizationButtonsParent);
            equipmentButton.Bind(null,null);
            equipmentButton.gameObject.SetActive(false);
            equipmentButton.Selected += OnEmptyTurnedOn;
            _customizationDisplays.Add(equipmentButton);
        }

        private void OnEmptyTurnedOn(CustomizationDisplay display)
        {
            SelectedEmpty?.Invoke(this);
        }

        private void CreateButtons()
        {
            if (Category.CanBeHidden)
                CreateEmptyButton();

            for (int i = 0; i < _editorCustomizations.Count; i++)
            {
                var customization = _editorCustomizations[i];

                var button = Instantiate(_customizationDisplayPrefab, _customizationButtonsParent);
                button.Bind(customization.Data,customization.Icon);
                button.gameObject.SetActive(false);
                button.Selected += OnCustomizationButtonTurnedOn;
                _customizationDisplays.Add(button);
            }
        }

        private void DestroyButtons()
        {
            if (_customizationDisplays.Count == 0)
                return;

            if (Category.CanBeHidden)
            {
                var emptyButton = _customizationDisplays[0];
                emptyButton.Selected -= OnEmptyTurnedOn;
                Destroy(emptyButton.gameObject);
            }

            var startIndex = Category.CanBeHidden ? 1 : 0;

            for (int i = startIndex; i < _customizationDisplays.Count; i++)
            {
                var setButton = _customizationDisplays[i];
                setButton.Selected -= OnCustomizationButtonTurnedOn;
                Destroy(setButton.gameObject);
            }

            _customizationDisplays.Clear();
        }

        private void Show(bool show)
        {
            if (show)
            {
                ShowButtons();
                ChangedToShowing?.Invoke(this);
            }
            else
            {
                HideButtons();
            }
        }

        private void OnCustomizationButtonTurnedOn(CustomizationDisplay customizationDisplay)
        {
            SelectedCustomization?.Invoke(customizationDisplay.CustomizationData);
        }
    }
}