using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : EntityMovement
{
    [SerializeField] PlayerData testData;
    private MovementInput movementInput = new MovementInput();
    private MovementAnimator movementAnimator;

    private void Start()
    {
        movementAnimator = GetComponent<MovementAnimator>();
        movementAnimator.ResetRigs();
        //Remove this and replace with custom player data from the player component
        movementSpeed = testData.movementSpeed;
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
        
        float appliedSpeed = runningKeyPressed ? movementSpeed * 1.5f : movementSpeed;
        Vector2 newPosition = _rigidBody.position + movement * appliedSpeed * Time.fixedDeltaTime;
        
        _rigidBody.MovePosition(newPosition);

        float actualMovementSpeed = _rigidBody.velocity.x;
        Debug.Log(actualMovementSpeed);
        movementAnimator.HandleMovementAnimation(appliedSpeed, appliedSpeed, movement);
    }
}
