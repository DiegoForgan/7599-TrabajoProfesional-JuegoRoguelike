using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : Spawner
{
    [SerializeField] private GameObject ogrePrefab;
    [SerializeField] private GameObject goblinPrefab;
    [SerializeField] private GameObject archerPrefab;
    [SerializeField] private GameObject darkWitchPrefab;
    
    [SerializeField] private int ogresBoost = 1;
    [SerializeField] private int goblinsBoost = 2;
    [SerializeField] private int archersBoost = 3;
    [SerializeField] private int darkWitchesBoost = 4;

    public override void Spawn(int difficultyLevel, Dungeon currentDungeon)
    {
        spawnPrefabsOnDungeonByBoost(currentDungeon, difficultyLevel, ogrePrefab, ogresBoost);
        spawnPrefabsOnDungeonByBoost(currentDungeon, difficultyLevel, goblinPrefab, goblinsBoost);
        spawnPrefabsOnDungeonByBoost(currentDungeon, difficultyLevel, archerPrefab, archersBoost);
        spawnPrefabsOnDungeonByBoost(currentDungeon, difficultyLevel, darkWitchPrefab, darkWitchesBoost);
    }
}
