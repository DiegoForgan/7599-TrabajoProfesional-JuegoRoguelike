using UnityEngine;

namespace CustomizableCharacters
{
    /// <summary>
    /// A category for customizations. Used to make sure only one customization exist per category.
    /// </summary>
    [CreateAssetMenu(menuName = "2D Customizable Characters/Customization Category", fileName = "CustomizationCategory")]
    public class CustomizationCategory : ScriptableObject
    {
        [Tooltip("If customizations in this category should be able to hide." +
                 " Usually true if it's some kind of equipment and not part of the body." +
                 " Note that customizations that is hiding any location can still hide the sprites.")]
        [SerializeField] private bool _canBeHidden = true;

        /// <summary>
        /// If the category can be hidden or not. Categories that can't be hidden is usually thing part of a body, for example ears.
        /// </summary>
        public bool CanBeHidden => _canBeHidden;
    }
}