using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class EnemyMovement : EntityMovement
{
    [SerializeField] private float movementTolerance = 0.3f;
    [SerializeField] private Renderer _renderer;
    private HealthBarEnemies _healthBar;
    private Enemy _enemy;
    protected Transform currentPlayerTransform;
    protected float attackingDistance;

    private void Awake() {
        _rigidBody = GetComponent<Rigidbody2D>();
        _enemy = GetComponent<Enemy>();
    }

    private void Start() {
        _healthBar = _enemy.GetHealthBar(); 
        movementAnimator = GetComponent<CharactersAnimator>();
        movementAnimator.ResetRigs();
        _attackPoint = transform.Find("AttackPoint");
        // Set vector2 to enemy attackpoints
        attackPointPositions = new Vector2[] { new Vector2(0f, -0.2f), new Vector2(0f, 2f), new Vector2(0.8f, 0.5f), new Vector2(-0.8f, 0.5f) };
        _attackPoint.localPosition = attackPointPositions[0];
    }

    private bool CanMove(){
        return _renderer.isVisible;
    }

    private Vector2 GetMovementValues(){
        Vector2 movement = Vector2.zero;
        Vector2 distanceVector = currentPlayerTransform.position - transform.position;
        if (distanceVector.x < -movementTolerance)  movement.x = -1;
        if (distanceVector.x >  movementTolerance)  movement.x =  1;
        if (distanceVector.y < -movementTolerance)  movement.y = -1;
        if (distanceVector.y >  movementTolerance)  movement.y =  1;
        return movement;        
    }

    private void Update() 
    {
        if (currentPlayerTransform.Equals(null)) movement = Vector2.zero;
        else movement = GetMovementValues();   
    }   

    private void FixedUpdate()
    {
        if (!CanMove()) return;
        Vector2 newPosition = getNewPosition();
        moveEnemy(newPosition);
    }

    protected virtual Vector2 getNewPosition() { 
        return _rigidBody.position + movement * movementSpeed * Time.fixedDeltaTime; 
    }
    
    private void moveEnemy(Vector2 newPosition)
    {
        //Move the enemy
        _rigidBody.MovePosition(newPosition);
        Vector2 direction = GetMovementDirection(movement);
        moveAttackPointToDirection(direction);
        movementAnimator.HandleMovementAnimation(direction,movementSpeed);
        //Update the location of the enemy healthbar on screen
        UpdateHealthbarPosition(direction);
    }

    private void UpdateHealthbarPosition(Vector2 direction){
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
}
