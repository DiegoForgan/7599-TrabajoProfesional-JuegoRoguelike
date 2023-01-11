using UnityEngine;

namespace CustomizableCharacters
{
    /// <summary>
    /// Changes the expressions of the character.
    /// </summary>
    public class CharacterExpression : MonoBehaviour
    {
        [SerializeField] private Customizer _customizer;

        private CustomizationData _defaultEyebrows;
        private CustomizationData _defaultEyes;
        private CustomizationData _defaultMouth;
        private bool _hasSetExpression;

        /// <summary>
        /// Sets the expression of the character
        /// </summary>
        /// <param name="expressionData"></param>
        public void SetExpression(ExpressionData expressionData)
        {
            var eyebrows = expressionData.EyebrowsAppearance;
            if (eyebrows != null)
                SetData(eyebrows, ref _defaultEyebrows);

            var eyes = expressionData.EyesAppearance;
            if (eyes != null)
                SetData(eyes, ref _defaultEyes);

            var mouth = expressionData.MouthAppearance;
            if (mouth != null)
                SetData(mouth, ref _defaultMouth);

            _hasSetExpression = true;
        }

        /// <summary>
        /// Sets the expression to the default one. The default is cached when an expression is set for the first time.
        /// </summary>
        public void SetToDefault()
        {
            if (!_hasSetExpression)
                return;

            if (_defaultEyebrows != null && _customizer.Contains(_defaultEyebrows) == false)
                _customizer.Add(_defaultEyebrows);

            if (_defaultEyes != null && _customizer.Contains(_defaultEyes) == false)
                _customizer.Add(_defaultEyes);

            if (_defaultMouth != null && _customizer.Contains(_defaultMouth) == false)
                _customizer.Add(_defaultMouth);

            _hasSetExpression = false;
        }

        private void SetData(CustomizationData data, ref CustomizationData defaultData)
        {
            if (data != null)
            {
                if (!_hasSetExpression)
                    defaultData = _customizer.GetCustomizationDataInCategory(data.Category);

                if (_customizer.Contains(data) == false)
                    _customizer.Add(data);
            }
            else if (defaultData != null)
                _customizer.Add(defaultData);
        }
    }
}