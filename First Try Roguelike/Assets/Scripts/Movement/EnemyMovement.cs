using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : EntityMovement
{
  //public EnemyData enemyData;
  public GameObject target;
  private Vector2 currentTargetPosition;
  private bool isVisible;
  private Enemy _enemy;

  private float attackRate;
  private float nextAttackTime = 0;
  private float attackDistance;
  
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


  
  private void Update() 
  {   
      //It will only start moving towards the player if you faced the enemy on screen
      if(isVisible && target){
        movement = target.transform.position - transform.position;
        //prevents the enemy entity from accelerating and deaccelerating regarding its distance from the player
        movement.Normalize();
        currentTargetPosition = target.transform.position;

        //Checks if player is at the distance required for enemy to attack
        float playerEnemyDistance = Vector2.Distance(transform.position,currentTargetPosition);
        if(playerEnemyDistance <= (attackDistance)){
          if(Time.time >= nextAttackTime){
            _enemy.Attack();
            nextAttackTime = Time.time + 1f/attackRate;
          }
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
      _rigidBody.MovePosition(_rigidBody.position + movement * movementSpeed * Time.fixedDeltaTime);
      //TO DEBUG: Enemy looks on opposite direction
      LookToPosition(currentTargetPosition);
  }

}
