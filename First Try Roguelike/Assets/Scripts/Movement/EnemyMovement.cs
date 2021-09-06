using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : EntityMovement
{
  public GameObject target;
  private Vector2 currentTargetPosition;
  private bool isVisible;
  
  private void OnBecameVisible() {
      isVisible = true;
  }

  private void OnBecameInvisible() {
      isVisible = false;
  }
  private void Update() 
  {   
      //It will only start moving towards the player if you faced the enemy on screen
      if(isVisible && target){
        movement = target.transform.position - transform.position;
        //prevents the enemy entity from accelerating and deaccelerating regarding its distance from the player
        movement.Normalize();
        currentTargetPosition = target.transform.position;
      }
      else movement = Vector2.zero;  
  }

  private void FixedUpdate() 
  {
      _rigidBody.MovePosition(_rigidBody.position + movement * movementSpeed * Time.fixedDeltaTime);
      //TO DEBUG: Enemy looks on opposite direction
      LookToPosition(currentTargetPosition);
  }

}
