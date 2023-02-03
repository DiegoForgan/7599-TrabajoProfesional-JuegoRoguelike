using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class CustomizationTab : CategoryTab
    {
        [Header("References")]
        [SerializeField] private AssetOptionsPanel _assetOptionsPanel;
        [SerializeField] private Button _randomizeCategoryButton;
        [Space, Header("Categories")]
        [SerializeField] private CustomizationCategoryButton _categoryButtonPrefab;
        [SerializeField] private Transform _categoriesParent;

        public CustomizationCategory CurrentOpenCategory { get; private set; }
        private CharacterEditorCustomization[] _datas;
        private Customizer _customizer;
        private readonly List<CustomizationCategoryButton> _categoryButtons = new List<CustomizationCategoryButton>();

        public event Action<CustomizationTab> RandomizeWasClicked;

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            _categoryButtonPrefab.gameObject.SetActive(false);
            _randomizeCategoryButton.onClick.AddListener(OnRandomizeCategoryButtonClicked);
        }

        private void OnDestroy()
        {
            DestroyButtons();
            _randomizeCategoryButton.onClick.RemoveListener(OnRandomizeCategoryButtonClicked);
        }

        #endregion

        #region Public Methods

        public void BindCustomization(Customizer customizer, CharacterEditorCustomization[] data)
        {
            _customizer = customizer;
            _datas = data;
            CreateButtons();
        }

        public void Unbind()
        {
            _customizer = null;
            _datas = null;
            DestroyButtons();
        }

        #endregion

        private void OnRandomizeCategoryButtonClicked()
        {
            if (!_isShowing)
                return;

            RandomizeWasClicked?.Invoke(this);
            var categoryButton = _categoryButtons.Single(x => x.Category == CurrentOpenCategory);
            OnCategoryChangedToShowing(categoryButton);
        }

        protected override void ShowButtons()
        {
            if (_categoryButtons.Count == 0)
                return;

            _assetOptionsPanel.PickedMainColor += OnAssetMainColorPicked;
            _assetOptionsPanel.PickedDetailColor += OnAssetDetailColorPicked;
            _assetOptionsPanel.ChangedDetailSpriteIndex += OnAssetDetailSpriteIndexChanged;

            for (int i = 0; i < _categoryButtons.Count; i++)
            {
                _categoryButtons[i].gameObject.SetActive(true);
            }

            SelectFirstCategory();
        }

        protected override void HideButtons()
        {
            if (_categoryButtons.Count == 0)
                return;

            _assetOptionsPanel.PickedMainColor -= OnAssetMainColorPicked;
            _assetOptionsPanel.PickedDetailColor -= OnAssetDetailColorPicked;
            _assetOptionsPanel.ChangedDetailSpriteIndex -= OnAssetDetailSpriteIndexChanged;
            UnselectAll();

            for (int i = 0; i < _categoryButtons.Count; i++)
            {
                _categoryButtons[i].gameObject.SetActive(false);
            }
        }

        private void SelectFirstCategory() => _categoryButtons[0].GetComponent<Toggle>().isOn = true;
        private void UnselectAll() => _categoryButtons[0].GetComponent<Toggle>().group.SetAllTogglesOff();

        protected override void HideOptions()
        {
            _assetOptionsPanel.DisableOptions();
        }

        private void CreateButtons()
        {
            var categories = GetSortedCustomizationCategories();

            foreach (var category in categories)
            {
                var categoryButton = Instantiate(_categoryButtonPrefab, _categoriesParent);
                categoryButton.Bind(category.Value, category.Key);
                categoryButton.gameObject.SetActive(false);
                categoryButton.ChangedToShowing += OnCategoryChangedToShowing;
                categoryButton.SelectedCustomization += OnCustomizationSelected;
                categoryButton.SelectedEmpty += OnEmptySelected;
                _categoryButtons.Add(categoryButton);
            }
        }

        private void DestroyButtons()
        {
            for (int i = 0; i < _categoryButtons.Count; i++)
            {
                var categoryButton = _categoryButtons[i];
                categoryButton.ChangedToShowing -= OnCategoryChangedToShowing;
                categoryButton.SelectedCustomization -= OnCustomizationSelected;
                categoryButton.SelectedEmpty -= OnEmptySelected;
                categoryButton.Unbind();
                Destroy(categoryButton.gameObject);
            }

            _categoryButtons.Clear();
        }

        public Dictionary<CustomizationCategory, List<CharacterEditorCustomization>> GetSortedCustomizationCategories()
        {
            var sortedCategories = new Dictionary<CustomizationCategory, List<CharacterEditorCustomization>>();

            for (int i = 0; i < _datas.Length; i++)
            {
                var customizationData = _datas[i];

                if (customizationData == null)
                {
                    Debug.LogWarning($"Data was null.");
                    continue;
                }

                var category = customizationData.Data.Category;
                if (sortedCategories.ContainsKey(category) == false)
                    sortedCategories.Add(category, new List<CharacterEditorCustomization>());

                sortedCategories[category].Add(customizationData);
            }

            return sortedCategories;
        }

        private void ShowOptions(CustomizationData data)
        {
            var editorData = _datas.Single(x => x.Data == data);
            _assetOptionsPanel.ShowOptionsFor(data, editorData.SuggestedMainColorGroups,
                editorData.SuggestedDetailColorGroups);
            var mainColor = _customizer.GetCustomizationMainColor(data);
            var detailColor = _customizer.GetCustomizationDetailColor(data);
            _assetOptionsPanel.SetColors(mainColor, detailColor);

            var index = _customizer.GetCustomizationDetailSpritesIndex(data);
            _assetOptionsPanel.SetIndex(index);
            _assetOptionsPanel.UpdateDetailColorEnabledForIndex(index);
        }

        private void OnCategoryChangedToShowing(CustomizationCategoryButton categoryButton)
        {
            CurrentOpenCategory = categoryButton.Category;
            var currentSelected = _customizer.GetCustomizationDataInCategory(categoryButton.Category);
            categoryButton.SelectCustomization(currentSelected);
        }

        private void OnCustomizationSelected(CustomizationData data)
        {
            if (_customizer.Contains(data)) // reselected the same one
            {
                ShowOptions(data);
                return;
            }

            _customizer.Add(data);

            // var editorData = _datas.Single(x => x.Data == data);
            // var color = Color.white;
            //
            // if (data.CanBeTinted)
            // {
            //     if (editorData.HasSuggestedMainColors())
            //         color = editorData.SuggestedMainColorGroups[0].Colors[0];
            //     _customizer.SetCustomizationMainColor(data, color);
            // }
            //
            // if (data.CanDetailsBeTinted && data.UseMainColorForDetail == false)
            // {
            //     if (editorData.HasSuggestedDetailColors())
            //         color = editorData.SuggestedRandomDetailColorGroups[0].Colors[0];
            //     else
            //         color = Color.white;
            //
            //     _customizer.SetCustomizationDetailColor(data, color);
            // }

            ShowOptions(data);
        }

        private void OnAssetMainColorPicked(CustomizationData data, Color color)
        {
            _customizer.SetCustomizationMainColor(data, color);
        }

        private void OnAssetDetailColorPicked(CustomizationData data, Color color)
        {
            if (data.UseMainColorForDetail == false)
                _customizer.SetCustomizationDetailColor(data, color);
        }

        private void OnAssetDetailSpriteIndexChanged(CustomizationData data, int amount)
        {
            var currentIndex = _customizer.GetCustomizationDetailSpritesIndex(data);
            var newIndex = currentIndex + amount;
            var detailsCount = data.SpriteSets[0].AmountOfDetailSpritesSets;

            if (newIndex < 0)
                newIndex = detailsCount - 1;
            else if (newIndex > detailsCount - 1)
                newIndex = 0;

            _customizer.SetCustomizationDetailIndex(data, newIndex);

            _assetOptionsPanel.SetIndex(newIndex);
            _assetOptionsPanel.UpdateDetailColorEnabledForIndex(newIndex);
        }

        private void OnEmptySelected(CustomizationCategoryButton customizationCategoryButton)
        {
            var category = customizationCategoryButton.Category;
            SetNoCustomization(category);
            _assetOptionsPanel.DisableOptions();
        }

        private void SetNoCustomization(CustomizationCategory category)
        {
            var shouldRemoveEquipment = _customizer.HasCustomizationInCategory(category);

            if (shouldRemoveEquipment)
                _customizer.RemoveAllInCategory(category);

            _assetOptionsPanel.SetColors(Color.white, Color.white);
        }
    }
}