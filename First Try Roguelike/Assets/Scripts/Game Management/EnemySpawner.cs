using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : Spawner
{
    [SerializeField] private GameObject ogrePrefab;
    [SerializeField] private GameObject goblinPrefab;
    [SerializeField] private GameObject archerPrefab;
    [SerializeField] private GameObject darkWitchPrefab;
    [SerializeField] private GameObject midLevelBossPrefab;
    [SerializeField] private GameObject finalBossPrefab;
    
    [SerializeField] private int ogresMinAmount = 1;
    [SerializeField] private int darkWitchesMinAmount = 1;
    [SerializeField] private int goblinsMinAmount = 2;
    [SerializeField] private int archersMinAmount = 2;
    
    private const int FINAL_BOSS_AMOUNT = 1;
    private const int MIDLEVEL_BOSS_AMOUNT = 1;
    private const string ENEMY_TAG = "enemy";

    public override void Spawn(int difficultyLevel, Dungeon currentDungeon)
    {
        ogrePrefab.gameObject.tag = ENEMY_TAG;
        goblinPrefab.gameObject.tag = ENEMY_TAG;
        archerPrefab.gameObject.tag = ENEMY_TAG;
        darkWitchPrefab.gameObject.tag = ENEMY_TAG;
        // boost parameter gets modified by "minAmount" parameter but method signature remains the same for compatibility issues
        spawnPrefabsOnDungeonByBoost(currentDungeon, difficultyLevel, ogrePrefab, ogresMinAmount);
        spawnPrefabsOnDungeonByBoost(currentDungeon, difficultyLevel, goblinPrefab, goblinsMinAmount);
        spawnPrefabsOnDungeonByBoost(currentDungeon, difficultyLevel, archerPrefab, archersMinAmount);
        spawnPrefabsOnDungeonByBoost(currentDungeon, difficultyLevel, darkWitchPrefab, darkWitchesMinAmount);
    }

    internal void SpawnFinalBoss(int difficultyLevel, Dungeon dungeon)
    {
        finalBossPrefab.gameObject.tag = ENEMY_TAG;
        spawnFinalBossOnDungeonByAmount(dungeon, finalBossPrefab, FINAL_BOSS_AMOUNT);
    }

    internal void SpawnMidLevelBoss(int difficultyLevel, Dungeon dungeon)
    {
        midLevelBossPrefab.gameObject.tag = ENEMY_TAG;
        spawnMidLevelBossOnDungeonByAmount(dungeon, midLevelBossPrefab, MIDLEVEL_BOSS_AMOUNT);
    }

}
