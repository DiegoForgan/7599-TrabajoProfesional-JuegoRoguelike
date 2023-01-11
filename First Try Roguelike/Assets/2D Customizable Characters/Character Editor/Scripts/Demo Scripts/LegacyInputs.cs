using UnityEngine;

namespace CustomizableCharacters.CharacterEditor.Demo
{
    public class LegacyInputs : CharacterControllerInputs
    {
        public LegacyInputs(string horizontalInput, string verticalInput, string attackInput, string shootingInput,
            KeyCode walkInput, KeyCode cycleExpressionInput, KeyCode sheathInput)
        {
            _horizontalInput = horizontalInput;
            _verticalInput = verticalInput;
            _attackInput = attackInput;
            _shootingInput = shootingInput;
            _walkInput = walkInput;
            _cycleExpressionInput = cycleExpressionInput;
            _sheathInput = sheathInput;

            TrySetInputDefaults();
        }

        private string _horizontalInput;
        private string _verticalInput;
        private string _attackInput;
        private string _shootingInput;
        private KeyCode _walkInput;
        private KeyCode _sheathInput;
        private KeyCode _cycleExpressionInput;

        private void TrySetInputDefaults()
        {
            if (string.IsNullOrEmpty(_horizontalInput))
                _horizontalInput = "Horizontal";
            
            if (string.IsNullOrEmpty(_verticalInput))
                _verticalInput = "Vertical";
            
            if (string.IsNullOrEmpty(_attackInput))
                _attackInput = "Fire1";
            
            if (string.IsNullOrEmpty(_shootingInput))
                _shootingInput = "Fire2";
            
            if (_walkInput == KeyCode.None)
                _walkInput = KeyCode.LeftShift;
            
            if (_sheathInput== KeyCode.None)
                _sheathInput = KeyCode.Return;
            
            if (_cycleExpressionInput == KeyCode.None)
                _cycleExpressionInput = KeyCode.Space;
        }

        public override void UpdateInputs()
        {
            var horizontal = Input.GetAxisRaw(_horizontalInput);
            var vertical = Input.GetAxisRaw(_verticalInput);

            MoveAxis = new Vector2(horizontal, vertical).normalized;

            if (Input.GetKeyDown(_walkInput))
                InputWalking = true;
            else if (Input.GetKeyUp(_walkInput))
                InputWalking = false;

            if (Input.GetButtonDown(_shootingInput))
                InputShooting = true;
            if (Input.GetButtonDown(_attackInput))
                InputAttacked = true;
            if (Input.GetKeyDown(_cycleExpressionInput))
                InputCycleExpression = true;
            if (Input.GetKeyDown(_sheathInput))
                InputSheathed = true;
        }
    }
}