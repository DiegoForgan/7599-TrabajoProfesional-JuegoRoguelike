using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class ScaleTab : CategoryTab
    {
        [FormerlySerializedAs("_groupPrefab")]
        [Header("Groups")]
        [SerializeField] private ScaleGroupButton _groupButtonPrefab;
        [SerializeField] private Transform _groupParent;

        private ScaleCustomizer _scaleCustomizer;
        private readonly List<ScaleGroupButton> _scaleGroups = new List<ScaleGroupButton>();
        private float _scaleDeviation;

        protected override void Awake()
        {
            base.Awake();
            _groupButtonPrefab.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            DestroyButtons();
        }

        public void Bind(ScaleCustomizer scaleCustomizer, float scaleDeviation)
        {
            _scaleCustomizer = scaleCustomizer;
            _scaleDeviation = scaleDeviation;
            CreateButtons();
        }

        public void Unbind()
        {
            _scaleCustomizer = null;
            DestroyButtons();
        }

        protected override void ShowButtons()
        {
            if (_scaleGroups.Count == 0)
                return;

            for (int i = 0; i < _scaleGroups.Count; i++)
            {
                _scaleGroups[i].gameObject.SetActive(true);
            }

            SelectFirstScaleGroup();
        }

        protected override void HideButtons()
        {
            if (_scaleGroups.Count == 0)
                return;

            for (int i = 0; i < _scaleGroups.Count; i++)
            {
                _scaleGroups[i].gameObject.SetActive(false);
            }

            UnselectAll();
        }

        protected override void HideOptions()
        {
            // ...
        }

        private void SelectFirstScaleGroup()
        {
            _scaleGroups[0].GetComponent<Toggle>().isOn = false;
            _scaleGroups[0].GetComponent<Toggle>().isOn = true;
        }

        private void UnselectAll() => _scaleGroups[0].GetComponent<Toggle>().group.SetAllTogglesOff();
        private void OnScaleGroupValueChanged(ScaleGroup scaleGroup) => _scaleCustomizer.ApplyGroup(scaleGroup);

        private void CreateButtons()
        {
            var scaleGroups = _scaleCustomizer.ScaleGroups;

            for (int i = 0; i < scaleGroups.Count; i++)
            {
                var group = Instantiate(_groupButtonPrefab, _groupParent);
                group.gameObject.SetActive(false);
                group.Bind(scaleGroups[i], _scaleDeviation);
                group.ScaleGroupValueChanged += OnScaleGroupValueChanged;
                _scaleGroups.Add(group);
            }
        }

        private void DestroyButtons()
        {
            for (int i = 0; i < _scaleGroups.Count; i++)
            {
                var group = _scaleGroups[i];
                group.ScaleGroupValueChanged -= OnScaleGroupValueChanged;
                Destroy(group.gameObject);
            }

            _scaleGroups.Clear();
        }
    }
}