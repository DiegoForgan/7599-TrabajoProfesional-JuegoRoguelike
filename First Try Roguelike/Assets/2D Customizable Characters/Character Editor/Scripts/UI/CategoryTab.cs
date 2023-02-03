using UnityEngine;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public abstract class CategoryTab : MonoBehaviour
    {
        [SerializeField] private Canvas _customizationCanvas;
        protected bool _isShowing;
        
        protected virtual void Awake()
        {
            SetShow(false);
        }

        public virtual void SetShow(bool show)
        {
            if (show)
            {
                _customizationCanvas.enabled = true;
                ShowButtons();
            }
            else
            {
                _customizationCanvas.enabled = false;
                HideButtons();
                HideOptions();
            }

            _isShowing = show;
        }

        protected abstract void ShowButtons();
        protected abstract void HideButtons();
        protected abstract void HideOptions();
    }
}