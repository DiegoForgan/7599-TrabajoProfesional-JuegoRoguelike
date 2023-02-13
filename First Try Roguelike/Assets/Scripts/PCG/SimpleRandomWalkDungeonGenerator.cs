using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//This algorithm generates dungeons based on paths created by random "walks" through the tilemap cells
public class SimpleRandomWalkDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField] protected RandomWalkData randomWalkParameters;
    [SerializeField] protected const int CORRIDOR_WIDTH = 3;

    //private void Start() {
    //    RunProceduralGeneration();
    //}

    // PCG using the random walk algorithm which often results in a single dungeon room modified in size by the parameters defined on the editor
    protected override void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPosition = RunRandomWalk(startPosition);
        HashSet<Vector2Int> wallsPositions = WallGenerator.GenerateWalls(floorPosition);
        tilemapVisualizer.PaintFloortiles(floorPosition);
        tilemapVisualizer.PaintWallstiles(wallsPositions);
    }

    //  This method performs a number of random "walks" defined by the "iterations" parameter and returns the final path.
    protected HashSet<Vector2Int> RunRandomWalk(Vector2Int position)
    {
       Vector2Int currentPosition = position;
       HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
       for (int i = 0; i < randomWalkParameters.iterations; i++)
       {
           HashSet<Vector2Int> path = GenerateRandomWalkPath(currentPosition, randomWalkParameters.walkLength);
           floorPositions.UnionWith(path);
           if(randomWalkParameters.startRandomlyEachIteration) currentPosition = floorPositions.ElementAt(Random.Range(0,floorPositions.Count));  
       }
       return floorPositions; 
    }

    // This method makes a random "walk" starting at "startPosition" and a "walkLength" amount of steps.
    // It returns the path that was generated
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

    protected void CreateVerticalCorridor(ICollection<Vector2Int> corridor, ref Vector2Int currentPosition)
    {
        for (int i = 1; i <= CORRIDOR_WIDTH; i++)
        {
            corridor.Add(new Vector2Int(currentPosition.x + i, currentPosition.y));
        }
    }

    protected void CreateHorizontalCorridor(ICollection<Vector2Int> corridor, ref Vector2Int currentPosition)
    {
        for (int i = 1; i <= CORRIDOR_WIDTH; i++)
        {
            corridor.Add(new Vector2Int(currentPosition.x, currentPosition.y - i));
        }
    }
}
