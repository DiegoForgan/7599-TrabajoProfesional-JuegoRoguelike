using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class ConfirmationWindow : MonoBehaviour
    {
        [SerializeField] private Toggle _dontShowAgainToggle;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Canvas _windowCanvas;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Text _text;

        private Coroutine _coroutine;

        public event Action SelectedYes;
        public event Action SelectedNo;

        private const float FadeDuration = 0.4f;
        private const float BlockerAlpha = 0.4f;

        private void Awake()
        {
            _canvas.enabled = false;
            _windowCanvas.enabled = false;
        }

        public void SelectYes()
        {
            SelectedYes?.Invoke();
            Close();
        }

        public void SelectNo()
        {
            SelectedNo?.Invoke();
            Close();
        }

        public void Open(string text, bool alwaysAsk = false)
        {
            if (_dontShowAgainToggle.isOn && !alwaysAsk)
            {
                SelectYes();
                return;
            }

            _dontShowAgainToggle.gameObject.SetActive(!alwaysAsk);
            _text.text = text;

            if (_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(OpenWindow());
        }

        private void Close()
        {
            SelectedYes = null;
            SelectedNo = null;

            if (_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(CloseWindow());
        }

        private IEnumerator OpenWindow()
        {
            _canvas.enabled = true;
            _windowCanvas.enabled = true;

            var color = _backgroundImage.color;
            color.a = 0;
            var increment = 1 / FadeDuration;
            while (color.a < BlockerAlpha)
            {
                color.a += increment * Time.deltaTime;
                _backgroundImage.color = color;
                yield return null;
            }

            color.a = BlockerAlpha;
            _backgroundImage.color = color;
        }

        private IEnumerator CloseWindow()
        {
            _windowCanvas.enabled = false;

            var color = _backgroundImage.color;
            var increment = 1 / FadeDuration;
            while (color.a > 0)
            {
                color.a -= increment * Time.deltaTime;
                _backgroundImage.color = color;
                yield return null;
            }

            _canvas.enabled = false;
        }
    }
}