using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEngine;

namespace CustomizableCharacters
{
    /// <summary>
    /// Used to do appearance and equipment customizations of the character.
    /// </summary>
    public class Customizer : MonoBehaviour
    {
#if UNITY_EDITOR
        [Header("Settings")]
        [Tooltip(
            "If the component should refresh everything when a change was made. Disable if live update isn't needed or there is any performance issues.")]
        [SerializeField] private bool _refreshOnValidate = true;
#endif

        [Space]
        [Header("References")]
        [Tooltip("The GameObject that is considered down direction character rig")]
        [SerializeField] private Transform _downRig;
        [Tooltip("The GameObject that is considered side direction character rig")]
        [SerializeField] private Transform _sideRig;
        [Tooltip("The GameObject that is considered up direction character rig")]
        [SerializeField] private Transform _upRig;

        [Header("Body")]
        [Tooltip("The color that will be applied to all the body parts.")]
        [SerializeField] private Color _bodyColor = Color.white;

        [Space]
        [SerializeField] private List<Customization> _customizations = new List<Customization>();

        private CustomizationSlot[] _downSlots = Array.Empty<CustomizationSlot>();
        private CustomizationSlot[] _sideSlots = Array.Empty<CustomizationSlot>();
        private CustomizationSlot[] _upSlots = Array.Empty<CustomizationSlot>();
        private BodyPart[] _bodyParts;

        private Dictionary<CustomizationData, Customization> _customizationDatas =
            new Dictionary<CustomizationData, Customization>();
        private Dictionary<CustomizationCategory, Customization> _categories =
            new Dictionary<CustomizationCategory, Customization>();
        private Dictionary<CustomizationSlot, List<Customization>> _slots =
            new Dictionary<CustomizationSlot, List<Customization>>();

        public ReadOnlyCollection<Customization> Customizations => _customizations.AsReadOnly();

        public Color BodyColor => _bodyColor;
        public bool IsAllCustomizationHidden { get; private set; }

        public event Action<Customization> AddedCustomization;
        public event Action<Customization> RemovedCustomization;

        #region Unity Methods

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (_refreshOnValidate)
                EditorApplication.delayCall += DelayedOnValidate;
        }

        private void OnDestroy()
        {
            EditorApplication.delayCall -= DelayedOnValidate;
        }
#endif

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds and displays a customization. Note that any current customization in the same category will be removed.
        /// </summary>
        /// <param name="customization"></param>
        public void Add(Customization customization)
        {
            if (customization == null)
            {
                Debug.LogWarning($"Can't add {nameof(Customization)}, it was null.");
                return;
            }

            // if (Contains(customization.CustomizationData))
            // {
            //     Debug.LogWarning($"{customization.CustomizationData.name} has already been added and will not be added");
            //     return;
            // }

            var category = customization.CustomizationData.Category;

            if (_categories.ContainsKey(category))
                Remove(_categories[category].CustomizationData);


            _customizations.Add(customization);
            TryAddCustomizationToLookups(customization);
            AddCustomizationToSlots(customization);

            AddedCustomization?.Invoke(customization);
        }

        /// <summary>
        /// Creates, adds and displays a customization from data. Note that any current customization in the same category will be removed.
        /// </summary>
        /// <param name="data">The data that will be used to create the customization</param>
        public void Add(CustomizationData data)
        {
            var customization = CreateCustomization(data);
            var category = data.Category;
            if (_categories.ContainsKey(category))
            {
                var color = _categories[category].MainColor;
                var detailColor = _categories[category].DetailColor;

                if (customization.CustomizationData.CanBeTinted)
                    customization.SetMainColor(color);
                if (customization.CustomizationData.CanDetailsBeTinted)
                    customization.SetDetailColor(detailColor);
            }

            Add(customization);
        }

        /// <summary>
        /// Adds all Customizations from a CustomizationSet. Note that new customizations will be created and the ones on the set will not be referenced.
        /// </summary>
        /// <param name="set"></param>
        public void ApplySet(CustomizationSet set)
        {
            for (int i = 0; i < set.Customizations.Length; i++)
            {
                var customization = set.Customizations[i];
                Add(new Customization(customization.CustomizationData, customization.MainColor, customization.DetailColor,
                    customization.DetailSpritesIndex));
            }
        }

        /// <summary>
        /// Removes all Customizations which has data matching a CustomizationSet.
        /// </summary>
        /// <param name="set"></param>
        public void RemoveSet(CustomizationSet set)
        {
            for (int i = 0; i < set.Customizations.Length; i++)
            {
                var data = set.Customizations[i].CustomizationData;
                if (Contains(data))
                    Remove(data);
            }
        }

        /// <summary>
        /// Removes and hides a customization.
        /// </summary>
        /// <param name="data">The data that was used to create the customization</param>
        public void Remove(CustomizationData data)
        {
            if (_customizationDatas.ContainsKey(data) == false)
            {
                Debug.LogWarning($"Can't remove, no customization with {data} was found.");
                return;
            }

            var customization = _customizationDatas[data];
            RemoveCustomizationSprites(_customizationDatas[data]);
            _customizations.Remove(_customizationDatas[data]);
            _customizationDatas.Remove(data);
            _categories.Remove(data.Category);

            RemovedCustomization?.Invoke(customization);
        }

        /// <summary>
        /// Removes and hides a customization.
        /// </summary>
        /// <param name="customization">The data that was used to create the customization</param>
        public void Remove(Customization customization)
        {
            if (_customizationDatas.ContainsValue(customization) == false)
            {
                Debug.LogWarning($"Can't remove, no customization with {customization} was found.");
                return;
            }

            RemoveCustomizationSprites(customization);
            _customizations.Remove(customization);
            _customizationDatas.Remove(customization.CustomizationData);
            _categories.Remove(customization.CustomizationData.Category);

            RemovedCustomization?.Invoke(customization);
        }

        /// <summary>
        /// Hides and removes all customization that can be hidden.
        /// </summary>
        public void RemoveAll()
        {
            for (int i = _customizations.Count - 1; i >= 0; i--)
            {
                Remove(_customizations[i].CustomizationData);
            }
        }

        /// <summary>
        ///  Hides and removes all customization that can be hidden in a category.
        /// </summary>
        /// <param name="category"></param>
        public void RemoveAllInCategory(CustomizationCategory category)
        {
            if (_categories.ContainsKey(category) == false)
            {
                Debug.LogWarning($"Can't remove customization, category {category.name} not found.");
                return;
            }

            var customizationData = _categories[category].CustomizationData;
            Remove(customizationData);
        }

        /// <summary>
        /// Returns whether any customization created from a specific CustomizationData exists or not.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Contains(CustomizationData data) => _customizationDatas.ContainsKey(data);

        /// <summary>
        /// Returns whether customization is added or not.
        /// </summary>
        /// <param name="customization"></param>
        /// <returns></returns>
        public bool Contains(Customization customization) => _customizationDatas.ContainsValue(customization);

        /// <summary>
        /// Returns, if exists, a CustomizationData from a category.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public CustomizationData GetCustomizationDataInCategory(CustomizationCategory category)
        {
            if (_categories.ContainsKey(category) == false)
                return null;

            return _categories[category].CustomizationData;
        }

        /// <summary>
        /// Returns whether there is any customization existing in a category or not.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public bool HasCustomizationInCategory(CustomizationCategory category) => _categories.ContainsKey(category);

        /// <summary>
        /// Refreshes the whole class.
        /// </summary>
        public void Refresh()
        {
            GetAllComponents();
            ClearAllSlots();
            UpdateAllCustomizations();
            ApplyBodyColor();
        }

        /// <summary>
        /// Sets the main color of a customization created from a certain CustomizationData. 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="color"></param>
        public void SetCustomizationMainColor(CustomizationData data, Color color)
        {
            var customization = GetCustomizationWithData(data);
            if (customization == null)
                return;

            if (customization.CustomizationData.CanBeTinted == false)
            {
                Debug.Log(
                    $"{data.name} is set to not be able to tint color. If you want to allow this please enable CanBeTinted in the CustomizationData.",
                    data);
                return;
            }

            customization.SetMainColor(color);
            UpdateCustomizationSprites(_customizationDatas[data]);
        }

        /// <summary>
        /// Sets the detail color of a customization created from a certain CustomizationData. 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="color"></param>
        public void SetCustomizationDetailColor(CustomizationData data, Color color)
        {
            var customization = GetCustomizationWithData(data);
            if (customization == null)
                return;

            if (customization.CustomizationData.CanDetailsBeTinted == false)
            {
                Debug.Log(
                    $"{data.name} is set to not be able to tint details color. If you want to allow this please enable CanDetailsBeTinted in the CustomizationData.",
                    data);
                return;
            }

            if (data.UseMainColorForDetail)
            {
                Debug.Log(
                    $"{data.name} is set to use primary color for detail, so color will not be changed. If you want to allow this please disable UsePrimaryColorForDetail in the CustomizationData.",
                    data);
                return;
            }

            customization.SetDetailColor(color);
            UpdateCustomizationSprites(_customizationDatas[data]);
        }

        /// <summary>
        /// Sets the detail sprite index on a customization created from a certain CustomizationData. 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        public void SetCustomizationDetailIndex(CustomizationData data, int index)
        {
            var customization = GetCustomizationWithData(data);

            if (customization == null)
                return;

            customization.SetDetailSpriteIndex(index);
            UpdateCustomizationSprites(_customizationDatas[data]);
        }

        /// <summary>
        /// Gets the main color from a customization created from a certain CustomizationData. 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Color GetCustomizationMainColor(CustomizationData data)
        {
            var customization = GetCustomizationWithData(data);

            if (customization == null)
                return Color.white;

            return customization.MainColor;
        }

        /// <summary>
        /// Gets the detail color from a customization created from a certain CustomizationData. 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Color GetCustomizationDetailColor(CustomizationData data)
        {
            var customization = GetCustomizationWithData(data);

            if (customization == null)
                return Color.white;

            return customization.DetailColor;
        }

        /// <summary>
        /// Gets the detail sprite index from a customizatiom created from a certain CustomizationData. 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int GetCustomizationDetailSpritesIndex(CustomizationData data)
        {
            var customization = GetCustomizationWithData(data);

            if (customization == null)
                return 0;

            return customization.DetailSpritesIndex;
        }

        /// <summary>
        /// Gets a customization that was created from a certain CustomizationData.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Customization GetCustomizationWithData(CustomizationData data)
        {
            if (_customizationDatas.ContainsKey(data) == false)
            {
                Debug.LogWarning($"Can't find customization with set data {data}.");
                return null;
            }

            var customization = _customizationDatas[data];
            return customization;
        }

        /// <summary>
        /// Hides all customization that can be hidden. Note that this only visually hides the sprites, the customization will still exist.
        /// </summary>
        public void HideAll()
        {
            IsAllCustomizationHidden = true;
            UpdateVisibility();
        }

        /// <summary>
        /// Hides all slots with a location.
        /// </summary>
        /// <param name="location"></param>
        public void HideLocation(CustomizationLocation location)
        {
            var slots = GetSlotsWithLocation(location);

            for (int i = 0; i < slots.Length; i++)
            {
                var slot = slots[i];
                if (!slot.IsHidden)
                    slot.Hide();
            }
        }

        /// <summary>
        /// Hides the customization in a category.
        /// </summary>
        /// <param name="category"></param>
        public void HideCategory(CustomizationCategory category)
        {
            var customization = _categories[category];
            customization.SetIsHidden(true);
            UpdateCustomizationSprites(customization);
        }

        /// <summary>
        /// Shows all customization.
        /// </summary>
        public void ShowAll()
        {
            IsAllCustomizationHidden = false;
            UpdateVisibility();
        }

        /// <summary>
        /// Shows the customization in a category.
        /// </summary>
        /// <param name="category"></param>
        public void ShowCategory(CustomizationCategory category)
        {
            var customization = _categories[category];
            customization.SetIsHidden(false);
            UpdateCustomizationSprites(customization);
        }

        /// <summary>
        /// Shows all slots with a location.
        /// </summary>
        /// <param name="location"></param>
        public void ShowLocation(CustomizationLocation location)
        {
            var slots = GetSlotsWithLocation(location);

            for (int i = 0; i < slots.Length; i++)
            {
                var slot = slots[i];
                if (slot.IsHidden)
                    slot.Show();
            }
        }

        /// <summary>
        /// Changes the color of all the body parts.
        /// </summary>
        /// <param name="color"></param>
        public void SetBodyColor(Color color)
        {
            _bodyColor = color;
            ApplyBodyColor();
        }

        #endregion

        #region Private Methods

        private void ApplyBodyColor()
        {
            for (int i = 0; i < _bodyParts.Length; i++)
            {
                var bodyPart = _bodyParts[i];
                bodyPart.SetColor(_bodyColor);
            }
        }

        private void TryAddCustomizationToLookups(Customization customization)
        {
            if (_customizationDatas.ContainsKey(customization.CustomizationData) == false)
                _customizationDatas.Add(customization.CustomizationData, customization);

            if (_categories.ContainsKey(customization.CustomizationData.Category) == false)
                _categories.Add(customization.CustomizationData.Category, customization);
        }

        private Customization CreateCustomization(CustomizationData data)
        {
            var customization = new Customization(data);
            return customization;
        }

        private void UpdateVisibility()
        {
            if (IsAllCustomizationHidden)
            {
                for (int i = _customizations.Count - 1; i >= 0; i--)
                {
                    var customization = _customizations[i];
                    if (customization.CustomizationData.Category.CanBeHidden && RemoveCustomizationSprites(customization))
                        customization.SetIsHidden(true);
                }
            }
            else
            {
                for (int i = 0; i < _customizations.Count; i++)
                {
                    AddCustomizationToSlots(_customizations[i]);
                }
            }
        }

        private void UpdateCustomizationSprites(Customization customization)
        {
            if (customization.CustomizationData == null)
                return;

            if (customization.IsHidden)
                RemoveCustomizationSprites(customization);
            else
                AddCustomizationToSlots(customization);
        }

        private void UpdateAllCustomizations()
        {
            for (int i = 0; i < _customizations.Count; i++)
            {
                var customization = _customizations[i];

                if (customization.CustomizationData == null)
                {
                    Debug.LogWarning($"{gameObject.name} has missing {nameof(CustomizationData)}.", gameObject);
                    continue;
                }

                TryAddCustomizationToLookups(customization);
                UpdateCustomizationSprites(_customizations[i]);
            }
        }

        private CustomizationSlot[] GetCustomizationSlotsWithLocations(List<CustomizationLocation> location)
        {
            var result = new List<CustomizationSlot>();

            for (int i = 0; i < _downSlots.Length; i++)
            {
                if (location.Contains(_downSlots[i].Location))
                    result.Add(_downSlots[i]);
            }

            for (int i = 0; i < _sideSlots.Length; i++)
            {
                if (location.Contains(_sideSlots[i].Location))
                    result.Add(_sideSlots[i]);
            }

            for (int i = 0; i < _upSlots.Length; i++)
            {
                if (location.Contains(_upSlots[i].Location))
                    result.Add(_upSlots[i]);
            }

            return result.ToArray();
        }

        private CustomizationSlot[] GetSlotsWithLocation(CustomizationLocation location)
        {
            var result = new List<CustomizationSlot>();

            for (int i = 0; i < _downSlots.Length; i++)
            {
                if (location == _downSlots[i].Location)
                    result.Add(_downSlots[i]);
            }

            for (int i = 0; i < _sideSlots.Length; i++)
            {
                if (location == _sideSlots[i].Location)
                    result.Add(_sideSlots[i]);
            }

            for (int i = 0; i < _upSlots.Length; i++)
            {
                if (location == _upSlots[i].Location)
                    result.Add(_upSlots[i]);
            }

            return result.ToArray();
        }

        private void AddCustomizationToSlots(Customization customization)
        {
            for (int i = 0; i < customization.CustomizationData.SpriteSets.Length; i++)
            {
                var spriteSet = customization.CustomizationData.SpriteSets[i];
                var location = spriteSet.CustomizationLocation;
                SetSlotSprites(spriteSet, location, customization);
            }

            customization.SetIsHidden(false);


            for (int i = 0; i < customization.CustomizationData.LocationsToHide.Length; i++)
            {
                var locationToHide = customization.CustomizationData.LocationsToHide[i];
                HideLocation(locationToHide);
            }
        }

        private void SetSlotSprites(CustomizationSpriteSet spriteSet, CustomizationLocation location,
            Customization customization)
        {
            Sprite detailSpriteDown = null;
            Sprite detailSpriteSide = null;
            Sprite detailSpriteUp = null;
            var mainColor = customization.MainColor;

            Color detailColor;
            if (customization.CustomizationData.UseMainColorForDetail)
                detailColor = customization.MainColor;
            else
                detailColor = customization.DetailColor;


            if (spriteSet.HasDetailSprites)
            {
                detailSpriteDown = spriteSet.DetailSpriteSets[customization.DetailSpritesIndex].DownSprite;
                detailSpriteSide = spriteSet.DetailSpriteSets[customization.DetailSpritesIndex].SideSprite;
                detailSpriteUp = spriteSet.DetailSpriteSets[customization.DetailSpritesIndex].UpSprite;
            }

            var sortOvers = spriteSet.SortOverLocations;

            if (TryGetSlotWithLocation(location, _downSlots, out var slot)
                && IsFirstPriorityInSlot(slot, customization))
            {
                slot.SetSprites(spriteSet.DownSprite, detailSpriteDown);
                slot.SetColors(mainColor, detailColor);
                HandleSortOversForSlot(slot, sortOvers, _downSlots);
            }

            if (TryGetSlotWithLocation(location, _sideSlots, out slot)
                && IsFirstPriorityInSlot(slot, customization))
            {
                slot.SetSprites(spriteSet.SideSprite, detailSpriteSide);
                slot.SetColors(mainColor, detailColor);
                HandleSortOversForSlot(slot, sortOvers, _sideSlots);
            }

            if (TryGetSlotWithLocation(location, _upSlots, out slot)
                && IsFirstPriorityInSlot(slot, customization))
            {
                slot.SetSprites(spriteSet.UpSprite, detailSpriteUp);
                slot.SetColors(mainColor, detailColor);
                HandleSortOversForSlot(slot, sortOvers, _upSlots);
            }
        }

        private void HandleSortOversForSlot(CustomizationSlot slot, CustomizationLocation[] sortOver,
            CustomizationSlot[] slots)
        {
            if (sortOver != null && sortOver.Length > 0)
            {
                var slotWithHighestSortOrder = GetSlotWithHighestSortOrder(sortOver, slots);
                if (slotWithHighestSortOrder.SortOrder > slot.OriginalSortOrder)
                {
                    slot.SetSortOver(slotWithHighestSortOrder);
                    return;
                }
            }

            slot.SetSortOver(null);
        }

        private bool IsFirstPriorityInSlot(CustomizationSlot slot, Customization customization)
        {
            if (_slots.ContainsKey(slot) == false)
            {
                _slots.Add(slot, new List<Customization> { customization });
                return true;
            }

            var isNotAdded = _slots[slot].Contains(customization) == false;
            if (isNotAdded)
            {
                var priority = customization.CustomizationData.SlotPriority;
                var priorityIndex = 0;

                for (int i = 0; i < _slots[slot].Count; i++)
                {
                    if (priority >= _slots[slot][i].CustomizationData.SlotPriority)
                        continue;
                    priorityIndex++;
                }

                _slots[slot].Insert(priorityIndex, customization);
            }

            var isFirst = _slots[slot][0] == customization;
            return isFirst;
        }

        private int GetCountOfCustomizationsHidingLocation(CustomizationLocation location)
        {
            var count = 0;
            for (int i = 0; i < _customizations.Count; i++)
            {
                var customization = _customizations[i];

                for (int j = 0; j < customization.CustomizationData.LocationsToHide.Length; j++)
                {
                    if (customization.CustomizationData.LocationsToHide[j] == location)
                        count++;
                }
            }

            return count;
        }

        private bool RemoveCustomizationSprites(Customization customization)
        {
            for (int i = 0; i < customization.CustomizationData.LocationsToHide.Length; i++)
            {
                var hiddenLocation = customization.CustomizationData.LocationsToHide[i];
                if (GetCountOfCustomizationsHidingLocation(hiddenLocation) <= 1)
                    ShowLocation(hiddenLocation);
            }


            var locations = customization.CustomizationData.GetLocations();
            var slots = GetCustomizationSlotsWithLocations(locations);

            for (int i = 0; i < slots.Length; i++)
            {
                var slot = slots[i];

                if (_slots.ContainsKey(slot))
                    _slots[slot].Remove(customization);

                var isSlotSpriteFromCustomization = customization.CustomizationData.ContainSprite(slot.Sprite);

                if (!isSlotSpriteFromCustomization)
                    continue;

                var slotHasCustomizationToShow =
                    _slots.ContainsKey(slot) && _slots[slot].Count > 0 && !IsAllCustomizationHidden;
                if (slotHasCustomizationToShow)
                    AddCustomizationToSlots(_slots[slot][0]);
                else
                    slot.Clear();
            }

            return true;
        }

        private void GetAllComponents()
        {
            _bodyParts = GetComponentsInChildren<BodyPart>(true);

            if (_downRig != null)
                _downSlots = _downRig.GetComponentsInChildren<CustomizationSlot>(true);

            if (_sideRig != null)
                _sideSlots = _sideRig.GetComponentsInChildren<CustomizationSlot>(true);

            if (_upRig != null)
                _upSlots = _upRig.GetComponentsInChildren<CustomizationSlot>(true);
        }

        private bool TryGetSlotWithLocation(CustomizationLocation location, CustomizationSlot[] slots,
            out CustomizationSlot customizationSlot)
        {
            customizationSlot = null;

            if (slots == null || slots.Length == 0)
                return false;

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].Location == location)
                {
                    customizationSlot = slots[i];
                    return true;
                }
            }

            return false;
        }

        private CustomizationSlot GetSlotWithHighestSortOrder(CustomizationLocation[] locations, CustomizationSlot[] slots)
        {
            if (slots == null || slots.Length == 0)
                return null;
            CustomizationSlot highest = slots[0];
            for (int i = 0; i < slots.Length; i++)
            {
                var slot = slots[i];
                for (int j = 0; j < locations.Length; j++)
                {
                    var location = locations[j];
                    if (slot.Location == location && slot.OriginalSortOrder > highest.SortOrder)
                    {
                        highest = slot;
                    }
                }
            }

            return highest;
        }

        private void ClearAllSlots()
        {
            for (int i = 0; i < _downSlots.Length; i++)
            {
                var slot = _downSlots[i];
                slot.SetSprites(null, null);
                slot.SetColors(Color.white, Color.white);
                slot.Show();
            }

            for (int i = 0; i < _sideSlots.Length; i++)
            {
                var slot = _sideSlots[i];
                slot.SetSprites(null, null);
                slot.SetColors(Color.white, Color.white);
                slot.Show();
            }

            for (int i = 0; i < _upSlots.Length; i++)
            {
                var slot = _upSlots[i];
                slot.SetSprites(null, null);
                slot.SetColors(Color.white, Color.white);
                slot.Show();
            }

            _slots.Clear();
            _categories.Clear();
            _customizationDatas.Clear();
        }

#if UNITY_EDITOR
        private void DelayedOnValidate()
        {
            if (Application.isPlaying)
                return;

            EditorApplication.delayCall -= DelayedOnValidate;
            if (this != null)
                Refresh();
        }
#endif

        #endregion
    }
}