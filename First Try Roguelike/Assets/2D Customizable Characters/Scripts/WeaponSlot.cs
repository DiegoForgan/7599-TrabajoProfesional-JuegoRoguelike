using UnityEngine;

namespace CustomizableCharacters
{
    /// <summary>
    /// Used for weapons. Will adjusts effect positions based on length of the weapon sprites and disable effects if no sprites are set.
    /// </summary>
    public class WeaponSlot : CustomizationSlot
    {
        [SerializeField] private SpriteRenderer _swingEffect;
        [SerializeField] private SpriteRenderer _stabEffect;

        public SpriteRenderer SwingEffect => _swingEffect;
        public SpriteRenderer StabEffect => _stabEffect;

        /// <summary>
        /// Sets the sprite and calculates the length, which is used for changing the position of the weapon effects.
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="detailSprite"></param>
        public override void SetSprites(Sprite sprite, Sprite detailSprite)
        {
            base.SetSprites(sprite, detailSprite);
            var weaponLength = GetWeaponLength();
            if (_swingEffect != null)
            {
                var swingLocalPosition = new Vector3(0, weaponLength);
                _swingEffect.transform.localPosition = swingLocalPosition;
                _swingEffect.enabled = _spriteRenderer.sprite != null;
            }

            if (_stabEffect != null)
            {
                var stabLocalPosition = new Vector3(0, weaponLength);
                _stabEffect.transform.localPosition = stabLocalPosition;
                _stabEffect.enabled = _spriteRenderer.sprite != null;
            }
        }

        /// <summary>
        /// Returns the length of the distance between hand and top of the weapon (usually weapon tip). This is done by the height if the weapon sprites.
        /// </summary>
        /// <returns></returns>
        public float GetWeaponLength()
        {
            var weaponLength = 0f;
            var detailLength = 0f;

            if (_spriteRenderer.sprite != null)
            {
                var sprite = _spriteRenderer.sprite;
                weaponLength = (sprite.rect.height - sprite.pivot.y) / sprite.pixelsPerUnit;
            }

            if (_detailSpriteRenderer.sprite != null)
            {
                var detailSprite = _detailSpriteRenderer.sprite;
                detailLength = (detailSprite.rect.height - detailSprite.pivot.y) / detailSprite.pixelsPerUnit;
            }

            var longest = weaponLength > detailLength ? weaponLength : detailLength;
            return longest;
        }
    }
}