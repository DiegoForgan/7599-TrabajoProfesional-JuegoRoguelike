using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class EnterExitEventTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private UnityEvent PointerEnter;
        [SerializeField] private UnityEvent PointerExit;
        public void OnPointerEnter(PointerEventData eventData)
        {
            PointerEnter?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PointerExit?.Invoke();
        }
    }
}