using UnityEngine;
using UnityEngine.UI;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class ButtonDropdown : MonoBehaviour
    {
        [SerializeField] protected Toggle _toggle;
        [SerializeField] protected GameObject _dropdown;
        [SerializeField] private Button _blocker;
        [SerializeField] private Canvas _canvas;

        protected virtual void Awake()
        {
            _toggle.onValueChanged.AddListener(OnToggleValueChanged);
            _toggle.onValueChanged.AddListener(_dropdown.SetActive);
            _dropdown.SetActive(false);
        }

        protected virtual void OnDestroy()
        {
            _toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            _toggle.onValueChanged.RemoveListener(_dropdown.SetActive);
        }

        private void OnToggleValueChanged(bool isOn)
        {
            if (isOn)
                OpenDropdown();
            else
                CloseDropdown();
        }

        private void OpenDropdown()
        {
            transform.SetAsLastSibling();
            EnableBlocker();
            _canvas.overrideSorting = true;
            _canvas.sortingOrder = 510;
        }

        private void CloseDropdown()
        {
            _blocker.onClick.RemoveListener(CloseDropdown);
            _blocker.gameObject.SetActive(false);
            _toggle.isOn = false;
            _canvas.overrideSorting = false;
        }

        private void EnableBlocker()
        {
            _blocker.transform.SetParent(transform.parent);
            _blocker.transform.SetSiblingIndex(transform.parent.childCount - 2);
            _blocker.onClick.AddListener(CloseDropdown);
            _blocker.gameObject.SetActive(true);
        }
    }
}