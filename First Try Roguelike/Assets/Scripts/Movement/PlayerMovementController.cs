using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : EntityMovement
{

    private MovementInput movementInput = new MovementInput();

    // Update is called once per frame
    void Update()
    {
        movementInput.updateInputs();
    }

    //Updates at constant time interval regardless of Frames Per Second
    private void FixedUpdate() 
    {
        movement = movementInput.getMovement();
        float appliedSpeed = movementInput.isRunningKeyPressed() ? movementSpeed * 1.5f : movementSpeed; 
        
        _rigidBody.MovePosition(_rigidBody.position + movement * appliedSpeed * Time.fixedDeltaTime);
    }
}
