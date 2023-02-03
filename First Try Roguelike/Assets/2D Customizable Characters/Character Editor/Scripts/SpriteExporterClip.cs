using UnityEngine;

namespace CustomizableCharacters.CharacterEditor
{
    public class SpriteExporterClip
    {
        public SpriteExporterClip(AnimationClip animationClip, GameObject rigGameObject)
        {
            AnimationClip = animationClip;
            RigGameObject = rigGameObject;
        }

        public AnimationClip AnimationClip;
        public GameObject RigGameObject;
    }
}