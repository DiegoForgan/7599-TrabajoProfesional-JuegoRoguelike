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

    public override void Spawn(int difficultyLevel, Dungeon currentDungeon)
    {
        ogrePrefab.gameObject.tag = "enemy";
        goblinPrefab.gameObject.tag = "enemy";
        archerPrefab.gameObject.tag = "enemy";
        darkWitchPrefab.gameObject.tag = "enemy";
        spawnPrefabsOnDungeonByBoost(currentDungeon, difficultyLevel, ogrePrefab, ogresBoost);
        spawnPrefabsOnDungeonByBoost(currentDungeon, difficultyLevel, goblinPrefab, goblinsBoost);
        spawnPrefabsOnDungeonByBoost(currentDungeon, difficultyLevel, archerPrefab, archersBoost);
        spawnPrefabsOnDungeonByBoost(currentDungeon, difficultyLevel, darkWitchPrefab, darkWitchesBoost);
    }

    internal void SpawnFinalBoss(int difficultyLevel, Dungeon dungeon)
    {
        finalBossPrefab.gameObject.tag = "enemy";
        spawnPrefabsOnDungeonByAmount(dungeon, finalBossPrefab, FINAL_BOSS_AMOUNT);
    }

}
