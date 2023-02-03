using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace CustomizableCharacters.CharacterEditor.UI
{
    public class AnimationPlayer : MonoBehaviour
    {
        [SerializeField] private Dropdown _dropdown;

        private AnimationClipGroup _defaultPoseAnimationClipGroup;
        private List<AnimationClipGroup> _animationClipGroups = new List<AnimationClipGroup>();
        private Animator _characterAnimator;

        private PlayableGraph _playableGraph;
        private PlayableOutput _playableOutputDown;
        private PlayableOutput _playableOutputSide;
        private PlayableOutput _playableOutputUp;
        private AnimationClipPlayable _downPlayable;
        private AnimationClipPlayable _sidePlayable;
        private AnimationClipPlayable _upPlayable;
        private bool _hasBoundData;
        private int currentIndex;

        #region Unity Methods

        private void Awake()
        {
            _dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        private void OnDestroy()
        {
            _dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);

            if (_playableGraph.IsValid())
                DestroyPlayable();
        }

        private void Update()
        {
            if (!_hasBoundData)
                return;

            var clip = _downPlayable.GetAnimationClip();
            var isLooping = clip.isLooping;

            if (currentIndex == 0 || isLooping)
                return;

            var time = _downPlayable.GetTime();
            var length = clip.length;

            if (time >= length)
            {
                RestartClips();
            }
        }

        #endregion

        public void BindCharacter(Animator animator, AnimationClipGroup defaultPoseClipGroup,
            AnimationClipGroup[] clipGroups)
        {
            // make the animator stop updating while this class uses it
            _characterAnimator = animator;
            _characterAnimator.WriteDefaultValues();
            _characterAnimator.Rebind();
            _characterAnimator.playableGraph.Stop();
            _characterAnimator.playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

            _defaultPoseAnimationClipGroup = defaultPoseClipGroup;
            _animationClipGroups = CopyClips(clipGroups);
            _animationClipGroups.Insert(0, null); // for "none" animation option

            CreateDropdownOptions();
            CreatePlayables();
            SampleDefaultPoses();

            if (_animationClipGroups.Count > 0)
            {
                PlayCLipAtIndex(1);
                _dropdown.SetValueWithoutNotify(1);
            }

            _hasBoundData = true;
        }

        public void Unbind()
        {
            if (_playableGraph.IsValid())
            {
                SampleDefaultPoses();
                _playableGraph.Stop();
                DestroyPlayable();
            }

            _animationClipGroups.Clear();
            _dropdown.ClearOptions();
            _characterAnimator.playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            // for some reason this seems to be the only thing that works to make the animator start playing again... 
            _characterAnimator.enabled = false;
            _characterAnimator.enabled = true;
            _hasBoundData = false;
        }

        public void PreviewFromDropdownItem(Transform contentTransform)
        {
            var index = contentTransform.GetSiblingIndex() - 1; // -1 because of "item" from dropdown is present
            PlayCLipAtIndex(index);
        }

        public void PlayCurrentDropdownIndex()
        {
            var index = _dropdown.value;
            PlayCLipAtIndex(index);
        }

        private List<AnimationClipGroup> CopyClips(AnimationClipGroup[] sourceClipGroups)
        {
            var newClipGroups = new List<AnimationClipGroup>();
            for (int i = 0; i < sourceClipGroups.Length; i++)
            {
                var clipGroup = sourceClipGroups[i];
                var downClip = Instantiate(clipGroup.DownAnimationClip);
                downClip.wrapMode = WrapMode.Loop;
                var sideClip = Instantiate(clipGroup.SideAnimationClip);
                sideClip.wrapMode = WrapMode.Loop;
                var upClip = Instantiate(clipGroup.UpAnimationClip);
                upClip.wrapMode = WrapMode.Loop;
                newClipGroups.Add(new AnimationClipGroup(clipGroup.GroupName, downClip, sideClip, upClip));
            }

            return newClipGroups;
        }

        private void CreatePlayables()
        {
            _playableGraph = PlayableGraph.Create();
            _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            _playableOutputDown = AnimationPlayableOutput.Create(_playableGraph, "Animation Down", _characterAnimator);
            _playableOutputSide = AnimationPlayableOutput.Create(_playableGraph, "Animation Side", _characterAnimator);
            _playableOutputUp = AnimationPlayableOutput.Create(_playableGraph, "Animation Up", _characterAnimator);
        }

        private void CreateDropdownOptions()
        {
            _dropdown.ClearOptions();

            for (int i = 0; i < _animationClipGroups.Count; i++)
            {
                var clipGroup = "None";

                if (_animationClipGroups[i] != null)
                    clipGroup = _animationClipGroups[i].GroupName;

                var option = new Dropdown.OptionData(clipGroup);
                _dropdown.options.Add(option);
            }

            _dropdown.RefreshShownValue();
        }

        private void OnDropdownValueChanged(int index)
        {
            PlayCLipAtIndex(index);
        }

        private void RestartClips()
        {
            _downPlayable.SetTime(0);
            _sidePlayable.SetTime(0);
            _upPlayable.SetTime(0);
        }

        private void PlayCLipAtIndex(int index)
        {
            currentIndex = index;
            SampleDefaultPoses();

            var clipGroup = _animationClipGroups[index];
            if (clipGroup == null)
            {
                _playableGraph.Stop();
                return;
            }

            _downPlayable = AnimationClipPlayable.Create(_playableGraph, clipGroup.DownAnimationClip);
            _sidePlayable = AnimationClipPlayable.Create(_playableGraph, clipGroup.SideAnimationClip);
            _upPlayable = AnimationClipPlayable.Create(_playableGraph, clipGroup.UpAnimationClip);
            _playableOutputDown.SetSourcePlayable(_downPlayable);
            _playableOutputSide.SetSourcePlayable(_sidePlayable);
            _playableOutputUp.SetSourcePlayable(_upPlayable);
            _playableGraph.Play();
        }

        private void SampleDefaultPoses()
        {
            _defaultPoseAnimationClipGroup.DownAnimationClip.SampleAnimation(_characterAnimator.gameObject, 0);
            _defaultPoseAnimationClipGroup.SideAnimationClip.SampleAnimation(_characterAnimator.gameObject, 0);
            _defaultPoseAnimationClipGroup.UpAnimationClip.SampleAnimation(_characterAnimator.gameObject, 0);
        }

        private void DestroyPlayable()
        {
            _playableGraph.Destroy();
        }
    }
}