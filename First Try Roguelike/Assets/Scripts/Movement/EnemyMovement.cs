using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class EnemyMovement : EntityMovement
{
    [SerializeField] private float movementTolerance = 0.3f;
    [SerializeField] private Renderer _renderer;
    //private HealthBar _healthBar;
    private float offset_y;
    protected Transform currentPlayerTransform;
    protected float attackingDistance;

    private void Awake() {
        _rigidBody = GetComponent<Rigidbody2D>();
        //_enemy = GetComponent<Enemy>();
    }

    private void Start() {
        //_healthBar = _enemy.GetHealthBar();
        //offset_y = (Vector2.Distance(_rigidBody.position,_healthBar.transform.position));
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
        movement = GetMovementValues();   
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
        //UpdateHealthbarPosition();
    }

    private void UpdateHealthbarPosition(){
    //1 - We get the angle that the enemy is facing the player
    float angleFacingPlayer = transform.rotation.eulerAngles.z;
    //2 - Separation beetween the healthbar and the entity
    float offset = offset_y;
    //3 - We check if the health bar must be placed above or below the enemy    
    if(!(angleFacingPlayer > 90 && angleFacingPlayer < 270)) offset = offset_y * -1;
    //4 - Update the health bar accordingly to the enemy movement and previous calculations
    //_healthBar.SetPosition(new Vector2(_rigidBody.position.x,_rigidBody.position.y+offset));
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
