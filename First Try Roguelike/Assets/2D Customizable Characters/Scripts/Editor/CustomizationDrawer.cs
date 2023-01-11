using UnityEditor;
using UnityEngine;

namespace CustomizableCharacters.Editor
{
    [CustomPropertyDrawer(typeof(Customization))]
    public class CustomizationDrawer : PropertyDrawer
    {
        private GUIContent _hiddenIcon;
        private GUIContent _shownIcon;
        private SerializedProperty _property;
        private SerializedProperty _dataProperty;
        private CustomizationData _customizationData;
        private const string MainColorPropertyName = "_mainColor";
        private const string DetailColorPropertyName = "_detailColor";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SetupIcons();
            _property = property;
            _dataProperty = property.FindPropertyRelative("_customizationData");
            _customizationData = (CustomizationData)_dataProperty.objectReferenceValue;

            ValidateCustomizationData(_customizationData);

            if (_customizationData == null || _customizationData.Category == null)
                label.text = "MISSING REFERENCE";
            else
                label.text = _customizationData.Category.name;

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            var setRectWidth = position.width - 130;
            position.x -= 40;

            var setPropertyRect = new Rect(position.x, position.y, setRectWidth, position.height);
            var colorRect = new Rect(position.x + setRectWidth + 10, position.y, 25, position.height);
            var arrowLeftRect = new Rect(position.x + setRectWidth + 45, position.y, 20, position.height);
            var indexRect = new Rect(position.x + setRectWidth + 65, position.y, 20, position.height);
            var arrowRightRect = new Rect(position.x + setRectWidth + 90, position.y, 20, position.height);
            var detailColorRect = new Rect(position.x + setRectWidth + 115, position.y, 25, position.height);
            var visibilityRect = new Rect(position.x + setRectWidth + 145, position.y, 28, position.height);

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(setPropertyRect, _dataProperty, GUIContent.none);

            // reset if the data property was changed
            if (EditorGUI.EndChangeCheck())
                Reset();

            if (_customizationData != null)
            {
                if (_customizationData.CanBeTinted && PropertyHasSprite())
                    DrawColorBox(colorRect, MainColorPropertyName);

                if (PropertyHasDetailSprite())
                {
                    if (_customizationData.GetDetailSpriteCount() > 1)
                        DrawDetailSpriteIndex(arrowLeftRect, arrowRightRect, indexRect);

                    if (_customizationData.UseMainColorForDetail == false && _customizationData.CanDetailsBeTinted)
                        DrawColorBox(detailColorRect, DetailColorPropertyName);
                }

                if (_customizationData.Category.CanBeHidden)
                    DrawVisibilityButton(visibilityRect);
            }

            EditorGUI.EndProperty();
        }

        private void DrawVisibilityButton(Rect rect)
        {
            var isHiddenProperty = _property.FindPropertyRelative("_isHidden");
            var icon = isHiddenProperty.boolValue ? _hiddenIcon : _shownIcon;
            var isHidden = isHiddenProperty.boolValue;

            if (GUI.Button(rect, icon))
            {
                isHidden = !isHidden;
            }

            if (isHidden != isHiddenProperty.boolValue)
            {
                SetIsHidden(isHidden);
            }
        }

        private void DrawColorBox(Rect rect, string propertyName)
        {
            var colorProperty = _property.FindPropertyRelative(propertyName);
            var colorValue = colorProperty.colorValue;
            var color = EditorGUI.ColorField(rect, GUIContent.none, colorProperty.colorValue,
                false, true, false);

            if (colorValue != color)
                SetColorProperty(colorProperty, color);
        }

        private bool PropertyHasSprite()
        {
            if (_customizationData == null)
                return false;

            var elements = _customizationData.SpriteSets;
            if (elements == null)
                return false;

            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i].DownSprite)
                    return true;
                if (elements[i].SideSprite)
                    return true;
                if (elements[i].UpSprite)
                    return true;
            }

            return false;
        }

        private bool PropertyHasDetailSprite()
        {
            if (_customizationData == null)
                return false;

            var elements = _customizationData.SpriteSets;
            if (elements == null)
                return false;

            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i].DetailSpriteSets.Length > 0)
                    return true;
                if (elements[i].DetailSpriteSets.Length > 0)
                    return true;
                if (elements[i].DetailSpriteSets.Length > 0)
                    return true;
            }

            return false;
        }

        private void DrawDetailSpriteIndex(Rect leftRect, Rect rightRect, Rect indexRect)
        {
            var detailIndex = _property.FindPropertyRelative("_detailSpritesIndex");
            var currentDetailIndex = detailIndex.intValue;
            var detailsCount = _customizationData.SpriteSets[0].DetailSpriteSets.Length;

            if (currentDetailIndex > detailsCount)
                currentDetailIndex = detailsCount;

            if (GUI.Button(leftRect, "<"))
            {
                if (currentDetailIndex - 1 < 0)
                {
                    currentDetailIndex = detailsCount - 1;
                }
                else
                {
                    currentDetailIndex -= 1;
                }
            }

            indexRect.x = indexRect.center.x;
            GUI.Label(indexRect, currentDetailIndex.ToString());

            if (GUI.Button(rightRect, ">"))
            {
                if (currentDetailIndex + 1 >= detailsCount)
                {
                    currentDetailIndex = 0;
                }
                else
                {
                    currentDetailIndex += 1;
                }
            }

            if (currentDetailIndex != detailIndex.intValue)
            {
                SetDetailIndex(currentDetailIndex);
            }
        }

        private void Reset()
        {
            var colorProperty = _property.FindPropertyRelative(MainColorPropertyName);
            SetColorProperty(colorProperty, Color.white);

            colorProperty = _property.FindPropertyRelative(DetailColorPropertyName);
            SetColorProperty(colorProperty, Color.white);

            SetDetailIndex(0);

            SetIsHidden(false);
        }

        private void SetColorProperty(SerializedProperty property, Color color)
        {
            property.colorValue = color;
            property.serializedObject.ApplyModifiedProperties();
        }

        private void SetDetailIndex(int index)
        {
            var property = _property.FindPropertyRelative("_detailSpritesIndex");
            property.intValue = index;
            property.serializedObject.ApplyModifiedProperties();
        }

        private void SetIsHidden(bool isHidden)
        {
            var property = _property.FindPropertyRelative("_isHidden");
            property.boolValue = isHidden;
            property.serializedObject.ApplyModifiedProperties();
        }

        private void SetupIcons()
        {
            if (EditorGUIUtility.isProSkin)
                _hiddenIcon = EditorGUIUtility.IconContent("d_SceneViewVisibility");
            else
                _hiddenIcon = EditorGUIUtility.IconContent("SceneViewVisibility");

            if (EditorGUIUtility.isProSkin)
                _shownIcon = EditorGUIUtility.IconContent("d_scenevis_visible_hover");
            else
                _shownIcon = EditorGUIUtility.IconContent("scenevis_visible_hover");
        }

        private void ValidateCustomizationData(CustomizationData data)
        {
            if (data == null)
            {
                return;
            }

            if (data.Category == null)
                Debug.LogError($"Missing {nameof(CustomizationCategory)}", data);
            if (data.SpriteSets == null)
                Debug.LogError($"Missing {nameof(CustomizationSpriteSet)}", data);
        }
    }
}