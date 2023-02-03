using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class InfoTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [TextArea]
        [SerializeField] private string _information;
        [SerializeField] private Text _infoText;
        [SerializeField] private Canvas _tooltipCanvas;

        private void Awake()
        {
            _infoText.text = _information;
            _tooltipCanvas.enabled = false;
        }

        private void OnValidate()
        {
            _infoText.text = _information;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _tooltipCanvas.enabled = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _tooltipCanvas.enabled = false;
        }
    }
}