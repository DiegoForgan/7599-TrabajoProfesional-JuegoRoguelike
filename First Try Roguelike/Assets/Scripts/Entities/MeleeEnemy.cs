using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        InitHealth();
        meleeWeapon.SetWeaponData(enemyData.availableMeleeWeapon);
        //distance has to be tweaked in order to respond to the attacking point
        float distance = Vector2.Distance(transform.position,_attackPoint.position) + enemyData.availableMeleeWeapon.range;
        //Passing movement data to corresponding component
        InitMovementStats(distance);
    }
   
    public override void Attack(){
        Debug.Log("Attacking from Melee atacker class");
        meleeWeapon.AttackPlayer(_attackPoint);
        animator.setAttackAnimation();
    }

    public override bool IsMeleeAttacker() { return true; }
}
