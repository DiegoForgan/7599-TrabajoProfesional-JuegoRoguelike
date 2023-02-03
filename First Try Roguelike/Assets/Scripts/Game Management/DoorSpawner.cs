using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSpawner : Spawner
{
    [SerializeField] private GameObject doorPrefab;
    private const int DOOR_AMOUNT = 1;

    public override void Spawn(int difficultyLevel, Dungeon currentDungeon)
    {
        spawnPrefabsOnDungeonByAmount(currentDungeon, doorPrefab, DOOR_AMOUNT);
    }
}
