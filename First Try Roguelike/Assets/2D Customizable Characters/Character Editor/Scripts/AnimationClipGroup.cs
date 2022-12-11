using System;
using UnityEngine;

namespace CustomizableCharacters.CharacterEditor
{
    /// <summary>
    /// A group of animation clips used for previewing animations in the character editor.
    /// </summary>
    [Serializable]
    public class AnimationClipGroup
    {
        public AnimationClipGroup(string groupName, AnimationClip downAnimationClip, AnimationClip sideAnimationClip,
            AnimationClip upAnimationClip)
        {
            _groupName = groupName;
            _downAnimationClip = downAnimationClip;
            _sideAnimationClip = sideAnimationClip;
            _upAnimationClip = upAnimationClip;
        }

        [SerializeField] private string _groupName;
        [SerializeField] private AnimationClip _downAnimationClip;
        [SerializeField] private AnimationClip _sideAnimationClip;
        [SerializeField] private AnimationClip _upAnimationClip;

        public string GroupName => _groupName;
        public AnimationClip DownAnimationClip => _downAnimationClip;
        public AnimationClip SideAnimationClip => _sideAnimationClip;
        public AnimationClip UpAnimationClip => _upAnimationClip;

        public AnimationClip[] ToArray()
        {
            return new AnimationClip[] { _downAnimationClip, _sideAnimationClip, _upAnimationClip };
        }
    }
}