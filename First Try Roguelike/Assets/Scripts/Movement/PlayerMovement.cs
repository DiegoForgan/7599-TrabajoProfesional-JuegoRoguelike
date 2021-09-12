using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : EntityMovement
{
    public PlayerData playerData;
    private Vector2 currentMousePosition;

    private void Start() {
        movementSpeed = playerData.movementSpeed;
    }
    // Update is called once per frame
    void Update()
    {
        //Works with WASD and Arrow Keys
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        //Get mouse position
        currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    //Updates at constant time interval regardless of Frames Per Second
    private void FixedUpdate() 
    {
        _rigidBody.MovePosition(_rigidBody.position + movement * movementSpeed * Time.fixedDeltaTime);
        
        LookToPosition(currentMousePosition);
    }
}
