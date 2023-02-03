using System;
using UnityEngine;

namespace CustomizableCharacters
{
    /// <summary>
    /// Changes to sprites from another sprite renderer when scale is inversed. Mostly useful when animations inverses the scale and any of the sprites is showing the wrong visual direction. For example animations in up direction where arms are stretched out in front of character.
    /// </summary>
    public class InverseScaleSpriteReplacer : MonoBehaviour
    {
        [Serializable]
        private class SpriteReplacer
        {
            [SerializeField] private SpriteRenderer _originalSpriteRenderer;
            [SerializeField] private SpriteRenderer _replaceSpriteRenderer;
            [SerializeField] private bool _shouldChangeSortOrder;
            [SerializeField] private int _sortOrderWhenReplaced;
            [SerializeField] private bool _flipX;
            
            public SpriteRenderer OriginalSpriteRenderer => _originalSpriteRenderer;

            public bool OriginalFlipX { get; private set; }
            public Sprite OriginalSprite { get; private set; }

            public int OriginalSortOrder { get; private set; }

            public void Replace()
            {
                CopyOriginalValues();
                var replaceSprite = _replaceSpriteRenderer.sprite;
                _originalSpriteRenderer.sprite = replaceSprite;
                _originalSpriteRenderer.flipX = _flipX;
                if (_shouldChangeSortOrder)
                    _originalSpriteRenderer.sortingOrder = _sortOrderWhenReplaced;
            }

            public void CopyOriginalValues()
            {
                OriginalSprite = _originalSpriteRenderer.sprite;
                OriginalFlipX = _originalSpriteRenderer.flipX;
                if (_shouldChangeSortOrder)
                    OriginalSortOrder = _originalSpriteRenderer.sortingOrder;
            }

            public void RestoreOriginalValues()
            {
                _originalSpriteRenderer.sprite = OriginalSprite;
                _originalSpriteRenderer.flipX = OriginalFlipX;
                if (_shouldChangeSortOrder)
                    _originalSpriteRenderer.sortingOrder = OriginalSortOrder;
            }
        }

        [SerializeField] private SpriteReplacer[] _replaceDatas;
        private bool _didReplace;

        private void OnDestroy()
        {
            TryRestore();
        }

        private void LateUpdate()
        {
            TryReplace();
            TryRestore();
        }

        private void TryReplace()
        {
            var highestSortOrder = 0;
            var lowestSortOrder = 0;
            if (transform.lossyScale.y < 0 && _didReplace == false)
            {
                for (int i = 0; i < _replaceDatas.Length; i++)
                {
                    var data = _replaceDatas[i];
                    data.Replace();
                    if (data.OriginalSpriteRenderer.sortingOrder < lowestSortOrder)
                        lowestSortOrder = data.OriginalSpriteRenderer.sortingOrder;
                    if (data.OriginalSpriteRenderer.sortingOrder > highestSortOrder)
                        highestSortOrder = data.OriginalSpriteRenderer.sortingOrder;
                }

                _didReplace = true;
            }
        }

        private void TryRestore()
        {
            if (!_didReplace)
                return;
            if (transform.lossyScale.y >= 0)
            {
                for (int i = 0; i < _replaceDatas.Length; i++)
                {
                    var data = _replaceDatas[i];
                    data.RestoreOriginalValues();
                }

                _didReplace = false;
            }
        }
    }
}