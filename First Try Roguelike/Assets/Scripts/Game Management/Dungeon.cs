using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Dungeon : MonoBehaviour
{
    [SerializeField] private Tilemap floor;
    [SerializeField] private Tilemap walls;

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

    public string GetDungeonTilemapSize()
    {
        return "(" + walls.cellBounds.size.x + "," + walls.cellBounds.size.y + ")";
    }
    
    // Returns level size and wall tilemap structure
    // in string format, ready for output to file
    public string ExportDungeon()
    {
        StringBuilder stringBuilder = new StringBuilder();
        List<string> linesList = new List<string>();

        int i = 0;
        foreach (var pos in walls.cellBounds.allPositionsWithin)
        {   
            Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
            Vector3 place = walls.CellToWorld(localPlace);

            // We add a new line to the list when we hit the boundary of the tilemap 
            if ((i % walls.cellBounds.size.x) == 0)
            {
                linesList.Add(stringBuilder.ToString());
                stringBuilder.Clear();
            }
            else
            {
                // If we are within limits, we print a wall or a space
                if (walls.HasTile(localPlace))
                {
                    // Wall
                    stringBuilder.Append("\u2588");
                }
                else
                {
                    // Floor
                    stringBuilder.Append("\u0020");
                }                
            }
            i++;
        }
        linesList.Add(stringBuilder.ToString());

        // We add the size of the level
        linesList.Add("");
        linesList.Add("Level size: (" + walls.cellBounds.size.x + "," + walls.cellBounds.size.y + ")");

        // We need to reverse this list as walls.cellBounds.allPositionsWithin returns the
        // tiles in reverse order!
        linesList.Reverse();

        // We return a ingle string with the entire level
        return string.Join("\n", linesList);
    }
}
