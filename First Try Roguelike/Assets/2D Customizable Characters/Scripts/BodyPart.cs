using UnityEngine;

namespace CustomizableCharacters
{
    /// <summary>
    /// Body part of a character. 
    /// </summary>
    public class BodyPart : MonoBehaviour
    {
        [SerializeField] SpriteRenderer _spriteRenderer;

        /// <summary>
        /// Sets the color of the body part.
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(Color color)
        {
            _spriteRenderer.color = color;
        }
    }
}