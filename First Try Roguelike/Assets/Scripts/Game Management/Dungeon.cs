using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Dungeon : MonoBehaviour
{
    [SerializeField] private Tilemap floor;
    [SerializeField] private Tilemap walls;
    
    /*private void Start() {
        GameManager.Instance.CreateNewDungeon();
    }*/

    //This method is supposed to put the player on a valid FLOOR tile.
    //It seems to work fine but maybe it needs some tunning
    public Vector2Int GetRandomFloorPosition(){
        Vector2Int candidatePosition = Vector2Int.zero;
        Vector3Int tilePosition;
        do
        {
           //TODO: Be careful with this numbers, may break the game
           candidatePosition.x = Random.Range(-1000, 1000);
           candidatePosition.y = Random.Range(-1000, 1000);
           tilePosition = floor.WorldToCell((Vector3Int)candidatePosition);
        } while (!floor.HasTile(tilePosition));
        
        return candidatePosition;
    }

    public Tilemap GetFloorTilemap()
    {
        return floor;
    }

    public Tilemap GetWallsTilemap()
    {
        return walls;
    }
}
