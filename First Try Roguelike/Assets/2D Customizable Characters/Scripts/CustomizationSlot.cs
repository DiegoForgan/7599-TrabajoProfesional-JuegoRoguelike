using UnityEngine;

namespace CustomizableCharacters
{
    /// <summary>
    /// A location in a rig that changes SpriteRenderers.
    /// </summary>
    public class CustomizationSlot : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer _spriteRenderer;
        [SerializeField] protected SpriteRenderer _detailSpriteRenderer;
        [SerializeField] private CustomizationLocation _location;
        [Tooltip(
            "Makes the component not set main sprite color. Useful when something else should control the sprite renderer color.")]
        [SerializeField] private bool _ignoreSetMainColor;
        [Tooltip(
            "Makes the component not set detail sprite color. Useful when something else should control the sprite renderer color.")]
        [SerializeField] private bool _ignoreSetDetailColor;

        [HideInInspector]
        [SerializeField] private int _sortOrderOffset;

        public bool IsHidden => _spriteRenderer.enabled == false && _detailSpriteRenderer.enabled == false;
        public CustomizationLocation Location => _location;
        public Sprite Sprite => _spriteRenderer.sprite;

        public int OriginalSortOrder => _spriteRenderer.sortingOrder - _sortOrderOffset;
        public int SortOrder => _spriteRenderer.sortingOrder;
        public int DetailSortOrder => _detailSpriteRenderer.sortingOrder;

        /// <summary>
        /// Sets the sprites of the sprite renderers.
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="detailSprite"></param>
        public virtual void SetSprites(Sprite sprite, Sprite detailSprite)
        {
            _spriteRenderer.sprite = sprite;
            _detailSpriteRenderer.sprite = detailSprite;
        }

        /// <summary>
        /// Sets the colors of the sprite renderers. Won't have any effect if the 'ignore set colors' is true. 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="detailColor"></param>
        public void SetColors(Color color, Color detailColor)
        {
            if (!_ignoreSetMainColor)
                _spriteRenderer.color = color;

            if (!_ignoreSetDetailColor)
                _detailSpriteRenderer.color = detailColor;
        }

        /// <summary>
        /// Clears the slot by setting the sprites to null and colors to white.
        /// </summary>
        public void Clear()
        {
            SetSprites(null, null);
            SetColors(Color.white, Color.white);
            ResetSortOrders();
        }

        /// <summary>
        /// Hides the slot by disabling the sprite renderers.
        /// </summary>
        public void Hide()
        {
            _spriteRenderer.enabled = false;
            _detailSpriteRenderer.enabled = false;
        }

        /// <summary>
        /// Shows the slot by enabling the sprite renderers.
        /// </summary>
        public void Show()
        {
            _spriteRenderer.enabled = true;
            _detailSpriteRenderer.enabled = true;
        }

        /// <summary>
        /// Sets the sorting order of the sprite renderers to be higher than another CustomizationSlot.
        /// </summary>
        /// <param name="sortOverSlot"></param>
        public void SetSortOver(CustomizationSlot sortOverSlot)
        {
            ResetSortOrders();
            if (sortOverSlot == null)
                return;

            var newSortOrder = sortOverSlot.SortOrder + 1;
            _sortOrderOffset = newSortOrder - _spriteRenderer.sortingOrder;
            _spriteRenderer.sortingOrder = newSortOrder;
            _detailSpriteRenderer.sortingOrder = newSortOrder + 1;
        }

        private void ResetSortOrders()
        {
            var originalSortOrder = _spriteRenderer.sortingOrder - _sortOrderOffset;
            _spriteRenderer.sortingOrder = originalSortOrder;
            _detailSpriteRenderer.sortingOrder = originalSortOrder + 1;
            _sortOrderOffset = 0;
        }
    }
}