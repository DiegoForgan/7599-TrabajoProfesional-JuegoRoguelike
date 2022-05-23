using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : EntityMovement
{
  //public EnemyData enemyData;
  private GameObject target;
  private Vector2 currentTargetPosition;
  private bool isVisible;
  private Enemy _enemy;
  private HealthBar _healthBar;

  private float attackRate;
  private float nextAttackTime = 0;
  private float attackDistance;
  private float offset_y;
  
  private void OnBecameVisible() {
      isVisible = true;
  }

  private void OnBecameInvisible() {
      isVisible = false;
  }

  private void Awake() {
    _rigidBody = GetComponent<Rigidbody2D>();
    _enemy = GetComponent<Enemy>();
  }

  private void Start() {
    _healthBar = _enemy.GetHealthBar();
    offset_y = (Vector2.Distance(_rigidBody.position,_healthBar.transform.position));
    target  = GameObject.FindGameObjectWithTag("Player");
    if (!target) Debug.LogError("No target aquired!");
  }

  private bool CanMove(){
    return (isVisible && target);
  }

  private void GetMovementValues(){
    movement = target.transform.position - transform.position;
    //prevents the enemy entity from accelerating and deaccelerating regarding its distance from the player
    movement.Normalize();
    currentTargetPosition = target.transform.position;
  }

  private bool IsInAttackDistanceAndAbleToAttack(){
    //Checks if player is at the distance required for enemy to attack
    float playerEnemyDistance = Vector2.Distance(transform.position,currentTargetPosition);
    if ((playerEnemyDistance <= attackDistance) && (Time.time >= nextAttackTime)) return true;
    return false;
  }

  
  private void Update() 
  {   
      //It will only start moving towards the player if you faced the enemy on screen
      if(CanMove()){
        GetMovementValues();
        //Checks if player is at the distance required for enemy to attack
        if (IsInAttackDistanceAndAbleToAttack()){
          _enemy.Attack();
          nextAttackTime = Time.time + 1f/attackRate;
        }
      }
      else movement = Vector2.zero;
  }

    internal void SetAttackingParameters(float attRate, float attDistance)
    {
        attackRate = attRate;
        attackDistance = attDistance;
    }

    private void FixedUpdate() 
  {
      if(CanMove()){
        //Move the enemy
        _rigidBody.MovePosition(_rigidBody.position + movement * movementSpeed * Time.fixedDeltaTime);
        //Make the enemy face the player
        LookToPosition(currentTargetPosition);
        //Update the location of the enemy healthbar on screen
        UpdateHealthbarPosition();
      }
  }

  private void UpdateHealthbarPosition(){
    //1 - We get the angle that the enemy is facing the player
    float angleFacingPlayer = transform.rotation.eulerAngles.z;
    //2 - Separation beetween the healthbar and the entity
    float offset = offset_y;
    //3 - We check if the health bar must be placed above or below the enemy    
    if(!(angleFacingPlayer > 90 && angleFacingPlayer < 270)) offset = offset_y * -1;
    //4 - Update the health bar accordingly to the enemy movement and previous calculations
    _healthBar.SetPosition(new Vector2(_rigidBody.position.x,_rigidBody.position.y+offset));
   }

}
