using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : EntityMovement
{
    private Vector2 currentMousePosition;
    private ShootProyectile _shootProyectile;
    private Entity _entity;

    private void Awake() {
        _rigidBody = GetComponent<Rigidbody2D>();
        _shootProyectile = GetComponent<ShootProyectile>();
        _entity = GetComponent<Entity>();
    }
    
    // Update is called once per frame
    void Update()
    {
        //Works with WASD and Arrow Keys
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        //Get mouse position
        currentMousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);

        //this script now also checks if the fire key was pressed in order to shoot the proyectile
        if(Input.GetButtonDown("Fire1") && (_entity.GetMana() >= 5) && (PauseMenu.GameIsPaused == false)) {
            _entity.SpendMana(5);
            _shootProyectile.Shoot();
            }
    }

    //Updates at constant time interval regardless of Frames Per Second
    private void FixedUpdate() 
    {
        _rigidBody.MovePosition(_rigidBody.position + movement * movementSpeed * Time.fixedDeltaTime);
        
        LookToPosition(currentMousePosition);
    }
}
