using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    public float movementSpeed = 2.5f;

    protected Rigidbody2D _rigidBody;
    protected Camera _camera;
    protected Vector2 movement;

    private void Awake() {
        _rigidBody = GetComponent<Rigidbody2D>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
    }

    protected void LookToPosition(Vector2 positionToLook)
    {
        Vector2 lookDirection = positionToLook - _rigidBody.position;
        float angle = Mathf.Atan2(lookDirection.y,lookDirection.x) * Mathf.Rad2Deg - 90f;

        _rigidBody.rotation = angle;
    }
}
