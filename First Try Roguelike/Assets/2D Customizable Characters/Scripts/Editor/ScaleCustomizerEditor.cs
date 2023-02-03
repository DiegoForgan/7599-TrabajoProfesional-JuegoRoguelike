using UnityEditor;

namespace CustomizableCharacters.Editor
{
    [CustomEditor(typeof(ScaleCustomizer))]
    public class ScaleCustomizerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            InspectorLayout.Header("Scale Customizer");
            base.OnInspectorGUI();
        }
    }
}