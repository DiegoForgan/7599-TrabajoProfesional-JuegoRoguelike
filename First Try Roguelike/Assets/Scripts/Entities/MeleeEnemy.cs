using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    private void Awake()
    {
        //_enemySpriteRenderer = GetComponent<SpriteRenderer>();
        playerTransform = GameObject.FindGameObjectWithTag(PLAYER_TAG).transform;
        meleeWeapon = GetComponent<MeleeWeapon>();
        _enemyMovement = GetComponent<EnemyMovement>();
        _attackPoint = transform.Find("AttackPoint");
        animator = GetComponent<CharactersAnimator>();
        if (!playerTransform) Debug.LogError("No player Transform aquired!");
        if (!_attackPoint) Debug.LogError("No attack point found!");
        if (!_enemyMovement) Debug.LogError("No movement component found!");
    }

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
