using System.Collections.Generic;
using UnityEngine;


public abstract class Spawner : MonoBehaviour
{
    private List<GameObject> spawned = new List<GameObject>();
    // this will make max random spawns for enemies the half of the current difficulty level
    // example: On difficulty "4", you will be able to spawn:
    // difficultyLevel * factor => 4*0.5 => 2 (up to 2 enemies of that type)
    private const float SPAWN_DIFFICULTY_FACTOR = 0.5f; //half the current difficulty level
    private const int MIN_SPAWN_AMOUNT = 1;
    
    protected int getRandomSpawnNumberBasedOnDifficulty(int difficultyLevel, int boost)
    {
        // Because random is exclusive on max but inclusive on min
        int maxValue = difficultyLevel + boost + 1;
        return Random.Range(difficultyLevel, maxValue);
    }

    protected void spawnPrefabsOnDungeonByBoost(Dungeon dungeon, int difficultyLevel, GameObject prefab, int boostValue)
    {
        int amountToSpawnBasedOnDifficulty = prefab.CompareTag("enemy") ? getRandomSpawnNumberForEnemies(difficultyLevel, boostValue) : getRandomSpawnNumberBasedOnDifficulty(difficultyLevel, boostValue);
        for (int i = 0; i < amountToSpawnBasedOnDifficulty; i++)
        {
            spawnPrefabOnRandomPosition(dungeon, prefab);
        }
    }

    private int getRandomSpawnNumberForEnemies(int difficultyLevel, int minAmount)
    {
        float maxRandomAmount = Mathf.Floor(difficultyLevel * SPAWN_DIFFICULTY_FACTOR);
        float randomAmountToSpawn = (maxRandomAmount<= MIN_SPAWN_AMOUNT) ? MIN_SPAWN_AMOUNT : Random.Range(minAmount, maxRandomAmount);
        return Mathf.FloorToInt(randomAmountToSpawn);
    }

    protected void spawnPrefabsOnDungeonByAmount(Dungeon dungeon, GameObject prefab, int amountToSpawn)
    {
        for (int i = 0; i < amountToSpawn; i++)
        {
            spawnPrefabOnRandomPosition(dungeon, prefab);
        }
    }

    protected void spawnFinalBossOnDungeonByAmount(Dungeon dungeon, GameObject finalBossPrefab, int fINAL_BOSS_AMOUNT)
    {
        Vector3Int spawnPosition = dungeon.GetBossPosition();
        addGameObjectToList(Instantiate(finalBossPrefab, spawnPosition, Quaternion.identity));
    }

    protected void spawnMidLevelBossOnDungeonByAmount(Dungeon dungeon, GameObject midLevelBossPrefab, int mIDLEVEL_BOSS_AMOUNT)
    {
        Vector3Int spawnPosition = dungeon.GetBossPosition();
        addGameObjectToList(Instantiate(midLevelBossPrefab, spawnPosition, Quaternion.identity));
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
