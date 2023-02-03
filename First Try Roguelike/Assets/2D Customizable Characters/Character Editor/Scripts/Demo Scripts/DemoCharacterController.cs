using System;
using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

namespace CustomizableCharacters.CharacterEditor.Demo
{
    /// <summary>
    /// Character controller used for demonstrating some functionality of the customizable character. Made for demo purpose only.
    /// </summary>
    public class DemoCharacterController : MonoBehaviour, ICharacterController
    {
        private enum State
        {
            Moving,
            Attacking,
            Shooting,
        }

        [Header("References")]
        [SerializeField] private CustomizableCharacter _customizableCharacter;
        [SerializeField] private CharacterExpression _characterExpression;
        [SerializeField] private Arrow _arrowPrefab;
        [SerializeField] private CustomizationCategory _meleeWeaponCategory;
        [SerializeField] private CustomizationCategory _shieldCategory;
        [SerializeField] private CustomizationCategory _bowCategory;
        [SerializeField] private ExpressionData[] _expressions = Array.Empty<ExpressionData>();
        [SerializeField] private Transform _attackHeightReference;
        [SerializeField] private Animator _animator;
        
        
        [Header("Inputs")]
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        [SerializeField] private InputAction _horizontalInput;
        [SerializeField] private InputAction _verticalInput;
        [SerializeField] private InputAction _attackInput;
        [SerializeField] private InputAction _shootingInput;
        [SerializeField] private InputAction _walkInput;
        [SerializeField] private InputAction _sheathInput;
        [SerializeField] private InputAction _cycleExpressionInput;
#else
        [SerializeField] private string _horizontalInput = "Horizontal";
        [SerializeField] private string _verticalInput = "Vertical";
        [SerializeField] private string _attackInput = "Fire1";
        [SerializeField] private string _shootingInput = "Fire2";
        [SerializeField] private KeyCode _walkInput = KeyCode.LeftShift;
        [SerializeField] private KeyCode _sheathInput = KeyCode.Return;
        [SerializeField] private KeyCode _cycleExpressionInput = KeyCode.Space;
#endif
        [Header("Settings")]
        [SerializeField] private float _acceleration;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private bool _isWeaponSheathed;

        private readonly Vector2[] _directions = { Vector2.right, Vector2.left, Vector2.up, Vector2.down };

        private CharacterControllerInputs _inputs;
        private State _currentState;
        private Vector2 _previousDirection;
        private GameObject _currentDirectionGameObject;
        private int _animatorDirection;
        private Vector2 _currentMovement;
        private float _currentAcceleration;
        private Vector2 _lastActiveAxis;
        private int _expressionIndex;
        private bool _hasMeleeWeapon;
        private bool _hasBow;
        private bool _hasShield;
        private Coroutine _currentCoroutine;
        private bool _isDisabled;
        private readonly string[] _attackTriggers = new string[] { "Attack 1", "Stab" };

        #region Unity Methods

        private void Awake()
        {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            _inputs = new InputSystemInputs(_horizontalInput, _verticalInput, _attackInput, _shootingInput, _walkInput,
                _cycleExpressionInput, _sheathInput);
#else
            _inputs = new LegacyInputs(_horizontalInput, _verticalInput, _attackInput, _shootingInput, _walkInput,
                _cycleExpressionInput, _sheathInput);
#endif

            if (_inputs == null)
                Debug.LogWarning(
                    "No inputs available. The character controller only supports the legacy input system at the moment." +
                    " Make sure it's enabled in the project settings");
        }

        private void Start()
        {
            if (!_isDisabled)
                EnableController();
        }

        private void Update()
        {
            HandleInput();

            if (_inputs.InputAttacked && _hasMeleeWeapon && _currentState == State.Moving)
            {
                HandleAttack();
            }
            else if (_inputs.InputShooting && _hasBow && _currentState == State.Moving)
            {
                HandleShooting();
            }

            HandleLocomotion();
            Move(_currentMovement);

            if (_inputs.InputCycleExpression)
                HandleCharacterExpression();

            if (_inputs.InputSheathed)
            {
                SetWeaponSheathed(!_isWeaponSheathed);
                HandleAnimator();
                _inputs.ResetSheath();
            }
        }

        #endregion

        public void EnableController()
        {
            enabled = true;
            _isDisabled = false;

            var customizer = _customizableCharacter.Customizer;
            customizer.AddedCustomization += OnCustomizerAddedCustomization;
            customizer.RemovedCustomization += OnCustomizerRemovedCustomization;

            CheckForWeapons();
            HandleWeaponVisibility();

            ResetRigs();
            ResetInputs();
            HandleAnimator();
            ChangeState(State.Moving);

            _previousDirection = Vector2.zero;
            HandleDirection();
        }

        public void DisableController()
        {
            _isDisabled = true;
            _customizableCharacter.Customizer.AddedCustomization -= OnCustomizerAddedCustomization;
            _customizableCharacter.Customizer.RemovedCustomization -= OnCustomizerRemovedCustomization;

            if (_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);

            HandleWeaponVisibility();
            _characterExpression.SetToDefault();
            enabled = false;
        }

        private void CheckForWeapons()
        {
            var customizer = _customizableCharacter.Customizer;
            _hasMeleeWeapon = customizer.GetCustomizationDataInCategory(_meleeWeaponCategory) != null;
            _hasShield = customizer.GetCustomizationDataInCategory(_shieldCategory) != null;
            _hasBow = customizer.GetCustomizationDataInCategory(_bowCategory) != null;
        }

        private void OnCustomizerRemovedCustomization(Customization customization)
        {
            if (_hasMeleeWeapon && customization.CustomizationData.Category == _meleeWeaponCategory
                || _hasBow && customization.CustomizationData.Category == _bowCategory)
            {
                CheckForWeapons();
                HandleWeaponVisibility();
            }
        }

        private void OnCustomizerAddedCustomization(Customization customization)
        {
            if (!_hasMeleeWeapon && customization.CustomizationData.Category == _meleeWeaponCategory
                || !_hasBow && customization.CustomizationData.Category == _bowCategory)
            {
                CheckForWeapons();
                HandleWeaponVisibility();
            }
        }

        private void ResetRigs()
        {
            _customizableCharacter.UpRig.SetActive(false);
            _customizableCharacter.SideRig.SetActive(false);
            _customizableCharacter.DownRig.SetActive(false);
        }

        private void ResetInputs()
        {
            _inputs.Reset();

            _lastActiveAxis = Vector2.down;
        }

        private void HandleAnimator()
        {
            var showingWeapon = _hasMeleeWeapon && !_isWeaponSheathed || _hasShield && !_isWeaponSheathed;
            _animator.SetBool("Showing Weapon", showingWeapon);
        }

        private void HandleWeaponVisibility()
        {
            if (_isDisabled)
            {
                if (_hasMeleeWeapon)
                    SetWeaponSheathed(false);

                if (_hasBow)
                    _customizableCharacter.Customizer.ShowCategory(_bowCategory);
            }
            else
            {
                if (_hasMeleeWeapon)
                    SetWeaponSheathed(false);

                if (_hasBow)
                    _customizableCharacter.Customizer.HideCategory(_bowCategory);
            }
        }

        private void ChangeState(State state)
        {
            _currentState = state;
        }

        private void HandleCharacterExpression()
        {
            _inputs.ResetCycleExpressions();

            if (_expressionIndex >= _expressions.Length)
            {
                _expressionIndex = 0;
                _characterExpression.SetToDefault();
            }
            else
            {
                _characterExpression.SetExpression(_expressions[_expressionIndex]);
                _expressionIndex++;
            }
        }

        private void Move(Vector2 amount)
        {
            transform.position += (Vector3)(amount * Time.deltaTime);
        }

        private Vector2 GetClosestDirection(Vector2 from)
        {
            var maxDot = -Mathf.Infinity;
            var ret = Vector3.zero;

            for (int i = 0; i < _directions.Length; i++)
            {
                var t = Vector3.Dot(from, _directions[i]);
                if (t > maxDot)
                {
                    ret = _directions[i];
                    maxDot = t;
                }
            }

            return ret;
        }

        private void ShootArrow()
        {
            var arrow = Instantiate(_arrowPrefab);
            var height = _attackHeightReference.position.y - transform.position.y;
            arrow.transform.position = transform.position + (Vector3)_previousDirection;
            arrow.SetHeight(height);
            arrow.StartMoving(_previousDirection);
        }

        private void SetWeaponSheathed(bool isSheathed)
        {
            if (_isWeaponSheathed == isSheathed)
                return;

            _isWeaponSheathed = isSheathed;

            if (_hasMeleeWeapon)
            {
                if (_isWeaponSheathed)
                    _customizableCharacter.Customizer.HideCategory(_meleeWeaponCategory);
                else
                    _customizableCharacter.Customizer.ShowCategory(_meleeWeaponCategory);
            }

            if (_hasShield)
            {
                if (_isWeaponSheathed)
                    _customizableCharacter.Customizer.HideCategory(_shieldCategory);
                else
                    _customizableCharacter.Customizer.ShowCategory(_shieldCategory);
            }
        }

        #region Handlers

        private void HandleInput()
        {
            _inputs.UpdateInputs();
        }

        private void HandleDirection()
        {
            var direction = GetClosestDirection(_inputs.MoveAxis);
            if (direction == _previousDirection)
                return;

            _currentDirectionGameObject?.SetActive(false);

            if (direction == Vector2.right)
            {
                _currentDirectionGameObject = _customizableCharacter.SideRig;
                var scale = _customizableCharacter.SideRig.transform.localScale;
                scale.x = Mathf.Abs(scale.x);
                _currentDirectionGameObject.transform.localScale = scale;
                _animatorDirection = 1;
            }
            else if (direction == Vector2.left)
            {
                _currentDirectionGameObject = _customizableCharacter.SideRig;
                var scale = _customizableCharacter.SideRig.transform.localScale;
                scale.x = Mathf.Abs(scale.x) * -1;
                _currentDirectionGameObject.transform.localScale = scale;
                _animatorDirection = 1;
            }
            else if (direction == Vector2.up)
            {
                _currentDirectionGameObject = _customizableCharacter.UpRig;
                _animatorDirection = 0;
            }
            else if (direction == Vector2.down)
            {
                _currentDirectionGameObject = _customizableCharacter.DownRig;
                _animatorDirection = 2;
            }

            _currentDirectionGameObject?.SetActive(true);
            _previousDirection = direction;
        }

        private void HandleAttack()
        {
            ChangeState(State.Attacking);

            if (_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);
            _currentCoroutine = StartCoroutine(DoAttack());
        }

        private void HandleShooting()
        {
            ChangeState(State.Shooting);

            if (_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);
            _currentCoroutine = StartCoroutine(DoShoot());
        }

        private void HandleLocomotion()
        {
            var accelerationTarget = _inputs.InputWalking ? 0.5f : 1f;
            var maxAcceleration = _acceleration;

            if (_inputs.MoveAxis != Vector2.zero && _currentState == State.Moving)
            {
                if (_inputs.InputWalking)
                    maxAcceleration /= 2;

                HandleDirection();
                _currentAcceleration =
                    Mathf.MoveTowards(_currentAcceleration, accelerationTarget, maxAcceleration * Time.deltaTime);
                _lastActiveAxis = _inputs.MoveAxis;
            }
            else
            {
                maxAcceleration /= 2;
                _currentAcceleration = Mathf.MoveTowards(_currentAcceleration, 0, maxAcceleration * Time.deltaTime);
            }

            var targetSpeed = _moveSpeed * _currentAcceleration;
            _currentMovement = _lastActiveAxis * targetSpeed;

            var animatorSpeed = Mathf.InverseLerp(0, _moveSpeed, targetSpeed);
            _animator.SetFloat("Speed", animatorSpeed);
            _animator.SetFloat("Direction", _animatorDirection);
        }

        #endregion

        #region Coroutines

        private IEnumerator DoAttack()
        {
            var wasSheathed = _isWeaponSheathed;
            SetWeaponSheathed(false);

            var currentAttack = 0;
            while (_inputs.InputAttacked && currentAttack < _attackTriggers.Length)
            {
                _animator.SetTrigger(_attackTriggers[currentAttack]);

                // make sure each attack is going in the direction player is pressing
                if (_inputs.MoveAxis != Vector2.zero)
                {
                    _lastActiveAxis = _inputs.MoveAxis;
                    HandleDirection();
                    _currentAcceleration = 1.2f;
                }
                else
                    _currentAcceleration = 0.8f;

                // wait for animator state to change to attack
                while (_animator.GetCurrentAnimatorStateInfo(0).IsName(_attackTriggers[currentAttack]) == false)
                    yield return null;

                _animator.ResetTrigger(_attackTriggers[currentAttack]);

                while (_animator.GetCurrentAnimatorStateInfo(0).IsName(_attackTriggers[currentAttack])
                       && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
                {
                    // prevent too early attack input
                    if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
                        _inputs.ResetAttack();

                    yield return null;
                }

                currentAttack++;
            }

            SetWeaponSheathed(wasSheathed);
            ChangeState(State.Moving);
        }

        private IEnumerator DoShoot()
        {
            var wasSheathed = _isWeaponSheathed;
            SetWeaponSheathed(true);
            _customizableCharacter.Customizer.ShowCategory(_bowCategory);

            while (_inputs.InputShooting)
            {
                if (_inputs.MoveAxis != Vector2.zero)
                {
                    _lastActiveAxis = _inputs.MoveAxis;
                    HandleDirection();
                }

                // enter animator to enter bow load state
                _animator.SetTrigger("Bow Load");
                while (_animator.GetCurrentAnimatorStateInfo(0).IsName("Bow Load") == false)
                    yield return null;

                _animator.ResetTrigger("Bow Load");

                // wait for bow load animation
                while (_animator.GetCurrentAnimatorStateInfo(0).IsName("Bow Load")
                       && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
                {
                    yield return null;
                }

                // enter animator to enter bow release state
                _animator.SetTrigger("Bow Release");
                while (_animator.GetCurrentAnimatorStateInfo(0).IsName("Bow Release") == false)
                    yield return null;

                _animator.ResetTrigger("Bow Release");

                ShootArrow();
                _inputs.ResetShoot();

                while (_animator.GetCurrentAnimatorStateInfo(0).IsName("Bow Release")
                       && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
                {
                    if (_inputs.InputShooting && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f)
                        break;
                    yield return null;
                }
            }

            SetWeaponSheathed(wasSheathed);
            _customizableCharacter.Customizer.HideCategory(_bowCategory);
            ChangeState(State.Moving);
        }

        #endregion
    }
}