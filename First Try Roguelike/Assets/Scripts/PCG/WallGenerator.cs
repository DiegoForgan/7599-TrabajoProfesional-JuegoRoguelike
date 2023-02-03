using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator 
{
    public static HashSet<Vector2Int> GenerateWalls(HashSet<Vector2Int> floorPositions){
        HashSet<Vector2Int> wallPositions = FindWallsInDirections(floorPositions,Direction2D.cardinalDirectionsList);
        return wallPositions;
    }

    // Search every floor tile possible neighbour to check if its filled with floor or if it´s empty and a wall should be placed
    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floor, List<Vector2Int> directions){
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (Vector2Int position in floor)
        {
            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighbourPosition = position + direction;
                //if the neighbour cell is not a floor tile, it should be a wall
                if (!floor.Contains(neighbourPosition)) wallPositions.Add(neighbourPosition);
            }
        }
        return wallPositions;
    }
}
