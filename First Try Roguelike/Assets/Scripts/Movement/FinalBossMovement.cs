using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossMovement : MonoBehaviour
{

    [SerializeField] private float movementTolerance = 0.3f;
    [SerializeField] private Renderer _renderer;
    private CharactersAnimator movementAnimator;

    private float movementSpeed;
    private Rigidbody2D _rigidBody;
    private Vector2 movement;
    private readonly Vector2[] _directions = { Vector2.right, Vector2.left, Vector2.up, Vector2.down };
    private Vector2[] attackPointPositions;
    private Transform _attackPoint;

    private HealthBarEnemies _healthBar;
    private Enemy _enemy;
    private Transform currentPlayerTransform;
    private float attackingDistance;
    private float rangeDistance;

    private enum MovementStrategy { Close, Range };
    private MovementStrategy currentMovementStrategy = MovementStrategy.Range;

    private const float FINAL_BOSS_SPEED_BOOST = 1.2f;


    public void SetMovementSpeed(float speed)
    {
        movementSpeed = speed;
    }

    public float GetMovementSpeed()
    {
        return movementSpeed;
    }

    protected Vector2 GetMovementDirection(Vector2 movement)
    {
        if (movement == Vector2.zero) return Vector2.zero;
        return getMostAccurateDirectionForCurrentMovement(movement);
    }

    private Vector2 getMostAccurateDirectionForCurrentMovement(Vector2 movementInput)
    {
        var maxDot = -Mathf.Infinity;
        var ret = Vector3.zero;

        for (int i = 0; i < _directions.Length; i++)
        {
            var t = Vector3.Dot(movementInput, _directions[i]);
            if (t > maxDot)
            {
                ret = _directions[i];
                maxDot = t;
            }
        }

        return ret;
    }

    protected void moveAttackPointToDirection(Vector2 direction)
    {
        if (direction == Vector2.zero) return;

        if (direction == Vector2.down)
        {
            _attackPoint.localPosition = attackPointPositions[0];
            _attackPoint.up = Vector2.down;
        }
        if (direction == Vector2.up)
        {
            _attackPoint.localPosition = attackPointPositions[1];
            _attackPoint.up = Vector2.up;
        }
        if (direction == Vector2.right)
        {
            _attackPoint.localPosition = attackPointPositions[2];
            _attackPoint.up = Vector2.right;
        }
        if (direction == Vector2.left)
        {
            _attackPoint.localPosition = attackPointPositions[3];
            _attackPoint.up = Vector2.left;
        }
    }

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        _healthBar = _enemy.GetHealthBar();
        movementAnimator = GetComponent<CharactersAnimator>();
        movementAnimator.ResetRigs();
        _attackPoint = transform.Find("AttackPoint");
        // Set vector2 to enemy attackpoints
        attackPointPositions = new Vector2[] { new Vector2(0f, -0.2f), new Vector2(0f, 2f), new Vector2(0.8f, 0.5f), new Vector2(-0.8f, 0.5f) };
        _attackPoint.localPosition = attackPointPositions[0];
    }

    protected bool CanMove()
    {
        return _renderer.isVisible;
    }

    protected Vector2 GetMovementValues()
    {
        Vector2 movement = Vector2.zero;
        Vector2 distanceVector = currentPlayerTransform.position - transform.position;
        if (distanceVector.x < -movementTolerance) movement.x = -1;
        if (distanceVector.x > movementTolerance) movement.x = 1;
        if (distanceVector.y < -movementTolerance) movement.y = -1;
        if (distanceVector.y > movementTolerance) movement.y = 1;
        return movement;
    }

    private void Update()
    {
        if (currentPlayerTransform.Equals(null)) movement = Vector2.zero;
        else movement = GetMovementValues();
    }

    protected virtual Vector2 getNewPosition()
    {
        return _rigidBody.position + movement * movementSpeed * Time.fixedDeltaTime;
    }

    protected void moveEnemy(Vector2 newPosition)
    {
        //Move the enemy
        _rigidBody.MovePosition(newPosition);
        Vector2 direction = GetMovementDirection(movement);
        moveAttackPointToDirection(direction);
        movementAnimator.HandleMovementAnimation(direction, movementSpeed);
        //Update the location of the enemy healthbar on screen
        UpdateHealthbarPosition(direction);
    }

    private void UpdateHealthbarPosition(Vector2 direction)
    {
        if (direction.Equals(Vector2.left) || direction.Equals(Vector2.right)) return;
        if (direction.Equals(Vector2.up)) _healthBar.setBottomPosition(_rigidBody.position);
        if (direction.Equals(Vector2.down)) _healthBar.setTopPosition(_rigidBody.position);
    }

    internal void SetPlayerTransform(Transform playerTransform)
    {
        currentPlayerTransform = playerTransform;
    }

    internal void SetAttackingDistance(float distance)
    {
        attackingDistance = distance;
    }
  
    internal void SetRangeDistance(float distance)
    {
        rangeDistance = distance;
    }

    private void FixedUpdate()
    {
        if (!CanMove()) return;
        if (currentPlayerTransform == null) return;
        
        Vector2 newPosition = Vector2.zero;
        
        if (currentMovementStrategy == MovementStrategy.Close) newPosition = getNewPosition();
        else if (currentMovementStrategy == MovementStrategy.Range) newPosition = getNewRangePosition();
        
        moveEnemy(newPosition);
    }

    private Vector2 getNewRangePosition()
    {
        Vector2 appliedMovement = Vector2.zero;
        float playerEnemyDistance = Vector2.Distance(transform.position, currentPlayerTransform.position);
        if (playerEnemyDistance > rangeDistance) appliedMovement = movement;
        if (playerEnemyDistance < rangeDistance) appliedMovement = movement * -1;
        return _rigidBody.position + appliedMovement * movementSpeed * Time.fixedDeltaTime;
    }

    internal void UpdateBossState(FinalBossState currentBossState)
    {
        if (currentBossState == FinalBossState.MeleeAttacker)
        {   
            currentMovementStrategy = MovementStrategy.Close;
            //TODO: Maybe add speed boost but when i tried i broke the laws of physics
            //SetMovementSpeed(GetMovementSpeed() * FINAL_BOSS_SPEED_BOOST);
        }
        else if (currentBossState == FinalBossState.SpellCasterAttacker) currentMovementStrategy = MovementStrategy.Range;
    }
}
