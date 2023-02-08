using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossEnemy : MonoBehaviour
{
    [SerializeField] private MeleeEnemy meleeEnemy;
    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private SpellCasterEnemy spellCasterEnemy;
    [SerializeField] private EnemyLongRangeMovement enemyLongRangeMovement;
    [SerializeField] private Enemy enemy;

    private enum FinalBossState { MeleeAttacker, SpellCasterAttacker}

    private FinalBossState currentBossState;
    private const float FINAL_BOSS_STATE_THRESHOLD = 0.5f;

    private void Start()
    {
        currentBossState = FinalBossState.SpellCasterAttacker;
    }

    private void Update()
    {
        SetScriptsBasedOnBossState();
        UpdateBossState();
    }

    private void UpdateBossState()
    {
        Debug.Log("Nilbud Remaining Health Percentage: " + GetEnemyRemainingHealthPercentage() * 100);
        if (GetEnemyRemainingHealthPercentage() < FINAL_BOSS_STATE_THRESHOLD) setBossState(FinalBossState.MeleeAttacker);
    }

    private float GetEnemyRemainingHealthPercentage()
    {
        return enemy.GetHealth() / enemy.GetMaxHealth();
    }

    private void SetScriptsBasedOnBossState()
    {
        if (currentBossState == FinalBossState.MeleeAttacker) EnableMeleeScripts();
        else if (currentBossState == FinalBossState.SpellCasterAttacker) EnableSpellCastingScripts();
    }

    private void setBossState(FinalBossState newState)
    {
        Debug.Log("Changing Nilbud Attack State");
        currentBossState = newState;
    }

    private void EnableSpellCastingScripts()
    {
        meleeEnemy.enabled = false;
        enemyMovement.enabled = false;
        spellCasterEnemy.enabled = true;
        enemyLongRangeMovement.enabled = true;
    }

    private void EnableMeleeScripts()
    {
        meleeEnemy.enabled = true;
        enemyMovement.enabled = true;
        spellCasterEnemy.enabled = false;
        enemyLongRangeMovement.enabled = false;
    }
}

