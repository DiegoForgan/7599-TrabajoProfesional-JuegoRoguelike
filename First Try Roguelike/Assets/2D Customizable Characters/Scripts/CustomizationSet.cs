using UnityEngine;

namespace CustomizableCharacters
{
    /// <summary>
    /// A collection of customizations.
    /// </summary>
    [CreateAssetMenu(menuName = "2D Customizable Characters/Customization Set", fileName = "CustomizationSet")]
    public class CustomizationSet : ScriptableObject
    {
        [SerializeField] private Customization[] _customizations;
        
        /// <summary>
        /// The collection of customizations.
        /// </summary>
        public Customization[] Customizations => _customizations;
    }
}