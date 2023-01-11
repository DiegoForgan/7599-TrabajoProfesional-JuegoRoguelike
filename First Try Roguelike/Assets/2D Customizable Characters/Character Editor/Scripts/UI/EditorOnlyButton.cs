using UnityEngine;
using UnityEngine.UI;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class EditorOnlyButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;
        [SerializeField] private bool _disableGameObject;

        private void Awake()
        {
            if (!Application.isEditor)
            {
                if (_button != null)
                {
                    _button.interactable = false;
                    gameObject.SetActive(!_disableGameObject);
                }
            }

            if (_image != null)
                _image.enabled = !Application.isEditor;
        }
    }
}