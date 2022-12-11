using UnityEditor;
using UnityEngine;

namespace CustomizableCharacters.Editor
{
    public static class InspectorLayout
    {
        private static GUIStyle _headerStyle;
        private static GUIStyle _dropBoxStyle;
        private static Color _dropBoxColor = new Color(0.68f, 0.91f, 1f, 0.5f);

        public static void Header(string label)
        {
            if (_headerStyle == null)
            {
                _headerStyle = new GUIStyle(GUI.skin.label);
                _headerStyle.fontSize = 15;
                _headerStyle.fontStyle = FontStyle.Bold;
                _headerStyle.alignment = TextAnchor.MiddleCenter;
            }

            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(label, _headerStyle);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        public static Rect DropBox(string typeInfo)
        {
            if (_dropBoxStyle == null)
            {
                _dropBoxStyle = new GUIStyle(GUI.skin.textField);
                _dropBoxStyle.alignment = TextAnchor.MiddleCenter;
            }

            var previousColor = GUI.color;
            GUI.color = _dropBoxColor;
            var rect = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            GUI.Box(rect, $"Drag and Drop {typeInfo} here", _dropBoxStyle);
            GUI.color = previousColor;
            return rect;
        }
    }
}