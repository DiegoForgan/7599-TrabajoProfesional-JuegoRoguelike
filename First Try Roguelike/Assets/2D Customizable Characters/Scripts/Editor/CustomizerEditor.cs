using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomizableCharacters.Editor
{
    [CustomEditor(typeof(Customizer))]
    public class CustomizerEditor : UnityEditor.Editor
    {
        private Customizer _script;

        private void OnEnable()
        {
            _script = (target as Customizer);
        }

        public override void OnInspectorGUI()
        {
            InspectorLayout.Header("Customizer");

            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox(
                    "Note that editing the inspector during playmode might conflict with playmode customizer.",
                    MessageType.Warning);
            }

            DrawDefaultInspector();

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            DrawDropArea();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(140));
            DrawToggleVisibility();
            DrawRefreshButton();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        private void DrawRefreshButton()
        {
            if (GUILayout.Button("Refresh"))
                _script.Refresh();
        }

        private void DrawToggleVisibility()
        {
            var isHidden = _script.IsAllCustomizationHidden;
            if (GUILayout.Button("Toggle Visibility"))
            {
                Undo.RecordObject(_script, $"Toggle Visibility");
                if (isHidden)
                    _script.ShowAll();
                else
                    _script.HideAll();
            }
        }

        private void DrawDropArea()
        {
            var dropRect = InspectorLayout.DropBox($"{nameof(CustomizationData)} / {nameof(CustomizationSet)}");

            var currentEvent = Event.current;
            switch (currentEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:

                    if (dropRect.Contains(currentEvent.mousePosition) == false)
                        break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (currentEvent.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (Object draggedObject in DragAndDrop.objectReferences)
                        {
                            if (draggedObject is CustomizationData customizationData)
                            {
                                AddCustomization(customizationData);
                            }
                            else if (draggedObject is CustomizationSet customizationSet)
                            {
                                AddCustomizedSet(customizationSet);
                            }
                            else
                            {
                                Debug.LogWarning(
                                    $"Object type must be {nameof(CustomizationData)} or {nameof(CustomizationSet)}.",
                                    draggedObject);
                            }
                        }
                    }

                    break;
            }
        }

        private void AddCustomizedSet(CustomizationSet customizationSet)
        {
            if (customizationSet == null)
                return;

            Undo.RecordObject(_script, $"Add {nameof(CustomizationData)}");
            _script.ApplySet(customizationSet);
            EditorUtility.SetDirty(_script);
        }

        private void AddCustomization(CustomizationData customizationData)
        {
            if (customizationData == null)
                return;

            if (_script.Contains(customizationData))
            {
                Debug.Log($"{customizationData.name} already added.");
                return;
            }

            Undo.RecordObject(_script, $"Add {nameof(CustomizationData)}");
            _script.Add(customizationData);
            EditorUtility.SetDirty(_script);
        }
    }
}