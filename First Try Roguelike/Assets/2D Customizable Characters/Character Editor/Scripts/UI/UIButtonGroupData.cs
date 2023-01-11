using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public abstract class UIButtonGroupData<T> : MonoBehaviour where T : Object
    {
        [SerializeField] protected ButtonGroup _buttonGroup;
        [SerializeField] protected ConfirmationWindow _confirmationWindow;

        protected CustomizableCharacter _currentCharacter;
        protected string _previousDirectory;
        protected List<T> _datas = new List<T>();

        public event Action AppliedData;

        public void Bind(CustomizableCharacter character, T[] presets)
        {
            _currentCharacter = character;
            CreateButtons(presets);
            _buttonGroup.ButtonClicked += OnDropdownValueChanged;
        }

        private void OnDestroy()
        {
            _buttonGroup.ButtonClicked -= OnDropdownValueChanged;
        }

        private void CreateButtons(T[] presets)
        {
            _datas.Clear();
            _buttonGroup.ClearButtons();
            for (int i = 0; i < presets.Length; i++)
            {
                var preset = presets[i];
                AddButton(preset);
            }
        }

        private void AddButton(T data)
        {
            if (_datas.Contains(data))
                return;

            var buttonLabel = "null";
            if (data != null)
                buttonLabel = data.name;

            _buttonGroup.AddButton(buttonLabel);
            _datas.Add(data);
        }

        private void OnDropdownValueChanged(int index)
        {
            _confirmationWindow.SelectedYes += () =>
            {
                OnConfirmWindowSelectedYes(index);
                AppliedData?.Invoke();
            };
            _confirmationWindow.Open(GetConfirmationText());
        }

        protected abstract void OnConfirmWindowSelectedYes(int index);
        protected abstract string GetConfirmationText();

        public void Save()
        {
#if UNITY_EDITOR
            if (_previousDirectory == null || _previousDirectory.Contains(Application.dataPath) == false)
                _previousDirectory = Application.dataPath;

            var previousPath = _previousDirectory;
            var path = EditorUtility.SaveFilePanel("Save Character Preset", previousPath,
                "New Character Preset", "asset");
            _previousDirectory = path;

            if (string.IsNullOrEmpty(path) == false)
            {
                if (!path.Contains(Application.dataPath))
                {
                    EditorUtility.DisplayDialog("Can't Save Preset",
                        $"Asset must be saved inside the project asset folder {Application.dataPath}.", "Ok", null);
                    return;
                }

                path = CreateAssetPath(path);
                var dataObject = GetObjectToSave();
                SaveAsset(dataObject, path);
            }
#endif
        }

        public void Load()
        {
#if UNITY_EDITOR
            if (_previousDirectory == null || _previousDirectory.Contains(Application.dataPath) == false)
                _previousDirectory = Application.dataPath;

            var previousPath = _previousDirectory;
            var path = EditorUtility.OpenFilePanel("Load Character Preset", previousPath, "asset");
            _previousDirectory = path;

            if (string.IsNullOrEmpty(path) == false)
            {
                if (!path.Contains(Application.dataPath))
                {
                    EditorUtility.DisplayDialog("Can't Load Preset",
                        $"Asset must be located inside the project asset folder {Application.dataPath}.", "Ok", null);
                    return;
                }

                path = CreateAssetPath(path);
                var asset = (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
                if (asset == null)
                {
                    EditorUtility.DisplayDialog("Can't Load Preset",
                        $"Can't load asset, make sure the asset is of type {nameof(T)}", "Ok", null);
                    return;
                }

                AddButton(asset);
                ApplyLoadedData(asset);
                AppliedData?.Invoke();
            }
#endif
        }

        private void SaveAsset(T dataObject, string path)
        {
#if UNITY_EDITOR
            var asset = (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
            if (asset != null)
            {
                dataObject.name = asset.name;
                EditorUtility.CopySerialized(dataObject, asset);
                // AssetDatabase.SaveAssets();
            }
            else
            {
                AssetDatabase.CreateAsset(dataObject, path);
            }
            
            AssetDatabase.Refresh();
            asset = (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
            Debug.Log($"Saved {path}", asset);
            AddButton(asset);
#endif
        }

        protected abstract T GetObjectToSave();

        protected abstract void ApplyLoadedData(T data);

        private string CreateAssetPath(string fullPath)
        {
            var path = fullPath.Replace(Application.dataPath, "");
            path = "Assets" + path;
            return path;
        }
    }
}