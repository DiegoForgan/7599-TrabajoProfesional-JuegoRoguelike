using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityMovement : MonoBehaviour
{
    protected float movementSpeed;
    protected Rigidbody2D _rigidBody;
    protected Vector2 movement;
    private readonly Vector2[] _directions = { Vector2.right, Vector2.left, Vector2.up, Vector2.down };
    protected Vector2[] attackPointPositions;
    protected Transform _attackPoint;
    protected CharactersAnimator movementAnimator;

    private void Awake() {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    public void SetMovementSpeed(float speed){
        movementSpeed = speed;
    }

    public float GetMovementSpeed(){
        return movementSpeed;
    }
    
    protected void LookToPosition(Vector2 positionToLook)
    {
        Vector2 lookDirection = positionToLook - _rigidBody.position;
        float angle = Mathf.Atan2(lookDirection.y,lookDirection.x) * Mathf.Rad2Deg - 90f;

        _rigidBody.rotation = angle;
    }

    protected Vector2 GetMovementDirection(Vector2 movement)
    {
        if (movement == Vector2.zero) return Vector2.zero;
        return getMostAccurateDirectionForCurrentMovement(movement);
    }

    private Vector2 getMostAccurateDirectionForCurrentMovement(Vector2 movementInput)
    {
        var maxDot = -Mathf.Infinity;
        var ret = Vector3.zero;

        for (int i = 0; i < _directions.Length; i++)
        {
            var t = Vector3.Dot(movementInput, _directions[i]);
            if (t > maxDot)
            {
                ret = _directions[i];
                maxDot = t;
            }
        }

        return ret;
    }

    protected void moveAttackPointToDirection(Vector2 direction)
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
