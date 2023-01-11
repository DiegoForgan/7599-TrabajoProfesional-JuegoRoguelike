using CustomizableCharacters.CharacterEditor.UI;
using UnityEditor;
using UnityEngine;

namespace CustomizableCharacters.CharacterEditor
{
    public class PrefabExporter : MonoBehaviour
    {
        [SerializeField] private CharacterPicker _characterPicker;
        [SerializeField] private CharacterEditorManager _characterEditorManager;
        private string _previousDirectory;

        public void Save()
        {
#if UNITY_EDITOR
            if (_previousDirectory == null || _previousDirectory.Contains(Application.dataPath) == false)
                _previousDirectory = Application.dataPath;
            
            var path = EditorUtility.SaveFilePanel("Save Customizable Character", _previousDirectory,
                "New Customizable Character", "prefab");
            _previousDirectory = path;

            if (string.IsNullOrEmpty(path))
                return;

            if (path.Contains(Application.dataPath) == false)
            {
                EditorUtility.DisplayDialog("Can't Save Prefab",
                    $"Prefab must be located inside the project asset folder {Application.dataPath}.", "Ok", null);
                return;
            }

            path = CreateAssetPath(path);
            SavePrefab(path);
            _previousDirectory = CreateFullPath(path);
#endif
        }

        public void Load()
        {
#if UNITY_EDITOR
            if (_previousDirectory == null || _previousDirectory.Contains(Application.dataPath) == false)
                _previousDirectory = Application.dataPath;

            var character = LoadCharacter();
            if (character == null)
                return;

            var data = LoadCharacterEditorData();
            if (data == null)
                return;

            AddPrefab(character, data);
#endif
        }
#if UNITY_EDITOR
        private CustomizableCharacter LoadCharacter()
        {
            var path = EditorUtility.OpenFilePanel("Load Customizable Character", _previousDirectory, "prefab");
            _previousDirectory = path;
            
            if (string.IsNullOrEmpty(path))
                return null;

            if (path.Contains(Application.dataPath) == false)
            {
                EditorUtility.DisplayDialog("Can't Load Prefab",
                    $"Prefab must be located inside the project asset folder {Application.dataPath}.", "Ok", null);
                return null;
            }

            path = CreateAssetPath(path);
            var character = AssetDatabase.LoadAssetAtPath<CustomizableCharacter>(path);
            if (character != null && _characterPicker.Contains(character))
            {
                Debug.Log($"{character.gameObject.name} already loaded, will pick it.");
                _characterPicker.SetCharacter(character);
                return null;
            }

            _previousDirectory = path;
            return character;
        }

        private CharacterEditorData LoadCharacterEditorData()
        {
            var loadPath = _characterEditorManager.TryGetACharacterEditorDataPath();
            loadPath = CreateFullPath(loadPath);
            var path =
                EditorUtility.OpenFilePanel($"Load using {nameof(CharacterEditorData)}", loadPath, "asset");

            if (string.IsNullOrEmpty(path))
                return null;

            if (path.Contains(Application.dataPath) == false)
            {
                EditorUtility.DisplayDialog($"Can't Load {nameof(CharacterEditorData)}",
                    $"{nameof(CharacterEditorData)} must be located inside the project asset folder {Application.dataPath}.",
                    "Ok", null);
                return null;
            }

            path = CreateAssetPath(path);
            var data = AssetDatabase.LoadAssetAtPath<CharacterEditorData>(path);
            if (data == null)
            {
                Debug.LogError($" {path} isn't a valid {nameof(CharacterEditorData)}, will not be able to load prefab.");
            }

            return data;
        }

        private string CreateAssetPath(string fullPath)
        {
            var path = fullPath.Replace(Application.dataPath, "");
            path = "Assets" + path;
            return path;
        }

        private string CreateFullPath(string dataPath)
        {
            dataPath = dataPath.Replace("Assets", "");
            var path = Application.dataPath + dataPath;
            return path;
        }

        private void SavePrefab(string path)
        {
            var character = _characterPicker.PickedCharacterInstance;
            _characterPicker.Unpick(); // clean up the prefab from preview stuff before saving it.
            var prefabAsset = PrefabUtility.SaveAsPrefabAsset(character.gameObject, path, out var didSave);

            if (didSave)
            {
                Debug.Log($"Saved {path}", prefabAsset);

                // destroy the current instance as it will be recreated by the character picker
                Destroy(character.gameObject);
                _characterEditorManager.AddCharacter(prefabAsset.GetComponent<CustomizableCharacter>());
                _characterPicker.SetCharacter(prefabAsset.GetComponent<CustomizableCharacter>());
            }
        }

        private void AddPrefab(CustomizableCharacter character, CharacterEditorData data)
        {
            _characterEditorManager.AddCharacter(character, data);
            _characterPicker.SetCharacter(character);
            Debug.Log($"Loaded {character}", character);
        }
#endif
    }
}