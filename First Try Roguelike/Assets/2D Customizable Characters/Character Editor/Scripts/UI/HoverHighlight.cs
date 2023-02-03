using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class HoverHighlight : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
    {
        [SerializeField] private Image _highlightedImage;

        private void Awake()
        {
            _highlightedImage.enabled = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _highlightedImage.enabled = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _highlightedImage.enabled = false;
        }
    }
}