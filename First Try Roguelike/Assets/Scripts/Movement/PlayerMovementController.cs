using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : EntityMovement
{
    private MovementInput movementInput = new MovementInput();
    private CharactersAnimator movementAnimator;
    private readonly Vector2[] attackPointPositions = { new Vector2(0f, -0.2f), new Vector2(0f, 2f), new Vector2(0.8f, 0.5f), new Vector2(-0.8f, 0.5f) };
    private Transform _attackPoint;

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
        movementAnimator.HandleMovementAnimation(direction);
    }

    private void moveAttackPointToDirection(Vector2 direction)
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
}
