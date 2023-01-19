using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject ogrePrefab;
    [SerializeField] private GameObject goblinPrefab;
    [SerializeField] private GameObject archerPrefab;
    [SerializeField] private GameObject darkWitchPrefab;
    
    [SerializeField] private int ogresBoost = 1;
    [SerializeField] private int goblinsBoost = 2;
    [SerializeField] private int archersBoost = 3;
    [SerializeField] private int darkWitchesBoost = 4;

    private int getRandomSpawnNumberBasedOnDifficulty(int difficultyLevel, int constantNumber)
    {
        return Random.Range(difficultyLevel, difficultyLevel + constantNumber);
    }

    private void SpawnEnemy(Dungeon dungeon, int difficultyLevel, GameObject enemyPrefab, int baseValue)
    {
        int amountToSpawnBasedOnDifficulty = getRandomSpawnNumberBasedOnDifficulty(difficultyLevel, baseValue);
        
        for (int i = 0; i < amountToSpawnBasedOnDifficulty; i++)
        {
            Vector3Int spawnPosition = (Vector3Int)dungeon.GetRandomFloorPosition();
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    internal void Spawn(int difficultyLevel, Dungeon currentDungeon)
    {
        SpawnEnemy(currentDungeon, difficultyLevel, ogrePrefab, ogresBoost);
        SpawnEnemy(currentDungeon, difficultyLevel, goblinPrefab, goblinsBoost);
        SpawnEnemy(currentDungeon, difficultyLevel, archerPrefab, archersBoost);
        SpawnEnemy(currentDungeon, difficultyLevel, darkWitchPrefab, darkWitchesBoost);
    }
}
