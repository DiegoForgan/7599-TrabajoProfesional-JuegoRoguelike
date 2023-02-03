using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CustomizableCharacters.Editor
{
    [CustomEditor(typeof(CustomizableCharacter))]
    public class CustomizableCharacterEditor : UnityEditor.Editor
    {
        private CustomizableCharacter _script;
        private string _previousDirectory;
        private Object[] _customizationScripts;

        private void OnEnable()
        {
            _script = (target as CustomizableCharacter);
            _customizationScripts = new MonoBehaviour[] { _script.Customizer, _script.ScaleCustomizer };
        }

        public override void OnInspectorGUI()
        {
            InspectorLayout.Header("Customizable Character");

            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Note that editing the inspector during playmode is not supported.",
                    MessageType.Warning);
            }

            DrawDefaultInspector();
            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            DrawToggleShadows();
            DrawToggleWeaponEffects();
            DrawTogglePreview();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            DrawDropArea();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(140));
            DrawSavePreset();
            DrawLoadPreset();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void DrawToggleShadows()
        {
            var isHidden = _script.IsShadowsHidden;
            if (GUILayout.Button("Toggle Shadows"))
            {
                _script.SetHideShadows(!isHidden);
            }
        }

        private void DrawToggleWeaponEffects()
        {
            var isHidden = _script.IsWeaponEffectsHidden;
            if (GUILayout.Button("Toggle Weapon Effects"))
            {
                _script.SetHideWeaponEffects(!isHidden);
            }
        }

        private void DrawTogglePreview()
        {
            if (GUILayout.Button("Toggle Preview Positions"))
            {
                var rigs = new Object[3] { _script.DownRig.transform, _script.SideRig.transform, _script.UpRig.transform };
                Undo.RecordObjects(rigs, $"Toggle Preview Positions");
                ToggleRigPositions();
            }
        }

        private void DrawSavePreset()
        {
            if (GUILayout.Button("Save Preset"))
            {
                if (_previousDirectory == null || _previousDirectory.Contains(Application.dataPath) == false)
                    _previousDirectory = Application.dataPath;
                var path = EditorUtility.SaveFilePanel("Save Character Preset", _previousDirectory,
                    "New Character Preset", "asset");

                if (string.IsNullOrEmpty(path) == false)
                {
                    _previousDirectory = path;
                    path = CreateAssetPath(path);
                    SavePreset(path);
                }

                GUIUtility.ExitGUI();
            }
        }

        private void DrawLoadPreset()
        {
            if (GUILayout.Button("Load Preset"))
            {
                if (_previousDirectory == null || _previousDirectory.Contains(Application.dataPath) == false)
                    _previousDirectory = Application.dataPath;
                var path = EditorUtility.OpenFilePanel("Load Character Preset", _previousDirectory, "asset");

                if (string.IsNullOrEmpty(path) == false)
                {
                    _previousDirectory = path;
                    path = CreateAssetPath(path);
                    LoadPreset(path);
                }

                GUIUtility.ExitGUI();
            }
        }

        private void SavePreset(string path)
        {
            var preset = _script.CreatePreset();
            var asset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            if (AssetDatabase.Contains(asset))
            {
                EditorUtility.CopySerialized(preset, asset);
                AssetDatabase.SaveAssets();
            }
            else
            {
                AssetDatabase.CreateAsset(preset, path);
            }

            AssetDatabase.Refresh();

            asset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
            Debug.Log($"Saved {path}", asset);
        }

        private void LoadPreset(string path)
        {
            var preset = (CharacterPreset)AssetDatabase.LoadAssetAtPath(path, typeof(CharacterPreset));
            if (preset == null)
            {
                EditorUtility.DisplayDialog("Can't Load Preset",
                    $"Can't load preset, make sure the asset is of type {nameof(CharacterPreset)}", "Ok", null);
                return;
            }

            _script.ApplyPreset(preset);
        }

        private void ToggleRigPositions()
        {
            var isPreviewing = _script.SideRig.transform.localPosition != Vector3.zero
                               || _script.UpRig.transform.localPosition != Vector3.zero;
            if (isPreviewing)
            {
                _script.SideRig.transform.localPosition = Vector3.zero;
                _script.DownRig.transform.localPosition = Vector3.zero;
                _script.UpRig.transform.localPosition = Vector3.zero;
            }
            else
            {
                _script.SideRig.transform.localPosition = new Vector3(-2, 0, 0);
                _script.DownRig.transform.localPosition = Vector3.zero;
                _script.UpRig.transform.localPosition = new Vector3(+2, 0, 0);
            }
        }

        private void DrawDropArea()
        {
            var dropRect = InspectorLayout.DropBox($"{nameof(CharacterPreset)}");

            var currentEvent = Event.current;
            switch (currentEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:

                    if (dropRect.Contains(currentEvent.mousePosition) == false)
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (currentEvent.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (Object draggedObject in DragAndDrop.objectReferences)
                        {
                            if (draggedObject is CharacterPreset preset)
                            {
                                ApplyPreset(preset);
                            }
                            else
                            {
                                Debug.LogWarning($"Object type must be {nameof(CharacterPreset)}.", draggedObject);
                            }
                        }
                    }

                    break;
            }
        }

        private void ApplyPreset(CharacterPreset preset)
        {
            if (EditorUtility.DisplayDialog("Apply Character Preset?",
                    $"Are you sure you want to apply {preset.name}, any previous customization will be removed.", "Yes",
                    "No"))
            {
                Undo.RecordObjects(_customizationScripts, $"Add {nameof(CharacterPreset)}");
                _script.ApplyPreset(preset);

                for (int i = 0; i < _customizationScripts.Length; i++)
                {
                    EditorUtility.SetDirty(_customizationScripts[i]);
                }
            }
        }

        private string CreateAssetPath(string fullPath)
        {
            var path = fullPath.Replace(Application.dataPath, "");
            path = "Assets" + path;
            return path;
        }
    }
}