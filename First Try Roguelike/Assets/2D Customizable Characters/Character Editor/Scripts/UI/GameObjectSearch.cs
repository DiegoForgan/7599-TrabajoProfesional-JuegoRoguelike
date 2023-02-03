using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class GameObjectSearch : MonoBehaviour
    {
        [SerializeField] private GameObject _hierarchyTop;
        [SerializeField] private InputField _searchInputField;
        [SerializeField] private List<string> _alwaysDeactivatedNames = new List<string>();
        [SerializeField] private bool _shouldSearchOnEnabled;

        private void Awake()
        {
            _searchInputField.onValueChanged.AddListener(Search);
            _searchInputField.onEndEdit.AddListener(Search);
        }

        private void OnDestroy()
        {
            _searchInputField.onValueChanged.RemoveListener(Search);
            _searchInputField.onEndEdit.RemoveListener(Search);
        }

        private void OnEnable()
        {
            if (_shouldSearchOnEnabled)
                Search(_searchInputField.text);
        }

        private void Search(string searchString)
        {
            var children = _hierarchyTop.transform.childCount;
            var isSearchEmpty = string.IsNullOrEmpty(searchString);
            searchString = searchString.ToLower();

            for (int i = 0; i < children; i++)
            {
                var child = _hierarchyTop.transform.GetChild(i);
                if (_alwaysDeactivatedNames.Contains(child.gameObject.name))
                {
                    child.gameObject.SetActive(false);
                    continue;
                }

                if (isSearchEmpty)
                {
                    child.gameObject.SetActive(true);
                }
                else
                {
                    var nameContainsSearch = child.gameObject.name.ToLower().Contains(searchString);
                    child.gameObject.SetActive(nameContainsSearch);
                }
            }
        }
    }
}