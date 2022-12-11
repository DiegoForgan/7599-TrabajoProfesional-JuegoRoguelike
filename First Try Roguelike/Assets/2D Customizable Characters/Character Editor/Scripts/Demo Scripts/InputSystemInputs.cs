using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

namespace CustomizableCharacters.CharacterEditor.Demo
{
    public class InputSystemInputs : CharacterControllerInputs
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        public InputSystemInputs(InputAction horizontalInput, InputAction verticalInput, InputAction attackInput,
            InputAction shootingInput,
            InputAction walkInput, InputAction cycleExpressionInput, InputAction sheathedInput)
        {
            _horizontalInput = horizontalInput;
            _verticalInput = verticalInput;
            _attackInput = attackInput;
            _shootingInput = shootingInput;
            _walkInput = walkInput;
            _cycleExpressionInput = cycleExpressionInput;
            _sheathedInput = sheathedInput;
            
            TrySetInputDefaults();
            
            _horizontalInput.Enable();
            _verticalInput.Enable();
            _attackInput.Enable();
            _shootingInput.Enable();
            _walkInput.Enable();
            _sheathedInput.Enable();
            _cycleExpressionInput.Enable();
        }

        private InputAction _horizontalInput;
        private InputAction _verticalInput;
        private InputAction _attackInput;
        private InputAction _shootingInput;
        private InputAction _walkInput;
        private InputAction _sheathedInput;
        private InputAction _cycleExpressionInput;


        private void TrySetInputDefaults()
        {
            if (_horizontalInput.bindings.Count == 0)
                _horizontalInput.AddCompositeBinding("Axis")
                    .With("Positive", "<Keyboard>/d")
                    .With("Negative", "<Keyboard>/a");;
            
            if (_verticalInput.bindings.Count == 0)
                _verticalInput.AddCompositeBinding("Axis")
                    .With("Positive", "<Keyboard>/w")
                    .With("Negative", "<Keyboard>/s");;
            
            if (_attackInput.bindings.Count == 0)
                _attackInput.AddBinding("<Mouse>/leftButton");
            if (_shootingInput.bindings.Count == 0)
                _shootingInput.AddBinding("<Mouse>/rightButton");
            if (_walkInput.bindings.Count == 0)
                _walkInput.AddBinding("<Keyboard>/shift");
            if (_sheathedInput.bindings.Count == 0)
                _sheathedInput.AddBinding("<Keyboard>/enter");
            if (_cycleExpressionInput.bindings.Count == 0)
                _cycleExpressionInput.AddBinding("<Keyboard>/space");
        }
#endif
        public override void UpdateInputs()
        {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            var horizontal = _horizontalInput.ReadValue<float>();
            var vertical = _verticalInput.ReadValue<float>();
            MoveAxis = new Vector2(horizontal, vertical).normalized;

            if (_walkInput.WasPressedThisFrame())
                InputWalking = true;
            else if (_walkInput.WasReleasedThisFrame())
                InputWalking = false;

            if (_shootingInput.WasPressedThisFrame())
                InputShooting = true;
            if (_attackInput.WasPressedThisFrame())
                InputAttacked = true;
            if (_cycleExpressionInput.WasPressedThisFrame())
                InputCycleExpression = true;
            if (_sheathedInput.WasPressedThisFrame())
                InputSheathed = true;
#endif
        }
    }
}