using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossEnemy : Enemy
{
    /*[SerializeField] private EnemyLongRangeMovement longRangeMovement;
    [SerializeField] private EnemyMovement enemyMovement;
    private EnemyMovement currentMovementType;
    
    private float meleeDistance;
    private bossPhase currentPhase;
    private enum bossPhase {SpellCaster, MeleeAtacker, Mix };

    private void Awake()
    {
        currentPhase = bossPhase.MeleeAtacker;
        longRangeMovement.enabled = false;
        
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        meleeWeapon = GetComponent<MeleeWeapon>();
        currentMovementType = enemyMovement;
        _attackPoint = transform.Find("AttackPoint");
        animator = GetComponent<CharactersAnimator>();

        if (!playerTransform) Debug.LogError("No player Transform aquired!");
        if (!_attackPoint) Debug.LogError("No attack point found!");
    }*/

    /*private void Start()
    {
        InitHealth();
        
        meleeWeapon.SetWeaponData(enemyData.availableMeleeWeapon);
        //distance has to be tweaked in order to respond to the attacking point
        meleeDistance = Vector2.Distance(transform.position, _attackPoint.position) + enemyData.availableMeleeWeapon.range;
        
        
        //Creates a copy of the list to prevent changes on the Scriptable Object
        availableSpells = new List<SpellData>(enemyData.availableSpell);
        distance = enemyData.attackDistance;
        
        InitMovementStats(distance);
    }*/
}

