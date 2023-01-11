using System.Collections.Generic;
using UnityEngine;

namespace CustomizableCharacters
{
    /// <summary>
    /// A customizable character and all it's parts.
    /// </summary>
    public class CustomizableCharacter : MonoBehaviour
    {
        [Header("Customizers")]
        [SerializeField] private Customizer _customizer;
        [SerializeField] private ScaleCustomizer _scaleCustomizer;

        [Header("Rigs")]
        [Tooltip("The GameObject that is considered down direction character rig")]
        [SerializeField] private GameObject _downRig;
        [Tooltip("The GameObject that is considered side direction character rig")]
        [SerializeField] private GameObject _sideRig;
        [Tooltip("The GameObject that is considered up direction character rig")]
        [SerializeField] private GameObject _upRig;

        [Space]
        [Tooltip("GameObjects that would be considered shadows. Used to toggle visibility of the shadows.")]
        [SerializeField] private GameObject[] _shadows;
        [Tooltip("GameObjects that would be considered weapons. Used to toggle visibility of the weapons.")]
        [SerializeField] private GameObject[] _weapons;

        private GameObject[] _weaponEffects;

        public Customizer Customizer => _customizer;
        public ScaleCustomizer ScaleCustomizer => _scaleCustomizer;
        public GameObject DownRig => _downRig;
        public GameObject SideRig => _sideRig;
        public GameObject UpRig => _upRig;

        public bool IsShadowsHidden { get; private set; }
        public bool IsWeaponEffectsHidden { get; private set; }
        public bool IsWeaponsHidden { get; private set; }

        private void Awake()
        {
            _customizer.Refresh();
        }

        /// <summary>
        /// Shows all the rigs by activating their GameObjects.
        /// </summary>
        public void ShowRigs()
        {
            _downRig.SetActive(true);
            _sideRig.SetActive(true);
            _upRig.SetActive(true);
        }

        /// <summary>
        /// Hides all the rigs by deactivating their GameObjects.
        /// </summary>
        public void HideRigs()
        {
            _downRig.SetActive(false);
            _sideRig.SetActive(false);
            _upRig.SetActive(false);
        }

        /// <summary>
        /// Applies data from a preset to all the customizers.
        /// </summary>
        /// <param name="preset"></param>
        public void ApplyPreset(CharacterPreset preset)
        {
            _customizer.SetBodyColor(preset.BodyColor);

            if (preset.Customizations.Length != 0)
                _customizer.RemoveAll();

            for (int i = 0; i < preset.Customizations.Length; i++)
            {
                var data = preset.Customizations[i].CustomizationData;
                var mainColor = preset.Customizations[i].MainColor;
                var detailColor = preset.Customizations[i].DetailColor;
                var detailIndex = preset.Customizations[i].DetailSpritesIndex;
                var newCustomization = new Customization(data, mainColor, detailColor, detailIndex);
                _customizer.Add(newCustomization);
            }

            if (preset.ScaleGroups.Length != 0)
                _scaleCustomizer.ResetAllGroups();

            for (int i = 0; i < preset.ScaleGroups.Length; i++)
            {
                var groupPreset = preset.ScaleGroups[i];
                var group = _scaleCustomizer.TryGetScaleGroup(groupPreset.GroupName);
                if (group == null)
                {
                    Debug.LogWarning(
                        $"Scale group {groupPreset.GroupName} was not found on the character. It was either saved from another character, rename or removed.");
                    continue;
                }

                group.SetScale(groupPreset.Scale);
                group.SetWidth(groupPreset.Width);
                group.SetLength(groupPreset.Length);
                _scaleCustomizer.ApplyGroup(group);
            }
        }

        /// <summary>
        /// Returns a preset with all the current settings of the character.
        /// </summary>
        /// <returns></returns>
        public CharacterPreset CreatePreset()
        {
            var preset = ScriptableObject.CreateInstance<CharacterPreset>();
            var customizations = _customizer.Customizations;
            var presetCustomizations = new Customization[customizations.Count];
            for (int i = 0; i < customizations.Count; i++)
            {
                var customization = customizations[i];
                presetCustomizations[i] = new Customization(customization.CustomizationData, customization.MainColor,
                    customization.DetailColor, customization.DetailSpritesIndex);
            }

            var scaleGroups = _scaleCustomizer.ScaleGroups;
            var presetScaleGroups = new ScaleGroupPreset[scaleGroups.Count];
            for (int i = 0; i < scaleGroups.Count; i++)
            {
                var scaleGroup = scaleGroups[i];
                presetScaleGroups[i] = new ScaleGroupPreset(scaleGroup.GroupName, scaleGroup.Scale, scaleGroup.Width,
                    scaleGroup.Length);
            }

            preset.SetBodyColor(_customizer.BodyColor);
            preset.SetCustomizations(presetCustomizations);
            preset.SetScaleGroups(presetScaleGroups);

            return preset;
        }

        /// <summary>
        /// Sets whether the shadows should be hidden or not.
        /// </summary>
        /// <param name="hide"></param>
        public void SetHideShadows(bool hide)
        {
            for (int i = 0; i < _shadows.Length; i++)
            {
                var shadow = _shadows[i];
                if (shadow != null)
                    shadow.gameObject.SetActive(!hide);
            }

            IsShadowsHidden = hide;
        }

        /// <summary>
        /// Sets whether the weapon effects in all the WeaponSlots should be hidden or not.
        /// </summary>
        /// <param name="hide"></param>
        public void SetHideWeaponEffects(bool hide)
        {
            if (_weaponEffects == null || _weaponEffects.Length == 0)
                CacheWeaponEffects();

            for (int i = 0; i < _weaponEffects.Length; i++)
            {
                var weaponEffect = _weaponEffects[i];
                if (weaponEffect != null)
                    weaponEffect.gameObject.SetActive(!hide);
            }

            IsWeaponEffectsHidden = hide;
        }

        /// <summary>
        /// Sets whether the weapons should be hidden or not.
        /// </summary>
        /// <param name="hide"></param>
        public void SetHideWeapons(bool hide)
        {
            if (_weapons == null || _weapons.Length == 0)
                return;

            for (int i = 0; i < _weapons.Length; i++)
            {
                var weapon = _weapons[i];
                if (weapon != null)
                    weapon.gameObject.SetActive(!hide);
            }

            IsWeaponsHidden = hide;
        }

        private void CacheWeaponEffects()
        {
            var weaponSlots = GetComponentsInChildren<WeaponSlot>();
            var effects = new List<GameObject>();

            for (int i = 0; i < weaponSlots.Length; i++)
            {
                var weaponSlot = weaponSlots[i];
                effects.Add(weaponSlot.SwingEffect.gameObject);
                effects.Add(weaponSlot.StabEffect.gameObject);
            }

            _weaponEffects = effects.ToArray();
        }
    }
}