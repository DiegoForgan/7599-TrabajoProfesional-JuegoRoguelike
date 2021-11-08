using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Dungeon : MonoBehaviour
{
    [SerializeField] private Tilemap floor;
    [SerializeField] private Tilemap walls;
    public static Dungeon instance;
    private void Awake() {
        if (instance == null) instance = this;
        else {
          Destroy(gameObject);
          return;
        }
        DontDestroyOnLoad(gameObject);
    }
    
    public Vector2Int GetRandomFloorPosition(){
        Vector2Int candidatePosition = new Vector2Int(0,0);
        Vector3Int tilePosition;
        do
        {
           candidatePosition.x = Random.Range(0,500);
           candidatePosition.y = Random.Range(0,500);
           tilePosition = floor.WorldToCell((Vector3Int)candidatePosition);
        } while (!floor.HasTile(tilePosition));
        
        return candidatePosition;
    }
    
}
