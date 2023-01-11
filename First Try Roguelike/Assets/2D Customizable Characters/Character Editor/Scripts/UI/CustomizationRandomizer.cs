using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CustomizableCharacters.CharacterEditor.UI
{
    [Serializable]
    public class ColorMatchGroup
    {
        [SerializeField] private CustomizationCategory[] _categories = Array.Empty<CustomizationCategory>();

        public CustomizationCategory[] Categories => _categories;
    }

    public class CustomizationRandomizer : MonoBehaviour
    {
        [SerializeField] private CustomizationTab _appearanceTab;
        [SerializeField] private CustomizationTab _equipmentTab;
        [SerializeField] private ScaleTab _scaleTab;
        [SerializeField] private ColorOptionButton _bodyColorOptionButton;

        // rules
        [SerializeField] private ColorMatchGroup[] _colorMatchGroups;

        private CustomizableCharacter _character;

        private void Awake()
        {
            _appearanceTab.RandomizeWasClicked += OnTabRandomizeWasClicked;
            _equipmentTab.RandomizeWasClicked += OnTabRandomizeWasClicked;
        }

        private void OnDestroy()
        {
            _appearanceTab.RandomizeWasClicked -= OnTabRandomizeWasClicked;
            _equipmentTab.RandomizeWasClicked -= OnTabRandomizeWasClicked;
        }

        public void BindCharacter(CustomizableCharacter character) => _character = character;

        public void Randomize(CharacterEditorData data, CharacterEditorSettings settings)
        {
            RandomizeTab(_character, _appearanceTab);
            RandomizeTab(_character, _equipmentTab);
            RandomizeScale(_character.ScaleCustomizer, settings.ScaleDeviation);
            RandomizeBodyColor(_character, data);
            MatchColors(_character);
        }

        private void OnTabRandomizeWasClicked(CustomizationTab tab)
        {
            var sortedCategories = tab.GetSortedCustomizationCategories();
            RandomizeCategory(_character.Customizer, tab.CurrentOpenCategory, sortedCategories[tab.CurrentOpenCategory]);
        }

        private void MatchColors(CustomizableCharacter character)
        {
            for (int i = 0; i < _colorMatchGroups.Length; i++)
            {
                var group = _colorMatchGroups[i];
                var gotColor = false;
                var mainColor = Color.white;
                var detailColor = Color.white;

                for (int j = 0; j < group.Categories.Length; j++)
                {
                    var category = group.Categories[j];
                    var data = character.Customizer.GetCustomizationDataInCategory(category);

                    if (data != null)
                    {
                        var customization = character.Customizer.GetCustomizationWithData(data);
                        if (!gotColor)
                        {
                            mainColor = customization.MainColor;
                            detailColor = customization.DetailColor;
                            gotColor = true;
                        }
                        else
                        {
                            character.Customizer.SetCustomizationMainColor(data, mainColor);
                            if (data.CanDetailsBeTinted && !data.CanDetailsBeTinted)
                                character.Customizer.SetCustomizationDetailColor(data, detailColor);
                        }
                    }
                }
            }
        }

        private void RandomizeTab(CustomizableCharacter character, CustomizationTab tab)
        {
            var categories = tab.GetSortedCustomizationCategories();
            foreach (var categoryPair in categories)
            {
                RandomizeCategory(character.Customizer, categoryPair.Key, categoryPair.Value);
            }
        }

        private void RandomizeCategory(Customizer customizer, CustomizationCategory category,
            List<CharacterEditorCustomization> customizations)
        {
            var minRange = 0;
            if (category.CanBeHidden)
                minRange = -1;

            var randomIndex = Random.Range(minRange, customizations.Count);
            if (randomIndex == -1)
            {
                if (customizer.HasCustomizationInCategory(category))
                {
                    var shouldRemoveEquipment = customizer.HasCustomizationInCategory(category);
                    if (shouldRemoveEquipment)
                        customizer.RemoveAllInCategory(category);
                    return;
                }

                randomIndex = Random.Range(0, customizations.Count);
            }

            var editorCustomization = customizations[randomIndex];
            customizer.Add(editorCustomization.Data);

            if (editorCustomization.Data.CanBeTinted)
            {
                var color = Color.white;
                if (editorCustomization.HasSuggestedMainColors())
                    color = editorCustomization.GetRandomMainColor();

                customizer.SetCustomizationMainColor(editorCustomization.Data, color);
            }

            if (editorCustomization.Data.HasDetailSprites())
            {
                var detailIndex = editorCustomization.Data.SpriteSets[0].AmountOfDetailSpritesSets;
                randomIndex = Random.Range(0, detailIndex);
                customizer.SetCustomizationDetailIndex(editorCustomization.Data, randomIndex);

                if (editorCustomization.Data.CanDetailsBeTinted && editorCustomization.Data.UseMainColorForDetail == false)
                {
                    var color = Color.white;
                    if (editorCustomization.HasSuggestedDetailColors())
                        color = editorCustomization.GetRandomDetailColor();

                    customizer.SetCustomizationDetailColor(editorCustomization.Data, color);
                }
            }
        }

        private void RandomizeScale(ScaleCustomizer scaleCustomizer, float scaleDeviation)
        {
            var groups = scaleCustomizer.ScaleGroups;

            for (int i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var min = 1 - scaleDeviation;
                var max = 1 + scaleDeviation;

                var randomScale = GenerateWeightedRandom(min, max);
                group.SetScale(randomScale);

                // divide by two to avoid extreme cases with extreme stretching
                min = 1 - (scaleDeviation / 2);
                max = 1 + (scaleDeviation / 2);

                randomScale = GenerateWeightedRandom(min, max);
                group.SetWidth(randomScale);

                randomScale = GenerateWeightedRandom(min, max);
                group.SetLength(randomScale);
            }

            scaleCustomizer.ApplyAllScaleGroups();
        }

        //Random Box Muller
        private float GenerateWeightedRandom(float min, float max)
        {
            while (true)
            {
                float v = GetNext((max - min) * 0.16f, (min + max) * 0.5f);
                if (min <= v && v <= max) return v;
            }

            float GetNext(float sigma = 1f, float mu = 0)
            {
                float rand1, rand2;
                while ((rand1 = Random.value) == 0) ;
                while ((rand2 = Random.value) == 0) ;
                return Mathf.Sqrt(-2f * Mathf.Log(rand1)) * Mathf.Cos(2f * Mathf.PI * rand2) * sigma + mu;
            }
        }

        private void RandomizeBodyColor(CustomizableCharacter character, CharacterEditorData data)
        {
            var color = Color.white;
            if (data.HasRandomBodyColors())
                color = data.GetRandomBodyColor();
            else
                color = _bodyColorOptionButton.GetRandomColor();

            character.Customizer.SetBodyColor(color);
            _bodyColorOptionButton.SetColor(color);
        }
    }
}