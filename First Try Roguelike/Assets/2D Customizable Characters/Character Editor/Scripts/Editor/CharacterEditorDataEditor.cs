using UnityEditor;
using UnityEngine;

namespace CustomizableCharacters.CharacterEditor
{
    [CustomEditor(typeof(CharacterEditorData))]
    public class CharacterEditorDataEditor : Editor
    {
        private CharacterEditorData _script;

        private void OnEnable()
        {
            _script = (target as CharacterEditorData);
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            // Just a quick fix to make array elements display the names in the inspector
            if (GUILayout.Button("Set Names"))
            {
                var appearances = _script.Appearances;
                for (int i = 0; i < appearances.Length; i++)
                {
                    appearances[i].DisplayName = appearances[i].Data.name;
                }

                var equipment = _script.Equipment;
                for (int i = 0; i < equipment.Length; i++)
                {
                    equipment[i].DisplayName = equipment[i].Data.name;
                }
            }
        }
    }
}