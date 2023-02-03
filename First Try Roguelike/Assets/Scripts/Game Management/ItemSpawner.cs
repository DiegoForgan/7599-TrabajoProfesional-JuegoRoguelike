using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : Spawner
{
    // This sets the amount of items to spawn PER DUNGEON
    // It means that every time a new dungeon is loaded, the amount defined by this variables
    // will be instantiated for every type of item.
    [SerializeField] private int healthItemBoost = 5;
    [SerializeField] private int manaItemBoost = 5;
    [SerializeField] private const int GOLD_AMOUNT = 25;
    [SerializeField] private const int KEY_AMOUNT = 1;
    
    //Prefab Variables
    [SerializeField] private GameObject healthItemPrefab;
    [SerializeField] private GameObject manaItemPrefab;
    [SerializeField] private GameObject goldItemPrefab;
    [SerializeField] private GameObject keyItemPrefab;
  
    public override void Spawn(int difficultyLevel, Dungeon currentDungeon)
    {   
        spawnPrefabsOnDungeonByBoost(currentDungeon, difficultyLevel, healthItemPrefab, healthItemBoost);
        spawnPrefabsOnDungeonByBoost(currentDungeon, difficultyLevel, manaItemPrefab, manaItemBoost);
        spawnPrefabsOnDungeonByAmount(currentDungeon, goldItemPrefab, GOLD_AMOUNT);
        spawnPrefabsOnDungeonByAmount(currentDungeon, keyItemPrefab, KEY_AMOUNT);
    }
}
