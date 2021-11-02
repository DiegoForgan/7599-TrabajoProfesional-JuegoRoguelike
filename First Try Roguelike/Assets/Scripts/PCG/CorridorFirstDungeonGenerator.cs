using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField] private int corridorLength = 14;
    [SerializeField] private int corridorCount = 5;
    [SerializeField] [Range(0.1f,1f)] private float roomPercent = 0.8f;
    protected override void RunProceduralGeneration()
    {
        CorridorFirstGeneration();
    }

    private void CorridorFirstGeneration(){
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        CreateCorridors(floorPositions);
        tilemapVisualizer.PaintFloortiles(floorPositions);
        
        HashSet<Vector2Int> wallsPositions = WallGenerator.GenerateWalls(floorPositions);
        tilemapVisualizer.PaintWallstiles(wallsPositions);
    }

    private void CreateCorridors(HashSet<Vector2Int> floorPositions)
    {
        Vector2Int currentPosition = startPosition;

        for (int i = 0; i < corridorCount; i++)
        {
            List<Vector2Int> corridor = GenerateDungeonRandomWalkCorridor(currentPosition, corridorLength);
            currentPosition = corridor[corridor.Count - 1];
            floorPositions.UnionWith(corridor);
        }
    }

    private List<Vector2Int> GenerateDungeonRandomWalkCorridor(Vector2Int startPosition, int corridorLength)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        Vector2Int randomDirection = Direction2D.GetRandomCardinalDirection();
        
        Vector2Int currentPosition = startPosition;
        corridor.Add(currentPosition);

        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += randomDirection;
            corridor.Add(currentPosition);    
        }
        return corridor;
    }
}
