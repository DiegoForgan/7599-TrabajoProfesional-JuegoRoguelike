using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

namespace CustomizableCharacters.CharacterEditor.Demo
{
    public abstract class CharacterControllerInputs
    {
        public bool InputShooting { get; protected set; }
        public bool InputWalking { get; protected set; }
        public bool InputAttacked { get; protected set; }
        public bool InputCycleExpression { get; protected set; }
        public bool InputSheathed { get; protected set; }
        public Vector2 MoveAxis { get; protected set; }

        public abstract void UpdateInputs();

        public void Reset()
        {
            InputShooting = false;
            InputWalking = false;
            InputAttacked = false;
            InputCycleExpression = false;
            InputSheathed = false;
            MoveAxis = Vector2.down;
        }

        public void ResetCycleExpressions() => InputCycleExpression = false;
        public void ResetAttack() => InputAttacked = false;
        public void ResetShoot() => InputShooting = false;
        public void ResetSheath() => InputSheathed = false;
    }
}