using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 5f;

    private Rigidbody2D _rigidBody;
    private Camera _camera;

    private Vector2 movement;
    private Vector2 currentMousePosition;
    private ShootProyectile _shootProyectile;

    private void Awake() {
        _rigidBody = GetComponent<Rigidbody2D>();
        _shootProyectile = GetComponent<ShootProyectile>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
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
        if(Input.GetButtonDown("Fire1")) _shootProyectile.Shoot();
    }

    //Updates at constant time interval regardless of Frames Per Second
    private void FixedUpdate() 
    {
        _rigidBody.MovePosition(_rigidBody.position + movement * movementSpeed * Time.fixedDeltaTime);

        Vector2 lookDirection = currentMousePosition - _rigidBody.position;
        float angle = Mathf.Atan2(lookDirection.y,lookDirection.x) * Mathf.Rad2Deg - 90f;

        _rigidBody.rotation = angle;
    }
}
