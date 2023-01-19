using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject ogrePrefab;
    [SerializeField] private GameObject goblinPrefab;
    [SerializeField] private GameObject archerPrefab;
    [SerializeField] private GameObject darkWitchPrefab;
    [SerializeField] private int ogresToSpawn = 1;
    [SerializeField] private int goblinsToSpawn = 2;
    [SerializeField] private int archersToSpawn = 3;
    [SerializeField] private int darwitchesToSpawn = 4;

    internal void Spawn(int difficultyLevel, Dungeon currentDungeon)
    {
        SpawnOgres(currentDungeon,difficultyLevel);
        SpawnGoblins(currentDungeon,difficultyLevel);
        SpawnArchers(currentDungeon,difficultyLevel);
        SpawnDarkWitches(currentDungeon,difficultyLevel);
    }

    private void SpawnDarkWitches(Dungeon dungeon, int difficultyLevel)
    {
        //Random(difficultylevel,difficultyLevel+parametroFijo) 1,2(1) //5,6(5) //10,11(10)
        for (int i = 0; i < darwitchesToSpawn; i++)
        {
            Vector3Int spawnPosition = (Vector3Int)dungeon.GetRandomFloorPosition();
            Instantiate(darkWitchPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private void SpawnArchers(Dungeon dungeon, int difficultyLevel)
    {
        for (int i = 0; i < archersToSpawn; i++)
        {
            Vector3Int spawnPosition = (Vector3Int)dungeon.GetRandomFloorPosition();
            Instantiate(archerPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private void SpawnGoblins(Dungeon dungeon, int difficultyLevel)
    {
        for (int i = 0; i < goblinsToSpawn; i++)
        {
            Vector3Int spawnPosition = (Vector3Int)dungeon.GetRandomFloorPosition();
            Instantiate(goblinPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private void SpawnOgres(Dungeon dungeon, int difficulty)
    {
        for (int i = 0; i < ogresToSpawn; i++)
        {
            Vector3Int spawnPosition = (Vector3Int)dungeon.GetRandomFloorPosition();
            Instantiate(ogrePrefab, spawnPosition, Quaternion.identity);
        }
    }
}
