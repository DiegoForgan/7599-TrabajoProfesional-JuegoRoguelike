using System;
using UnityEngine;
using UnityEngine.UI;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class GroupButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        public event Action<int> Clicked;
        private int _groupIndex;

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        public void SetGroupIndex(int index)
        {
            _groupIndex = index;
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            Clicked?.Invoke(_groupIndex);
        }
    }
}