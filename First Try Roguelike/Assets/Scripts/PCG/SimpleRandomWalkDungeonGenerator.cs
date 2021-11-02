using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//This algorithm generates dungeons based on paths created by random "walks" through the tilemap cells
public class SimpleRandomWalkDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField] private RandomWalkData randomWalkParameters;
    
    private void Awake() {
        tilemapVisualizer = GetComponent<TilemapVisualizer>();
    }
    
    private void Start() {
        RunProceduralGeneration();
    }
    
    protected override void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPosition = RunRandomWalk();
        tilemapVisualizer.PaintFloortiles(floorPosition);
    }

    protected HashSet<Vector2Int> RunRandomWalk()
    {
       Vector2Int currentPosition = startPosition;
       HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
       for (int i = 0; i < randomWalkParameters.iterations; i++)
       {
           HashSet<Vector2Int> path = GenerateRandomWalkPath(currentPosition, randomWalkParameters.walkLength);
           floorPositions.UnionWith(path);
           if(randomWalkParameters.startRandomlyEachIteration) currentPosition = floorPositions.ElementAt(Random.Range(0,floorPositions.Count));  
       }
       return floorPositions; 
    }

    private HashSet<Vector2Int> GenerateRandomWalkPath(Vector2Int startPosition, int walkLength){
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();
        
        path.Add(startPosition);
        Vector2Int previousPosition = startPosition;

        for (int i = 0; i < walkLength; i++)
        {
            Vector2Int newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();
            path.Add(newPosition);
            previousPosition = newPosition;
        }

        return path;
    }
}
