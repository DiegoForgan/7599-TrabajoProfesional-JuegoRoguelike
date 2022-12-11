using System;
using UnityEngine;
using UnityEngine.UI;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class VisibilityPanel : MonoBehaviour
    {
        [SerializeField] private Toggle[] _toggles = Array.Empty<Toggle>();
        private CustomizableCharacter _character;

        public void BindCharacter(CustomizableCharacter character) => _character = character;

        public void ResetToggles()
        {
            for (int i = 0; i < _toggles.Length; i++)
            {
                var toggle = _toggles[i];
                toggle.isOn = true;
            }
        }

        public void ShowShadows(bool show) => _character.SetHideShadows(!show);
        public void ShowWeaponEffects(bool show) => _character.SetHideWeaponEffects(!show);
        public void ShowWeapon(bool show) => _character.SetHideWeapons(!show);

        public void ShowAllCustomizations(bool show)
        {
            if (show)
                _character.Customizer.ShowAll();
            else
                _character.Customizer.HideAll();
        }

        public void InvokeToggleValues()
        {
            for (int i = 0; i < _toggles.Length; i++)
            {
                var toggle = _toggles[i];
                toggle.onValueChanged.Invoke(toggle.isOn);
            }
        }
    }
}