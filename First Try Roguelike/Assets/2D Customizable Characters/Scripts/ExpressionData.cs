using UnityEngine;

namespace CustomizableCharacters
{
    /// <summary>
    /// Contains references that composes an expression.
    /// </summary>
    [CreateAssetMenu(menuName = "2D Customizable Characters/Expression Data", fileName = "ExpressionData")]
    public class ExpressionData : ScriptableObject
    {
        [SerializeField] private CustomizationData _eyebrowsAppearance;
        [SerializeField] private CustomizationData _eyesAppearance;
        [SerializeField] private CustomizationData _mouthAppearance;

        public CustomizationData EyebrowsAppearance => _eyebrowsAppearance;
        public CustomizationData EyesAppearance => _eyesAppearance;
        public CustomizationData MouthAppearance => _mouthAppearance;
    }
}