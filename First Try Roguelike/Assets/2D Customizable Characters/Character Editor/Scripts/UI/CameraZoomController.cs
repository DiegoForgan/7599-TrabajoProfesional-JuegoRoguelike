using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class CameraZoomController : MonoBehaviour
    {
        [SerializeField] private float[] _zoomLevels;
        [SerializeField] private Text _zoomText;

        [SerializeField] private Button _zoomInButton;
        [SerializeField] private Button _zoomOutButton;
        [SerializeField] private int _startZoomLevelIndex;
        private Camera _camera;
        private float _startCameraSize;
        private int _currentZoomLevelIndex;
        private Coroutine _coroutine;

        private void OnEnable()
        {
            _camera = Camera.main;
            _startCameraSize = _camera.orthographicSize;
            _currentZoomLevelIndex = _startZoomLevelIndex;
            UpdateCameraSize();
            UpdateButtons();
            UpdateZoomLabel();

            _zoomInButton.onClick.AddListener(ZoomIn);
            _zoomOutButton.onClick.AddListener(ZoomOut);
        }

        private void OnDisable()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            if (_camera != null)
                _camera.orthographicSize = _startCameraSize;

            _zoomInButton.onClick.RemoveListener(ZoomIn);
            _zoomOutButton.onClick.RemoveListener(ZoomOut);
        }

        public void ZoomIn()
        {
            if (_zoomLevels.Length - 1 < _currentZoomLevelIndex + 1)
                return;

            _currentZoomLevelIndex++;
            UpdateCameraSize();
            UpdateButtons();
            UpdateZoomLabel();
        }

        public void ZoomOut()
        {
            if (_currentZoomLevelIndex - 1 < 0)
                return;

            _currentZoomLevelIndex--;
            UpdateCameraSize();
            UpdateButtons();
            UpdateZoomLabel();
        }

        private void UpdateCameraSize()
        {
            var size = _zoomLevels[_currentZoomLevelIndex];
            size = _startCameraSize / size;


            if (_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(DoCameraZoom(size));
        }

        private IEnumerator DoCameraZoom(float targetSize)
        {
            var time = 0f;
            var startSize = _camera.orthographicSize;
            var zoomDuration = 0.3f;
            while (time <= zoomDuration)
            {
                var size = Mathf.Lerp(startSize, targetSize, Mathf.Sin(time / zoomDuration * (Mathf.PI / 2)));
                _camera.orthographicSize = size;
                yield return null;
                time += Time.deltaTime;
            }

            _camera.orthographicSize = targetSize;
        }

        private void UpdateButtons()
        {
            var isLastZoomLevel = _currentZoomLevelIndex == _zoomLevels.Length - 1;
            _zoomInButton.interactable = !isLastZoomLevel;

            var isFirstZoomLevel = _currentZoomLevelIndex == 0;
            _zoomOutButton.interactable = !isFirstZoomLevel;
        }

        private void UpdateZoomLabel()
        {
            var zoomLevel = (_zoomLevels[_currentZoomLevelIndex] * 100).ToString() + "%";
            _zoomText.text = zoomLevel;
        }
    }
}