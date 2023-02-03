using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace CustomizableCharacters
{
    /// <summary>
    /// Changes the scale of transforms by using ScaleGroups.
    /// </summary>
    public class ScaleCustomizer : MonoBehaviour
    {
#if UNITY_EDITOR
        [Header("Settings")]
        [Tooltip(
            "If the component should refresh everything when a change was made. Disable if liveupdate isn't needed or there is any performance issues.")]
        [SerializeField] private bool _refreshOnValidate = true;
#endif
        [Space(10)]
        [SerializeField] private List<ScaleGroup> _scaleGroups = new List<ScaleGroup>();

        [Tooltip(
            "Adjusts hip position relative to the ground. (for example, longer legs moves the hip up)")]
        [SerializeField]
        private HipPositionAdjusterGroup[] _hipPositionAdjusterGroups = Array.Empty<HipPositionAdjusterGroup>();
        public ReadOnlyCollection<ScaleGroup> ScaleGroups => _scaleGroups.AsReadOnly();

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_refreshOnValidate)
                ApplyAllScaleGroups();
        }
#endif

        #region Public Methods

        /// <summary>
        /// Applies scaling of all ScaleGroups.
        /// </summary>
        public void ApplyAllScaleGroups()
        {
            if (_scaleGroups == null)
                return;

            for (int i = 0; i < _scaleGroups.Count; i++)
            {
                var group = _scaleGroups[i];
                ApplyGroup(group);
            }
        }

        /// <summary>
        /// Applies scaling from a ScaleGroup.
        /// </summary>
        /// <param name="scaleGroup"></param>
        public void ApplyGroup(ScaleGroup scaleGroup)
        {
            if (scaleGroup.Transforms.Length == 0)
            {
                Debug.Log($"{scaleGroup.GroupName} doesn't have any transforms, nothing to apply.");
                return;
            }

            float prevYScale = 0;
            if (TryGetHipAdjusterForScaleGroup(scaleGroup.GroupName, out var hipAdjusterGroup))
            {
                prevYScale = scaleGroup.Transforms[0].localScale.y;
            }

            for (int i = 0; i < scaleGroup.Transforms.Length; i++)
            {
                var groupTransform = scaleGroup.Transforms[i];
                if (groupTransform == null)
                    continue;

                var newScale = new Vector3(scaleGroup.Width, scaleGroup.Length, 1);
                newScale *= scaleGroup.Scale;

                if (groupTransform.localScale.x < 0)
                    newScale.x *= -1;

                if (groupTransform.localScale.y < 0)
                    newScale.y *= -1;

                groupTransform.localScale = newScale;
            }

            if (hipAdjusterGroup != null)
            {
                var newYScale = scaleGroup.Transforms[0].localScale.y;
                AdjustHipPosition(hipAdjusterGroup, prevYScale, newYScale);
            }
        }

        /// <summary>
        /// Resets all ScaleGroups values to 1. Note that this doesn't scale any of the transforms, use ApplyGroup after if you want to update the transforms.
        /// </summary>
        public void ResetAllGroups()
        {
            for (int i = 0; i < _scaleGroups.Count; i++)
            {
                var group = _scaleGroups[i];
                group.Reset();
            }
        }

        /// <summary>
        /// Adds and applies a ScaleGroup.
        /// </summary>
        /// <param name="scaleGroup"></param>
        public void AddGroup(ScaleGroup scaleGroup)
        {
            if (_scaleGroups.Contains(scaleGroup))
            {
                Debug.LogWarning($"{nameof(ScaleGroup)} is already added and will not be added");
                return;
            }

            _scaleGroups.Add(scaleGroup);
            ApplyGroup(scaleGroup);
        }

        /// <summary>
        /// Find and returns any (if exists) ScaleGroup with a certain name.
        /// </summary>
        /// <param name="searchName"></param>
        /// <returns></returns>
        public ScaleGroup TryGetScaleGroup(string searchName)
        {
            for (int i = 0; i < _scaleGroups.Count; i++)
            {
                var group = _scaleGroups[i];
                if (group.GroupName == searchName)
                    return group;
            }

            return null;
        }

        #endregion

        #region Private Methods

        private bool TryGetHipAdjusterForScaleGroup(string groupName, out HipPositionAdjusterGroup adjusterGroup)
        {
            for (int i = 0; i < _hipPositionAdjusterGroups.Length; i++)
            {
                var scaleGroup = _hipPositionAdjusterGroups[i];
                if (String.CompareOrdinal(groupName, scaleGroup.ScaleGroupName) == 0)
                {
                    adjusterGroup = scaleGroup;
                    return true;
                }
            }

            adjusterGroup = null;
            return false;
        }

        private void AdjustHipPosition(HipPositionAdjusterGroup adjusterGroup, float previousYScale, float newYScale)
        {
            adjusterGroup.DownAdjuster.Adjust(previousYScale, newYScale);
            adjusterGroup.SideAdjuster.Adjust(previousYScale, newYScale);
            adjusterGroup.UpAdjuster.Adjust(previousYScale, newYScale);
        }

        #endregion
    }
}