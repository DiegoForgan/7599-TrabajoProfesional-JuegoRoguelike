using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : EntityMovement
{
    private MovementInput movementInput = new MovementInput();
    
    private void Start()
    {
        movementAnimator = GetComponent<CharactersAnimator>();
        movementAnimator.ResetRigs();
        _attackPoint = transform.Find("AttackPoint");
        _attackPoint.localPosition = attackPointPositions[0];
    }

    // Update is called once per frame
    void Update()
    {
        movementInput.updateInputs();
    }

    //Updates at constant time interval regardless of Frames Per Second
    private void FixedUpdate() 
    {
        movement = movementInput.getMovementAxis();
        bool runningKeyPressed = movementInput.isRunningKeyPressed();
        
        float appliedSpeed = runningKeyPressed ? movementSpeed * 1.25f : movementSpeed;
        Vector2 newPosition = _rigidBody.position + movement * appliedSpeed * Time.fixedDeltaTime;
        
        _rigidBody.MovePosition(newPosition);

        float actualMovementSpeed = _rigidBody.velocity.x;
        Vector2 direction = GetMovementDirection(movement);
        moveAttackPointToDirection(direction);
        movementAnimator.HandleMovementAnimation(direction,movementSpeed);
    }
}
