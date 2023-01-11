using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomizableCharacters.CharacterEditor.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CustomizableCharacters.CharacterEditor
{
    public class SpriteExporter : MonoBehaviour
    {
        [Header("Frames")]
        [SerializeField] private InputField _framesInput;
        [SerializeField] private Dropdown _frameCaptureOptionDropdown;
        [SerializeField] private Dropdown _backgroundColorOptionsDropdown;
        [SerializeField] private BackgroundColorOption[] _backgroundColorOptions = Array.Empty<BackgroundColorOption>();

        [Header("Sprite Sheet")]
        [SerializeField] private InputField _framesPerRowInputField;
        [SerializeField] private InputField _framesPerColumnInputField;
        [SerializeField] private Text _spriteSheetResolutionText;
        [SerializeField] private InputField _filePrefixInputField;

        [Header("Animation Clips")]
        [SerializeField] private Toggle _clipTogglePrefab;
        [SerializeField] private Transform _clipTogglesParent;

        [Header("References")]
        [SerializeField] private Button _exportButton;

        [SerializeField] private CaptureAreaBounds _captureAreaBounds;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private GameObject[] _gameObjectsToDeactivate;
        [SerializeField] private Text _amountOfSelectedClipsText;

        private readonly Dictionary<Toggle, SpriteExporterClip> _clipToggles = new Dictionary<Toggle, SpriteExporterClip>();
        private readonly List<GameObject> _hiddenGameObjects = new List<GameObject>();

        private Animation _animation;
        private Camera _captureCamera;
        private Animator _characterAnimator;
        private ClipToSpriteExporter _clipToSpriteExporter;
        private AnimationClip[] _defaultPoseClips;
        private bool _isOverwritingAll;
        private GameObject _previewRig;
        private string _savePath;
        private int _amountOfSelectedClips;

        private enum FramesCaptureOption
        {
            FramesPerClip = 0,
            FramesPerSecond = 1
        }

        #region Unity Methods

        private void Awake()
        {
            _clipTogglePrefab.gameObject.SetActive(false);
            _canvas.enabled = false;
            _framesPerRowInputField.onValueChanged.AddListener(x => UpdateSpriteSheetOptions());
            _framesPerColumnInputField.onValueChanged.AddListener(x => UpdateSpriteSheetOptions());
            _captureAreaBounds.ResolutionChanged += UpdateSpriteSheetOptions;
            UpdateSpriteSheetOptions();
        }

        private void OnDestroy()
        {
            _framesPerRowInputField.onValueChanged.RemoveListener(x => UpdateSpriteSheetOptions());
            _framesPerColumnInputField.onValueChanged.RemoveListener(x => UpdateSpriteSheetOptions());
            _captureAreaBounds.ResolutionChanged -= UpdateSpriteSheetOptions;
            Unbind();
        }

        #endregion

        #region Public Methods

        public void Open()
        {
            _canvas.enabled = true;
            _captureAreaBounds.SetHidden(false);

            for (var i = 0; i < _gameObjectsToDeactivate.Length; i++)
            {
                var go = _gameObjectsToDeactivate[i];
                if (go.activeSelf)
                {
                    go.SetActive(false);
                    _hiddenGameObjects.Add(go);
                }
            }
        }

        public void Close()
        {
            _canvas.enabled = false;
            _captureAreaBounds.SetHidden(true);

            for (var i = 0; i < _hiddenGameObjects.Count; i++)
            {
                var go = _hiddenGameObjects[i];
                go.SetActive(true);
            }
        }

        public void Bind(SpriteExporterClip[] clips, AnimationClip[] defaultPoseClips, Animator animator)
        {
            _captureCamera = Camera.main;
            _characterAnimator = animator;
            if (_characterAnimator != null)
                _characterAnimator.enabled = false;
            _defaultPoseClips = defaultPoseClips;
            CreateClipButtons(clips);
            UpdateAmountOfClipsSelected();
            CreateAnimation();
            CreateBackgroundColorDropdownOptions();
            SetPreviewRigShowing(true);
            _captureAreaBounds.UpdateBounds();
            // _captureAreaBounds.CreateBoundsFromClips(clips, _animation);
            SampleDefaultPoses();
        }

        public void Unbind()
        {
            DestroyImmediate(_animation);
            DestroyClipToggles();
            StopAllCoroutines();
            ClearProgressBar();

            if (_characterAnimator != null)
                _characterAnimator.enabled = true;
        }

        // public void UpdateBoundsWithSelectedClips()
        // {
        //     var clips = new List<SpriteExporterClip>();
        //     foreach (var clipToggle in _clipToggles)
        //         if (clipToggle.Key.isOn)
        //             clips.Add(clipToggle.Value);
        //
        //     if (clips.Count == 0)
        //     {
        //         Debug.Log("No clips selected.");
        //         return;
        //     }
        //
        //     // _captureAreaBounds.CreateBoundsFromClips(clips.ToArray(), _animation);
        //     SampleDefaultPoses();
        // }

        public void StartExport()
        {
            StopAllCoroutines();
            StartCoroutine(DoExport());
        }

        #endregion

        #region Scene

        private void SetPreviewRigShowing(bool show)
        {
            if (_previewRig == null && _clipToggles.Count > 0)
                _previewRig = _clipToggles.FirstOrDefault().Value.RigGameObject;

            if (_previewRig != null) _previewRig.SetActive(show);
        }

        private void SetSceneHidden(bool hide)
        {
            _captureAreaBounds.SetHidden(hide);
            SetPreviewRigShowing(!hide);
            _canvas.rootCanvas.enabled = !hide;
        }

        #endregion

        #region Animation Clips

        private void CreateClipButtons(SpriteExporterClip[] clips)
        {
            _amountOfSelectedClips = 0;
            var sortedClips = clips.OrderBy(x => x.AnimationClip.name).ToArray();
            for (var i = 0; i < sortedClips.Length; i++)
            {
                var clip = sortedClips[i];
                var toggle = CreateClipToggle(clip.AnimationClip.name);
                _clipToggles.Add(toggle, clip);
                toggle.onValueChanged.AddListener(OnClipToggleChangedValue);
            }
        }

        private void OnClipToggleChangedValue(bool isOn)
        {
            if (isOn)
                _amountOfSelectedClips += 1;
            else
                _amountOfSelectedClips -= 1;

            _amountOfSelectedClipsText.text = _amountOfSelectedClips.ToString();
            UpdateExportButton();
        }

        private void UpdateExportButton()
        {
            if (Application.isEditor)
                _exportButton.interactable = _amountOfSelectedClips != 0;
        }

        private void UpdateAmountOfClipsSelected()
        {
            var selected = 0;
            foreach (var toggle in _clipToggles.Keys)
            {
                if (toggle.isOn)
                    selected += 1;
            }

            _amountOfSelectedClipsText.text = selected.ToString();
            UpdateExportButton();
        }

        private void CreateAnimation()
        {
            _animation = _characterAnimator?.gameObject.AddComponent<Animation>();
        }

        private AnimationClip CreateLegacyClip(AnimationClip clip)
        {
            var legacyClip = Instantiate(clip);
            legacyClip.legacy = true;
            legacyClip.wrapMode = WrapMode.Loop;
            legacyClip.name = legacyClip.name.Replace("(Clone)", "");
            return legacyClip;
        }

        private void SampleDefaultPoses()
        {
            for (var i = 0; i < _defaultPoseClips.Length; i++)
            {
                var clip = _defaultPoseClips[i];
                if (clip == null)
                    continue;

                clip.SampleAnimation(_characterAnimator?.gameObject, 0);
            }
        }

        private Toggle CreateClipToggle(string clipName)
        {
            var toggle = Instantiate(_clipTogglePrefab, _clipTogglesParent);
            toggle.GetComponentInChildren<Text>().text = clipName;
            toggle.gameObject.name = clipName;
            toggle.gameObject.SetActive(true);
            return toggle;
        }

        private void DestroyClipToggles()
        {
            foreach (var clipToggle in _clipToggles) Destroy(clipToggle.Key.gameObject);

            _clipToggles.Clear();
        }

        private AnimationState CreateAnimationStateForCapture(string clipName)
        {
            var state = _animation[clipName];
            state.enabled = true;
            state.weight = 1;
            state.speed = 0;

            return state;
        }

        #endregion

        #region UI

        private void UpdateSpriteSheetOptions()
        {
            var resolutionPerFrame = _captureAreaBounds.Resolution;

            var x = int.Parse(_framesPerRowInputField.text) * resolutionPerFrame.x;
            var y = int.Parse(_framesPerColumnInputField.text) * resolutionPerFrame.y;
            _spriteSheetResolutionText.text = $"{x} x {y} px";
        }

        private void CreateBackgroundColorDropdownOptions()
        {
            var options = new List<Dropdown.OptionData>();
            for (var i = 0; i < _backgroundColorOptions.Length; i++)
            {
                var colorOption = _backgroundColorOptions[i];
                options.Add(new Dropdown.OptionData(colorOption.ColorName));
            }

            _backgroundColorOptionsDropdown.options = options;
            _backgroundColorOptionsDropdown.value = 0;
        }

        #endregion

        #region Windows

        private void DisplayProgressBar(string title, string info, float progress)
        {
#if UNITY_EDITOR
            EditorUtility.DisplayProgressBar(title, info, progress);
#endif
        }

        private void ClearProgressBar()
        {
#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif
        }

        private int DisplayOverwriteWindow(string path)
        {
            var option = 0;
#if UNITY_EDITOR
            option = EditorUtility.DisplayDialogComplex("Overwrite?",
                $"This file already exists\n {path}\n do you want to overwrite it?",
                "Yes",
                "No",
                "Yes to all");
#endif
            return option;
        }

        #endregion

        #region Capture

        private IEnumerator DoExport()
        {
            SetSceneHidden(true);
            _savePath = PlayerPrefs.GetString("SpriteExporterSavePath");
            string savePath = null;

            savePath = OpenSaveFolderPanel("Select a folder to save sprites...", _savePath, "");

            if (string.IsNullOrEmpty(savePath))
            {
                FinishExporting();
                yield break;
            }

            _isOverwritingAll = false;

            PlayerPrefs.SetString("SpriteExporterSavePath", savePath);
            _savePath = savePath + "/";

            var selectedClips = new List<SpriteExporterClip>();
            foreach (var clipToggle in _clipToggles)
                if (clipToggle.Key.isOn)
                    selectedClips.Add(clipToggle.Value);

            for (var i = 0; i < selectedClips.Count; i++)
            {
                var clipToExport = selectedClips[i];
                DisplayProgressBar("Creating Sprites",
                    $"Creating and saving sprites from {clipToExport.AnimationClip.name}...",
                    i / selectedClips.Count);

                yield return ExportClip(clipToExport);
                yield return null;
            }

            ClearProgressBar();

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
            Debug.Log("Exporting done!");


            FinishExporting();
        }

        private void FinishExporting()
        {
            SetSceneHidden(false);
        }

        private void PrepareSpriteExporter(string savePath)
        {
            if (_clipToSpriteExporter == null)
                _clipToSpriteExporter = new ClipToSpriteExporter();

            var resolution = _captureAreaBounds.Resolution;
            var rows = int.Parse(_framesPerColumnInputField.text);
            var columns = int.Parse(_framesPerRowInputField.text);
            var colorIndex = _backgroundColorOptionsDropdown.value;
            var backgroundColor = _backgroundColorOptions[colorIndex].Color;
            _clipToSpriteExporter.Prepare(resolution, backgroundColor, rows, columns, savePath);
        }

        private IEnumerator ExportClip(SpriteExporterClip exportClip)
        {
            var clip = CreateLegacyClip(exportClip.AnimationClip);
            var clipName = clip.name;
            var fileName = _filePrefixInputField.text + clipName;
            PrepareSpriteExporter(_savePath + fileName);

            var fullPath = _clipToSpriteExporter.GetFullSavePath();
            var doesFileExist = File.Exists(fullPath);
            if (doesFileExist && !_isOverwritingAll)
            {
                var option = DisplayOverwriteWindow(fullPath);

                switch (option)
                {
                    case 0: break;
                    case 1:
                        _clipToSpriteExporter.Complete();
                        yield break;
                    case 2:
                        _isOverwritingAll = true;
                        break;
                    default:
                        Debug.LogError("Unrecognized option.");
                        break;
                }
            }

            exportClip.RigGameObject.SetActive(true);
            _animation.AddClip(clip, clipName);
            var numberOfFrames = 0;
            var timeIncrement = 0f;
            var frames = int.Parse(_framesInput.text);
            var framesCaptureOption = (FramesCaptureOption)_frameCaptureOptionDropdown.value;

            switch (framesCaptureOption)
            {
                case FramesCaptureOption.FramesPerClip:
                    timeIncrement = clip.length / frames;
                    numberOfFrames = frames;
                    break;
                case FramesCaptureOption.FramesPerSecond:
                    timeIncrement = 1f / frames;
                    numberOfFrames = (int)(clip.length / timeIncrement);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // capture frames
            SampleDefaultPoses();
            var state = CreateAnimationStateForCapture(clipName);
            for (var i = 0; i < numberOfFrames; i++)
            {
                state.time = timeIncrement * i;
                _animation.Sample();
                yield return null; // wait one frame here so any state changed from animation has time visually update
                _clipToSpriteExporter.CaptureFrame(_captureAreaBounds.Bounds, _captureCamera);
                _clipToSpriteExporter.TrySaveAnimationFrame(i + 1);
            }

            // clean up
            _clipToSpriteExporter.Complete();
            _animation.RemoveClip(clip.name);
            exportClip.RigGameObject.SetActive(false);
            SampleDefaultPoses();
        }

        private string OpenSaveFolderPanel(string title, string folder, string defaultName)
        {
#if UNITY_EDITOR
            if (Application.isEditor)
                return EditorUtility.SaveFolderPanel(title, folder, defaultName);
#endif
            return string.Empty;
        }

        #endregion
    }
}