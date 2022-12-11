using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
#endif
using UnityEngine.UI;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class CharacterEditorManager : MonoBehaviour
    {
        [SerializeField] private CharacterEditorSettings _settings;

        [Header("UI")]
        [SerializeField] private Toggle _firstTabToggle;

        [Header("References")]
        [SerializeField] private ColorOptionButton _bodyColorOptionButton;
        [SerializeField] private ConfirmationWindow _confirmationWindow;
        [SerializeField] private CharacterPicker _characterPicker;
        [SerializeField] private CharacterPreviewController _previewController;
        [SerializeField] private CustomizationTab _equipmentTab;
        [SerializeField] private CustomizationTab _appearanceTab;
        [SerializeField] private ScaleTab _scaleTab;
        [SerializeField] private AnimationPlayer _animationPlayer;
        [SerializeField] private CharacterDemo _characterDemo;
        [SerializeField] private SpriteExporter _spriteExporter;
        [SerializeField] private CameraZoomController _cameraZoomController;
        [SerializeField] private UICharacterPresets _characterPresets;
        [SerializeField] private UICustomizationSets _customizationSets;
        [SerializeField] private VisibilityPanel _visibilityPanel;
        [SerializeField] private CustomizationRandomizer _randomizer;

        private CharacterEditorData _characterData;
        private CustomizableCharacter _currentCharacter;
        private bool _isApplicationQuiting;

        #region Unity Methods

        private void Awake()
        {
            if (_settings == null)
            {
                Debug.LogError(
                    $"No settings was set for the Character Editor. Please make sure a {typeof(CharacterEditorSettings)} is set.",
                    this);
                gameObject.SetActive(false);
            }

            DetectAndSetInputModule();

            _firstTabToggle.group.SetAllTogglesOff();
            _firstTabToggle.isOn = true;

            _bodyColorOptionButton.ColorPickerChangedColor += OnBodyColorPickerChangedColor;
            _characterPresets.AppliedData += OnCharacterPresetsAppliedPresets;
            _customizationSets.AppliedData += OnCustomizationSetsAppliedSet;
            _characterDemo.Starting += OnCharacterDemoStarting;
            _characterDemo.Stopped += OnCharacterDemoStopped;
            _characterPicker.PickedCharacter += StartCustomizeCharacter;

            AddCharactersFromSettingsToPicker();
        }

        private void OnDestroy()
        {
            _bodyColorOptionButton.ColorPickerChangedColor -= OnBodyColorPickerChangedColor;
            _characterPresets.AppliedData -= OnCharacterPresetsAppliedPresets;
            _customizationSets.AppliedData -= OnCustomizationSetsAppliedSet;
            _characterDemo.Starting -= OnCharacterDemoStarting;
            _characterDemo.Stopped -= OnCharacterDemoStopped;
            _characterPicker.PickedCharacter -= StartCustomizeCharacter;

            if (_currentCharacter != null && !_isApplicationQuiting)
                StopCustomizeCharacter();
        }

        private void OnApplicationQuit()
        {
            _isApplicationQuiting = true;
        }

        #endregion

        #region Public Methods

        public void RandomizeCustomization()
        {
            _confirmationWindow.SelectedYes += DoRandomization;
            _confirmationWindow.Open("This action will remove all current customizations, are you sure?");
        }

        public void OpenSpriteCreator()
        {
            if (_currentCharacter == null)
                return;

            var spriteCreatorClips = GetSpriteExporterClips();
            if (spriteCreatorClips.Count == 0)
            {
                Debug.LogError(
                    $"{_characterData} needs animation clips to be able to open {nameof(_spriteExporter)}.");
                return;
            }

            _previewController.ResetCurrentCharacterPositions();
            _currentCharacter.HideRigs();
            _animationPlayer.Unbind();
            var animator = _currentCharacter.GetComponentInChildren<Animator>();
            if (animator == null)
                Debug.LogError("No animator was found in the character, please make sure one is added.");
            _spriteExporter.Bind(spriteCreatorClips.ToArray(), _characterData.DefaultPoseClipGroup.ToArray(),
                animator);
            _spriteExporter.gameObject.SetActive(true);
            _spriteExporter.Open();
            _cameraZoomController.enabled = false;
            Hide();
        }

        public void CloseSpriteCreator()
        {
            _spriteExporter.Unbind();
            _spriteExporter.Close();
            _spriteExporter.gameObject.SetActive(false);
            _previewController.ResetCurrentCharacterPositions();
            _previewController.ResetCurrentCharacterActiveStates();
            _previewController.PreviewCharacter(_currentCharacter);
            BindAnimationPlayer();
            _cameraZoomController.enabled = true;
            Show();
        }

        public void Revert() => _characterPicker.RepickCurrentCharacter();

        public void AddCharacter(CustomizableCharacter character) => AddCharacter(character, _characterData);

        public void AddCharacter(CustomizableCharacter character, CharacterEditorData data)
        {
            data.AddCustomizableCharacter(character);
            _characterPicker.AddCharacter(character, data);
        }

        public string TryGetACharacterEditorDataPath()
        {
            var assetPath = string.Empty;
#if UNITY_EDITOR
            if (_settings.CharacterEditorDatas.Length == 0)
                return Application.dataPath;
            var asset = _settings.CharacterEditorDatas[0];
            assetPath = AssetDatabase.GetAssetPath(asset);
            assetPath = assetPath.Replace(asset.name + ".asset", "");
#endif
            return assetPath;
        }

        public void OpenManual()
        {
            Application.OpenURL(
                "https://www.danielthomasart.com/CustomizableCharacters/Manual/manual/character%20editor.html");
        }

        #endregion

        #region Private Methods

        private void DetectAndSetInputModule()
        {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            {
                var inputModule = FindObjectOfType<StandaloneInputModule>();
                if (inputModule == null)
                    return;

                var go = inputModule.gameObject;
                go.AddComponent<InputSystemUIInputModule>();
                Destroy(inputModule);
            }
#endif
        }

        private void AddCharactersFromSettingsToPicker()
        {
            for (int i = 0; i < _settings.CharacterEditorDatas.Length; i++)
            {
                var data = _settings.CharacterEditorDatas[i];
                data.CleanupNullReferences();
                for (int j = 0; j < data.CustomizableCharacters.Count; j++)
                {
                    var character = data.CustomizableCharacters[j];
                    _characterPicker.AddCharacter(character, data);
                }
            }
        }

        private void Show() => gameObject.SetActive(true);

        private void Hide() => gameObject.SetActive(false);

        private void StartCustomizeCharacter(CustomizableCharacter character, CharacterEditorData data)
        {
            if (_currentCharacter != null || character == null)
                StopCustomizeCharacter();

            if (character == null)
                return;

            _currentCharacter = character;
            _characterData = data;

            _bodyColorOptionButton.SetColor(character.Customizer.BodyColor);
            _previewController.PreviewCharacter(character);
            _bodyColorOptionButton.BindSwatches(_characterData.RandomBodyColorGroups);
            _randomizer.BindCharacter(_currentCharacter);
            BuildUIFromData();
            ReselectCurrentTab();
            _visibilityPanel.BindCharacter(_currentCharacter);
            InvokeVisibilityValueToggles();
        }

        // make sure visibility settings is re-applied
        private void InvokeVisibilityValueToggles() => _visibilityPanel.InvokeToggleValues();

        private void StopCustomizeCharacter()
        {
            _previewController.ResetCurrentCharacterPositions();
            _previewController.ResetCurrentCharacterActiveStates();
            TearDownUI();
            _currentCharacter = null;
        }

        private void DoRandomization()
        {
            _currentCharacter.Customizer.RemoveAll();
            _randomizer.Randomize(_characterData, _settings);
            ReselectCurrentTab();
            InvokeVisibilityValueToggles();
        }

        private void ReselectCurrentTab()
        {
            var activeToggle = _firstTabToggle.group.GetFirstActiveToggle();
            if (activeToggle != null)
            {
                _firstTabToggle.group.SetAllTogglesOff();
                activeToggle.isOn = true;
            }
        }

        private List<SpriteExporterClip> GetSpriteExporterClips()
        {
            List<SpriteExporterClip> spriteCreatorClips = new List<SpriteExporterClip>();
            for (int i = 0; i < _characterData.AnimationClipGroups.Length; i++)
            {
                var clipGroup = _characterData.AnimationClipGroups[i];

                var clip = clipGroup.DownAnimationClip;
                var rig = _currentCharacter.DownRig;
                spriteCreatorClips.Add(new SpriteExporterClip(clip, rig));

                clip = clipGroup.SideAnimationClip;
                rig = _currentCharacter.SideRig;
                spriteCreatorClips.Add(new SpriteExporterClip(clip, rig));

                clip = clipGroup.UpAnimationClip;
                rig = _currentCharacter.UpRig;
                spriteCreatorClips.Add(new SpriteExporterClip(clip, rig));
            }

            return spriteCreatorClips;
        }

        private void TearDownUI()
        {
            if (_characterData == null)
                return;

            _appearanceTab?.Unbind();
            _equipmentTab?.Unbind();
            _scaleTab?.Unbind();
            _animationPlayer?.Unbind();
        }

        private void OnCharacterDemoStarting()
        {
            _animationPlayer.Unbind();
            _previewController.ResetCurrentCharacterPositions();
            _cameraZoomController.enabled = false;
            Hide();
        }

        private void OnCharacterDemoStopped()
        {
            BindAnimationPlayer();
            _previewController.ResetCurrentCharacterPositions();
            _previewController.ResetCurrentCharacterActiveStates();
            _previewController.ResetCurrentCharacterScale();
            _previewController.PreviewCharacter(_currentCharacter);
            _cameraZoomController.enabled = true;
            Show();
        }

        private void OnCharacterPresetsAppliedPresets()
        {
            ReselectCurrentTab();
            UpdateBodyColor();
            InvokeVisibilityValueToggles();
        }

        private void OnCustomizationSetsAppliedSet()
        {
            ReselectCurrentTab();
            InvokeVisibilityValueToggles();
        }

        private void OnBodyColorPickerChangedColor(Color color)
        {
            _currentCharacter.Customizer.SetBodyColor(color);
        }

        private void UpdateBodyColor()
        {
            _bodyColorOptionButton.SetColor(_currentCharacter.Customizer.BodyColor);
        }

        private void BuildUIFromData()
        {
            _currentCharacter.Customizer.Refresh();
            _equipmentTab.BindCustomization(_currentCharacter.Customizer, _characterData.Equipment);
            _appearanceTab.BindCustomization(_currentCharacter.Customizer, _characterData.Appearances);
            _scaleTab.Bind(_currentCharacter.ScaleCustomizer, _settings.ScaleDeviation);
            BindAnimationPlayer();

            _characterPresets.Bind(_currentCharacter, _characterData.CharacterPresets);
            _customizationSets.Bind(_currentCharacter, _characterData.CustomizationSets);
        }

        private void BindAnimationPlayer()
        {
            var animator = _currentCharacter.GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.LogError(
                    "No animator was found in the character, please make sure one is added if you want to play animations.");
                return;
            }

            _animationPlayer.BindCharacter(animator, _characterData.DefaultPoseClipGroup,
                _characterData.AnimationClipGroups);
        }

        #endregion
    }
}