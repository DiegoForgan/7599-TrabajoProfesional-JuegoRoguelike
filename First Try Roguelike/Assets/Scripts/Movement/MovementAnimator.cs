using CustomizableCharacters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAnimator : MonoBehaviour
{
    [SerializeField] private CustomizableCharacter customizableCharacter;
    [SerializeField] private Animator animator;
    private readonly Vector2[] _directions = { Vector2.right, Vector2.left, Vector2.up, Vector2.down };
    private Vector2 previousDirection;
    private GameObject currentDirectionGameObject;
    private int animatorDirection;

    public void ResetRigs()
    {
        customizableCharacter.UpRig.SetActive(false);
        customizableCharacter.SideRig.SetActive(false);
        customizableCharacter.DownRig.SetActive(false);
    }
    public void HandleMovementAnimation(float currentSpeed, float movementSpeed, Vector2 movement)
    {
        HandleDirection(movement);
        HandleSpeed(currentSpeed,movementSpeed);
    }
    private void HandleSpeed(float currentSpeed, float movementSpeed) {
        float animatorSpeed = Mathf.InverseLerp(0, currentSpeed, movementSpeed);
        animator.SetFloat("Speed", animatorSpeed);
    }

    private void HandleDirection(Vector2 movement)
    {
        if (movement == Vector2.zero) return;

        Vector2 direction = getMostAccurateDirectionForCurrentMovement(movement);
        
        if (direction == previousDirection) return;

        currentDirectionGameObject?.SetActive(false);

        if (direction == Vector2.right)
        {
            currentDirectionGameObject = customizableCharacter.SideRig;
            var scale = customizableCharacter.SideRig.transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            currentDirectionGameObject.transform.localScale = scale;
            animatorDirection = 1;
        }
        else if (direction == Vector2.left)
        {
            currentDirectionGameObject = customizableCharacter.SideRig;
            var scale = customizableCharacter.SideRig.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * -1;
            currentDirectionGameObject.transform.localScale = scale;
            animatorDirection = 1;
        }
        else if (direction == Vector2.up)
        {
            currentDirectionGameObject = customizableCharacter.UpRig;
            animatorDirection = 0;
        }
        else if (direction == Vector2.down)
        {
            currentDirectionGameObject = customizableCharacter.DownRig;
            animatorDirection = 2;
        }

        currentDirectionGameObject?.SetActive(true);
        previousDirection = direction;

        animator.SetFloat("Direction", animatorDirection);
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
}
