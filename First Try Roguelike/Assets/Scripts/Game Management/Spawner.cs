using System.Collections.Generic;
using UnityEngine;

public abstract class Spawner : MonoBehaviour
{
    private List<GameObject> spawned = new List<GameObject>();
    
    internal int getRandomSpawnNumberBasedOnDifficulty(int difficultyLevel, int boost)
    {
        // Because random is exclusive on max but inclusive on min
        int maxValue = difficultyLevel + boost + 1;
        return Random.Range(difficultyLevel, maxValue);
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
        addGameObjectToList(Instantiate(prefab, spawnPosition, Quaternion.identity));
    } 

    public abstract void Spawn(int difficultyLevel, Dungeon currentDungeon);

    private void addGameObjectToList(GameObject newSpawn)
    {
        spawned.Add(newSpawn);
    }

    public void destroyAllSpawedObjects()
    {
        if (spawned == null) return;
        
        foreach (GameObject spawn in spawned)
        {
            Destroy(spawn);
        }
    }
}
