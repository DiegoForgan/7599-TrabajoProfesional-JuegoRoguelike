using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLongRangeMovement : EnemyMovement
{
    protected override Vector2 getNewPosition() {
        Vector2 appliedMovement = Vector2.zero;
        float playerEnemyDistance = Vector2.Distance(transform.position, currentPlayerTransform.position);
        if (playerEnemyDistance > attackingDistance) appliedMovement = movement;
        if (playerEnemyDistance < attackingDistance) appliedMovement = movement * -1;
        return _rigidBody.position + appliedMovement * movementSpeed * Time.fixedDeltaTime;
    }
}
