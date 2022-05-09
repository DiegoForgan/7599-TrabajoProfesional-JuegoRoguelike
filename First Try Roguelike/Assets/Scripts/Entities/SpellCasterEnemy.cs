using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCasterEnemy : Enemy
{
    //Called before the Start function
    private void Awake() {
        _enemySpriteRenderer = GetComponent<SpriteRenderer>();
        _enemyMovement = GetComponent<EnemyMovement>(); 
        _attackPoint = transform.Find("ShootPoint");    
    }
    
    // Start is called before the first frame update
    void Start()
    {
        health = enemyData.health;
        maxHealth = health; 
        healthBar.initialize(enemyData.health);
        //Assigning a big value cause we count on it being replaced on any of the following "if" statements
        float distance = 100;
        //Creates a copy of the list to prevent changes on the Scriptable Object
        availableSpells = new List<SpellData>(enemyData.availableSpell);
        distance = enemyData.attackDistance;
        //Passing movement data to corresponding component
        _enemyMovement.SetMovementSpeed(enemyData.movementSpeed);
        _enemyMovement.SetAttackingParameters(enemyData.attackRate,distance);
        slowedDown = false;
    }

    public override void Attack(){
        CastRandomSpell();
    }
}
