using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawner : MonoBehaviour
{
    // This sets the amount of items to spawn PER DUNGEON
    // It means that every time a new dungeon is loaded, the amount defined by this variables
    // will be instantiated for every type of item.
    [SerializeField] private int healthItemAmount = 5;
    [SerializeField] private int manaItemAmount = 5;
    [SerializeField] private int goldItemAmount = 10;
    
    //[SerializeField] private int keyItemAmount = 1;
    
    //Prefab Variables
    [SerializeField] private GameObject healthItemPrefab;
    [SerializeField] private GameObject manaItemPrefab;
    [SerializeField] private GameObject goldItemPrefab;
    [SerializeField] private GameObject keyItemPrefab;
    
    public void Spawn(Dungeon currentDungeon)
    {
        SpawnHealthItems(currentDungeon);
        SpawnManaItems(currentDungeon);
        SpawnGoldItems(currentDungeon);
        SpawnKeyItem(currentDungeon.GetRandomFloorPosition());
    }

    private void SpawnGoldItems(Dungeon currentDungeon)
    {
        for (int i = 0; i < goldItemAmount; i++)
        {
            Vector3Int spawnPosition = (Vector3Int) currentDungeon.GetRandomFloorPosition();
            GameObject gold = Instantiate(goldItemPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private void SpawnManaItems(Dungeon currentDungeon)
    {
        for (int i = 0; i < manaItemAmount; i++)
        {
            Vector3Int spawnPosition = (Vector3Int) currentDungeon.GetRandomFloorPosition();
            GameObject mana = Instantiate(manaItemPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private void SpawnHealthItems(Dungeon currentDungeon)
    {
        for (int i = 0; i < healthItemAmount; i++)
        {
            Vector3Int spawnPosition = (Vector3Int) currentDungeon.GetRandomFloorPosition();
            GameObject health = Instantiate(healthItemPrefab, spawnPosition, Quaternion.identity);
        }
    }

    // Spawn the only key that will be available per dungeon
    private void SpawnKeyItem(Vector2Int spawnPosition)
    {
        GameObject key = Instantiate(keyItemPrefab, (Vector3Int) spawnPosition, Quaternion.identity);
    }
}
