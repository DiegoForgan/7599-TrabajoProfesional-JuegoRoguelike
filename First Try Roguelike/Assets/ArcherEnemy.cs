using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherEnemy : Enemy
{
    //Called before the Start function
    private void Awake()
    {
        //_enemySpriteRenderer = GetComponent<SpriteRenderer>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _enemyMovement = GetComponent<EnemyMovement>();
        _attackPoint = transform.Find("AttackPoint");
        animator = GetComponent<CharactersAnimator>();
        if (!playerTransform) Debug.LogError("No player Transform aquired!");
        if (!_attackPoint) Debug.LogError("No attack point found!");
    }

    // Start is called before the first frame update
    void Start()
    {
        //Initialites health stats for the current Enemy and it´s healthbar ALSO slowed Down Status.
        InitHealth();
        //Setting data on the movement component
        InitMovementStats(enemyData.attackDistance);
    }

    public override void Attack()
    {
        Debug.Log("Throwing arrow from archer enemy");
        animator.SetArrowThrowingAnimation();
        shootArrow();
    }

    private void shootArrow()
    {
        throw new NotImplementedException();
    }
}
