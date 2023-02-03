using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class CharacterPicker : MonoBehaviour
    {
        private List<CustomizableCharacter> _characterPrefabs = new List<CustomizableCharacter>();
        private List<CharacterEditorData> _characterEditorDatas = new List<CharacterEditorData>();

        [SerializeField] private Dropdown _dropdown;
        [SerializeField] private ConfirmationWindow _confirmationWindow;
        public CustomizableCharacter PickedCharacterInstance { get; private set; }
        private int _currentCharacterIndex;

        public event Action<CustomizableCharacter, CharacterEditorData> PickedCharacter;

        #region Unity Methods

        private void Start()
        {
            if (_characterPrefabs.Count == 0)
                return;

            BuildDropdown();
            _dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
            PickFirstOption();
        }

        private void OnDestroy()
        {
            _dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
        }

        #endregion

        #region Public Methods

        public void AddCharacter(CustomizableCharacter character, CharacterEditorData data)
        {
            var isAlreadyAdded = Contains(character);
            if (isAlreadyAdded)
            {
                var index = GetCharacterIndex(character);
                _characterPrefabs[index] = character;
                _characterEditorDatas[index] = data;
            }
            else
            {
                _characterPrefabs.Add(character);
                _characterEditorDatas.Add(data);
            }

            BuildDropdown();
        }

        public void SetCharacter(CustomizableCharacter character)
        {
            if (Contains(character) == false)
            {
                Debug.LogError($"Character {character.name} reference not found in list.");
                return;
            }

            var index = GetCharacterIndex(character);
            SetCharacterAtIndex(index);
        }

        public void SetCharacterAtIndex(int index)
        {
            if (PickedCharacterInstance != null)
            {
                var previousPicked = PickedCharacterInstance;
                // Unpick();
                Destroy(previousPicked.gameObject);
            }

            var character = _characterPrefabs[index];
            _dropdown.SetValueWithoutNotify(index);
            _currentCharacterIndex = index;

            PickedCharacterInstance = CreateCharacterInstance(character);
            PickedCharacterInstance.Customizer.Refresh();
            var data = _characterEditorDatas[index];
            PickedCharacter?.Invoke(PickedCharacterInstance, data);
        }

        public void Unpick()
        {
            PickedCharacter?.Invoke(null, null);
            PickedCharacterInstance = null;
        }

        public bool Contains(CustomizableCharacter character) => _characterPrefabs.Contains(character);
        public void RepickCurrentCharacter() => TryPickCharacterAtIndex(_currentCharacterIndex);

        public void RemoveCharacter(GameObject sibling)
        {
            _dropdown.Hide();
            var index = sibling.transform.GetSiblingIndex() - 1; // -1 since dropdown keeps the original content element
            _confirmationWindow.SelectedYes += () => RemoveCharacterAt(index);
            _confirmationWindow.Open("This action will remove the prefab (asset won't be deleted), are you sure?", true);
        }

        #endregion

        #region Private Methods

        private void RemoveCharacterAt(int index)
        {
            _characterEditorDatas[index].RemoveCustomizableCharacter(_characterPrefabs[index]);

            var currentCharacter = _characterPrefabs[_currentCharacterIndex];

            _characterPrefabs.RemoveAt(index);
            _characterEditorDatas.RemoveAt(index);

            BuildDropdown();

            if (_currentCharacterIndex == index)
            {
                _dropdown.SetValueWithoutNotify(0);
                SetCharacterAtIndex(0);
                _dropdown.RefreshShownValue();
            }
            else
            {
                var newCurrentIndex = _characterPrefabs.IndexOf(currentCharacter);
                _dropdown.SetValueWithoutNotify(newCurrentIndex);
                _dropdown.RefreshShownValue();
                SetCharacterAtIndex(newCurrentIndex);
            }
        }

        private void OnDropdownValueChanged(int index) => TryPickCharacterAtIndex(index);
        private int GetCharacterIndex(CustomizableCharacter character) => _characterPrefabs.IndexOf(character);

        private void TryPickCharacterAtIndex(int index)
        {
            _confirmationWindow.SelectedYes += () => SetCharacterAtIndex(index);
            _confirmationWindow.SelectedNo += () => _dropdown.SetValueWithoutNotify(_currentCharacterIndex);
            _confirmationWindow.Open("This action will remove all current customizations and settings, are you sure?");
        }

        private void PickFirstOption()
        {
            if (_dropdown.options.Count > 0)
            {
                SetCharacterAtIndex(0);
            }
        }

        private CustomizableCharacter CreateCharacterInstance(CustomizableCharacter character)
        {
#if UNITY_EDITOR
            if (Application.isEditor)
                return (CustomizableCharacter)PrefabUtility.InstantiatePrefab(character);
#endif
            return Instantiate(character);
        }

        private void BuildDropdown()
        {
            if (_characterPrefabs.Count == 0)
                return;

            _dropdown.ClearOptions();

            for (int i = _characterPrefabs.Count - 1; i >= 0; i--)
            {
                var prefab = _characterPrefabs[i];
                if (prefab == null)
                {
                    _characterPrefabs.RemoveAt(i);
                    continue;
                }

                var option = new Dropdown.OptionData(_characterPrefabs[i].name);
                _dropdown.options.Insert(0, option);
            }
        }

        #endregion
    }
}