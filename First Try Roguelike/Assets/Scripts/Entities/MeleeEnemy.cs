using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
      health = enemyData.health;
      maxHealth = health; 
      healthBar.initialize(enemyData.health);
      //Assigning a big value cause we count on it being replaced on any of the following "if" statements
      float distance = 100;
      meleeWeapon.SetWeaponData(enemyData.availableMeleeWeapon);
      //distance has to be tweaked in order to respond to the attacking point
      distance = Vector2.Distance(transform.position,_attackPoint.position);
      distance += enemyData.availableMeleeWeapon.range;
      //Passing movement data to corresponding component
      _enemyMovement.SetMovementSpeed(enemyData.movementSpeed);
      _enemyMovement.SetAttackingParameters(enemyData.attackRate,distance);
      slowedDown = false;  
    }

    public override void Attack(){
        meleeWeapon.AttackPlayer(_attackPoint);
    }
}
