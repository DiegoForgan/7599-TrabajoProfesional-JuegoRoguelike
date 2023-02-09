using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FinalBossState { MeleeAttacker, SpellCasterAttacker }

public class FinalBossEnemy : MeleeEnemy
{
    private FinalBossMovement _finalBossMovement;
    private FinalBossState currentBossState;
    private const float FINAL_BOSS_STATE_THRESHOLD = 0.5f;
    private float rangeDistance;

    private void Awake()
    {
        //_enemySpriteRenderer = GetComponent<SpriteRenderer>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        meleeWeapon = GetComponent<MeleeWeapon>();
        _finalBossMovement = GetComponent<FinalBossMovement>();
        _attackPoint = transform.Find("AttackPoint");
        animator = GetComponent<CharactersAnimator>();
        if (!playerTransform) Debug.LogError("No player Transform aquired!");
        if (!_attackPoint) Debug.LogError("No attack point found!");
        if (!_finalBossMovement) Debug.LogError("No movement component found!");
    }

    private void setBossState(FinalBossState newState)
    {
        Debug.Log("Changing Nilbud Attack State");
        currentBossState = newState;
    }

    public void Start()
    {
        setBossState(FinalBossState.SpellCasterAttacker);
        InitHealth();
        meleeWeapon.SetWeaponData(enemyData.availableMeleeWeapon);
        //Creates a copy of the list to prevent changes on the Scriptable Object
        availableSpells = new List<SpellData>(enemyData.availableSpell);
        //distance has to be tweaked in order to respond to the attacking point
        float distance = Vector2.Distance(transform.position, _attackPoint.position) + enemyData.availableMeleeWeapon.range;
        rangeDistance = enemyData.attackDistance;
        //Passing movement data to corresponding component
        InitAllMovementStats(distance, rangeDistance);
    }

    private void InitAllMovementStats(float distance, float rangeDistance)
    {
        //Passing movement data to corresponding component
        _finalBossMovement.SetMovementSpeed(enemyData.movementSpeed);
        _finalBossMovement.SetPlayerTransform(playerTransform);
        SetAttackingParameters(enemyData.attackRate, distance);
        _finalBossMovement.SetAttackingDistance(distance);
        _finalBossMovement.SetRangeDistance(rangeDistance);
    }

    private bool IsAbleToAttack(FinalBossState state)
    {
        //Checks if player is at the distance required for enemy to attack
        float playerEnemyDistance = Vector2.Distance(transform.position, playerTransform.position);
        
        bool result = false;

        if (state == FinalBossState.MeleeAttacker) 
            result = _renderer.isVisible && (playerEnemyDistance <= attackDistance) && (Time.time >= nextAttackTime);
        else if (state == FinalBossState.SpellCasterAttacker)
            result = _renderer.isVisible && (playerEnemyDistance <= rangeDistance) && (Time.time >= nextAttackTime);
        
        return result;
    }

    private void ExecuteSpellCastingAttack()
    {
        Debug.Log("Spell casting from SPELL CASTER CLASS");
        animator.SetSpellCastingWithStaffAnimation();
        CastRandomSpell();
    }

    private void ExecuteMeleeAttack()
    {
        Debug.Log("Attacking from Melee atacker class");
        meleeWeapon.AttackPlayer(_attackPoint);
        animator.setAttackAnimation();
    }

    private float GetEnemyRemainingHealthPercentage()
    {
        return (float)GetHealth() / GetMaxHealth();
    }

    private void SetBossStateBasedOnRemainingHealthPercentage()
    {
        Debug.Log("Nilbud Remaining Health Percentage: " + GetEnemyRemainingHealthPercentage() * 100);
        currentBossState = (GetEnemyRemainingHealthPercentage() < FINAL_BOSS_STATE_THRESHOLD) ? FinalBossState.MeleeAttacker : FinalBossState.SpellCasterAttacker;
        _finalBossMovement.UpdateBossState(currentBossState);
    }

    private void Update()
    {
        SetBossStateBasedOnRemainingHealthPercentage();

        if (playerTransform.Equals(null)) return;
        if (!IsAbleToAttack(currentBossState)) return;

        if (currentBossState == FinalBossState.MeleeAttacker) ExecuteMeleeAttack();
        if (currentBossState == FinalBossState.SpellCasterAttacker) ExecuteSpellCastingAttack();
        
        nextAttackTime = Time.time + 1f / attackRate;
    }   
} 


