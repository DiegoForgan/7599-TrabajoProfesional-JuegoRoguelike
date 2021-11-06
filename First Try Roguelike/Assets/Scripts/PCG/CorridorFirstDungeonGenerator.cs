using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();
        //1. We create the corridors of the new dungeon according to the parameters set by the designer
        CreateCorridors(floorPositions, potentialRoomPositions);
        //2. With the information about the different corridors present on the map,
        //   we proceed to create rooms at random BUT POSSIBLE positions
        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);
        //3. With corridors and some random rooms generated, we get the dead ends that were not filled by a room.
        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);
        //4. Given the dead end candidates, rooms are created when necessary.
        CreateRoomsAtDeadEnds(deadEnds, roomPositions);
        //5. Information about corridors and rooms is merged
        floorPositions.UnionWith(roomPositions);
        //6. Draw floor tiles on screen to represent corridors and rooms
        tilemapVisualizer.PaintFloortiles(floorPositions);
        //7. Finally, walls are calculated and then drawn on screen to complete the dungeon
        HashSet<Vector2Int> wallsPositions = WallGenerator.GenerateWalls(floorPositions);
        tilemapVisualizer.PaintWallstiles(wallsPositions);
    }

    private void CreateRoomsAtDeadEnds(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach (Vector2Int position in deadEnds)
        {
            if(!roomFloors.Contains(position)){
                HashSet<Vector2Int> newRoom = RunRandomWalk(position);
                roomFloors.UnionWith(newRoom);
            }
        }
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomsToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);

        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomsToCreateCount).ToList();

        foreach (Vector2Int roomPosition in roomsToCreate)
        {
            HashSet<Vector2Int> roomFloor = RunRandomWalk(roomPosition);
            roomPositions.UnionWith(roomFloor);           
        }

        return roomPositions;
    }

    private void CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        Vector2Int currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);
        
        for (int i = 0; i < corridorCount; i++)
        {
            List<Vector2Int> corridor = GenerateDungeonRandomWalkCorridor(currentPosition, corridorLength);
            currentPosition = corridor[corridor.Count - 1];
            potentialRoomPositions.Add(currentPosition);
            floorPositions.UnionWith(corridor);
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions){
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        foreach (Vector2Int position in floorPositions)
        {
            int neighboursCount = 0;
            foreach (Vector2Int direction in Direction2D.cardinalDirectionsList)
            {
                if (floorPositions.Contains(position + direction)) neighboursCount++;
            }
            //Here the neighbour count was tweaked from 1 to 2 because corridors are wider now
            if (neighboursCount == 2) deadEnds.Add(position);
        }

        return deadEnds;
    }

    private List<Vector2Int> GenerateDungeonRandomWalkCorridor(Vector2Int startPosition, int corridorLength)
    {
        bool isHorizontal = false;

        List<Vector2Int> corridor = new List<Vector2Int>();
        Vector2Int randomDirection = Direction2D.GetRandomCardinalDirection();
        
        if(randomDirection.x != 0) isHorizontal = true;
         
        Vector2Int currentPosition = startPosition;
        corridor.Add(currentPosition);

        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += randomDirection;
            
            //This adds extra floor tiles to make corridors wider so the player can walk on them
            if(isHorizontal) corridor.Add(new Vector2Int(currentPosition.x, currentPosition.y - 1));
            else corridor.Add(new Vector2Int(currentPosition.x + 1, currentPosition.y));
            
            corridor.Add(currentPosition);    
        }
        return corridor;
    }
}
