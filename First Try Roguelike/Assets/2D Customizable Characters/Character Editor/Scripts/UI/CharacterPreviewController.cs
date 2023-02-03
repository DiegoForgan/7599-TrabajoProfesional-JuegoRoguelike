using UnityEngine;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class CharacterPreviewController : MonoBehaviour
    {
        [Header("Positions")]
        [SerializeField] private Vector3 _characterPosition;
        [SerializeField] private Vector3 _downPreviewPosition;
        [SerializeField] private Vector3 _sidePreviewPosition;
        [SerializeField] private Vector3 _upPreviewPosition;

        [Header("References")]
        [SerializeField] private Canvas _canvas;

        private Vector3 _downOriginalPosition;
        private Vector3 _sideOriginalPosition;
        private Vector3 _upOriginalPosition;
        private bool _downOriginalActive;
        private bool _sideOriginalActive;
        private bool _upOriginalActive;
        private Vector3 _downOriginalScale;
        private Vector3 _sideOriginalScale;
        private Vector3 _upOriginalScale;
        private CustomizableCharacter _character;

        public void PreviewCharacter(CustomizableCharacter character)
        {
            _character = character;
            _character.transform.position = _characterPosition;

            // cache original active states
            _downOriginalActive = _character.DownRig.activeSelf;
            _sideOriginalActive = _character.SideRig.activeSelf;
            _upOriginalActive = _character.UpRig.activeSelf;

            // set all to active state
            _character.DownRig.SetActive(true);
            _character.SideRig.SetActive(true);
            _character.UpRig.SetActive(true);

            // cache original positions
            _downOriginalPosition = _character.DownRig.transform.localPosition;
            _sideOriginalPosition = _character.SideRig.transform.localPosition;
            _upOriginalPosition = _character.UpRig.transform.localPosition;

            // set all positions
            _character.DownRig.transform.localPosition = _downPreviewPosition;
            _character.SideRig.transform.localPosition = _sidePreviewPosition;
            _character.UpRig.transform.localPosition = _upPreviewPosition;
            
            // cache original scales
            _downOriginalScale = character.DownRig.transform.localScale;
            _sideOriginalScale = character.SideRig.transform.localScale;
            _upOriginalScale = character.UpRig.transform.localScale;

        }

        public void ResetCurrentCharacterPositions()
        {
            _character.transform.position = Vector3.zero;
            _character.DownRig.transform.localPosition = _downOriginalPosition;
            _character.SideRig.transform.localPosition = _sideOriginalPosition;
            _character.UpRig.transform.localPosition = _upOriginalPosition;
        }

        public void ResetCurrentCharacterScale()
        {
            _character.DownRig.transform.localScale = _downOriginalScale;
            _character.SideRig.transform.localScale = _sideOriginalScale;
            _character.UpRig.transform.localScale = _upOriginalScale;
        }

        public void ResetCurrentCharacterActiveStates()
        {
            _character.DownRig.SetActive(_downOriginalActive);
            _character.SideRig.SetActive(_sideOriginalActive);
            _character.UpRig.SetActive(_upOriginalActive);
        }

        private void OnDrawGizmosSelected()
        {
            if (_canvas == null)
                return;

            Gizmos.matrix = _canvas.transform.localToWorldMatrix;
            Gizmos.DrawWireSphere((_characterPosition + _downPreviewPosition) * _canvas.referencePixelsPerUnit, 10f);
            Gizmos.DrawWireSphere((_characterPosition + _sidePreviewPosition) * _canvas.referencePixelsPerUnit, 10f);
            Gizmos.DrawWireSphere((_characterPosition + _upPreviewPosition) * _canvas.referencePixelsPerUnit, 10f);
        }
    }
}