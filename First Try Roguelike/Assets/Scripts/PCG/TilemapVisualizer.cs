using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallsTilemap;
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;

    public void PaintFloortiles(IEnumerable<Vector2Int> floorPositions){
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }
    public void PaintWallstiles(IEnumerable<Vector2Int> wallsPositions){
        PaintTiles(wallsPositions, wallsTilemap, wallTile);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile){
        foreach (Vector2Int position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position){
        Vector3Int tilePosition = tilemap.WorldToCell((Vector3Int) position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear(){
        floorTilemap.ClearAllTiles();
        wallsTilemap.ClearAllTiles();
    }

    public void SetTilemaps(Tilemap newFloorTilemap, Tilemap newWallTilemap)
    {
        floorTilemap = newFloorTilemap;
        wallsTilemap = newWallTilemap;
    }
}
