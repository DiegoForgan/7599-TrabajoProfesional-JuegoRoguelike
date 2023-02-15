using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : Spawner
{
    [SerializeField] private GameObject ogrePrefab;
    [SerializeField] private GameObject goblinPrefab;
    [SerializeField] private GameObject archerPrefab;
    [SerializeField] private GameObject darkWitchPrefab;
    [SerializeField] private GameObject finalBossPrefab;
    
    [SerializeField] private int ogresBoost = 1;
    [SerializeField] private int goblinsBoost = 2;
    [SerializeField] private int archersBoost = 3;
    [SerializeField] private int darkWitchesBoost = 4;

    private const int FINAL_BOSS_AMOUNT = 1;
    private const string ENEMY_TAG = "enemy";

    public override void Spawn(int difficultyLevel, Dungeon currentDungeon)
    {
        ogrePrefab.gameObject.tag = ENEMY_TAG;
        goblinPrefab.gameObject.tag = ENEMY_TAG;
        archerPrefab.gameObject.tag = ENEMY_TAG;
        darkWitchPrefab.gameObject.tag = ENEMY_TAG;
        spawnPrefabsOnDungeonByBoost(currentDungeon, difficultyLevel, ogrePrefab, ogresBoost);
        spawnPrefabsOnDungeonByBoost(currentDungeon, difficultyLevel, goblinPrefab, goblinsBoost);
        spawnPrefabsOnDungeonByBoost(currentDungeon, difficultyLevel, archerPrefab, archersBoost);
        spawnPrefabsOnDungeonByBoost(currentDungeon, difficultyLevel, darkWitchPrefab, darkWitchesBoost);
    }

    internal void SpawnFinalBoss(int difficultyLevel, Dungeon dungeon)
    {
        finalBossPrefab.gameObject.tag = ENEMY_TAG;
        spawnPrefabsOnDungeonByAmount(dungeon, finalBossPrefab, FINAL_BOSS_AMOUNT);
    }

}
