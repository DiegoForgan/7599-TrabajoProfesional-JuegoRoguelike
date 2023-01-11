using System;
using CustomizableCharacters.CharacterEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace CustomizableCharacters.CharacterEditor
{
    public class CharacterDemo : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _stopButton;
        [SerializeField] private CharacterPicker _characterPicker;
        private Camera _camera;

        private ICharacterController _controller;
        private Transform _cameraParent;
        private Transform _previousCameraParent;
        private Vector3 _previousLocalCameraPosition;

        public event Action Starting;
        public event Action Stopped;

        private void Awake()
        {
            _canvas.enabled = false;
            _playButton.onClick.AddListener(StartDemo);
            _stopButton.onClick.AddListener(StopDemo);
            _characterPicker.PickedCharacter += OnCharacterPickerPickedCharacter;
        }

        private void OnDestroy()
        {
            _playButton.onClick.RemoveListener(StartDemo);
            _stopButton.onClick.RemoveListener(StopDemo);
            _characterPicker.PickedCharacter -= OnCharacterPickerPickedCharacter;
        }

        private void OnCharacterPickerPickedCharacter(CustomizableCharacter character, CharacterEditorData data)
        {
            if (_controller != null)
                _controller.EnableController();

            if (character == null)
            {
                SetPlayButtonEnabled(false);
                return;
            }

            _cameraParent = character.transform;
            _controller = character.GetComponentInChildren<ICharacterController>();
            if (_controller != null)
                _controller.DisableController();

            SetPlayButtonEnabled(_controller != null);
        }

        private void SetPlayButtonEnabled(bool isEnabled)
        {
            _playButton.interactable = isEnabled;
        }

        private void StartDemo()
        {
            Starting?.Invoke();
            _controller.EnableController();
            _canvas.enabled = true;

            _camera = Camera.main;
            _previousLocalCameraPosition = _camera.transform.localPosition;
            _previousCameraParent = _camera.transform.parent;
            _camera.transform.parent = _cameraParent;
        }

        private void StopDemo()
        {
            _controller.DisableController();
            _canvas.enabled = false;
            _camera.transform.parent = _previousCameraParent;
            _camera.transform.localPosition = _previousLocalCameraPosition;
            Stopped?.Invoke();
        }
    }
}