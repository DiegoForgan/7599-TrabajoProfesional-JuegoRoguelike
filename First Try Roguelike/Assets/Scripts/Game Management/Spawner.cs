using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Spawner : MonoBehaviour
{
    internal int getRandomSpawnNumberBasedOnDifficulty(int difficultyLevel, int boost)
    {
        return Random.Range(difficultyLevel, difficultyLevel + boost);
    }

    internal void spawnPrefabsOnDungeonByBoost(Dungeon dungeon, int difficultyLevel, GameObject prefab, int boostValue)
    {
        int amountToSpawnBasedOnDifficulty = getRandomSpawnNumberBasedOnDifficulty(difficultyLevel, boostValue);
        for (int i = 0; i < amountToSpawnBasedOnDifficulty; i++)
        {
            spawnPrefabOnRandomPosition(dungeon, prefab);
        }
    }

    internal void spawnPrefabsOnDungeonByAmount(Dungeon dungeon, GameObject prefab, int amountToSpawn)
    {
        for (int i = 0; i < amountToSpawn; i++)
        {
            spawnPrefabOnRandomPosition(dungeon, prefab);
        }
    }

    private void spawnPrefabOnRandomPosition(Dungeon dungeon, GameObject prefab)
    {
        Vector3Int spawnPosition = (Vector3Int)dungeon.GetRandomFloorPosition();
        Instantiate(prefab, spawnPosition, Quaternion.identity);
    } 

    public abstract void Spawn(int difficultyLevel, Dungeon currentDungeon);
}
