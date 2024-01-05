using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapScanner : MonoBehaviour
{
    public Tilemap tilemap; // Reference to the Tilemap component
    public GridManager gridManager; // Reference to the GridManager
    public List<TileBase> propTiles; // List of wall tiles to check

    private void Start()
    {
        if (tilemap == null)
        {
            Debug.LogError("No Tilemap found. Assign a Tilemap in the inspector.");
            return;
        }

        if (gridManager == null)
        {
            Debug.LogError("No GridManager found. Assign a GridManager in the inspector.");
            return;
        }
        //Debug.Log("Tilemap scanning?");

        //ScanTilemap();
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         ScanTilemap();
    //     }
    // }



    public void ScanTilemap()
    {
        BoundsInt bounds = tilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            Vector3Int localPlace = (new Vector3Int(pos.x, pos.y, (int)tilemap.transform.position.y));
            Vector3 place = tilemap.CellToWorld(localPlace);

            if (tilemap.HasTile(localPlace)) // Check if there is a tile at the given position
            {
                TileBase tile = tilemap.GetTile(localPlace);

                // Check if the tile exists in the wallTiles list
                if (propTiles.Contains(tile))
                {
                    Vector3Int gridPosition = new Vector3Int(localPlace.x, localPlace.y, 0);
                    gridManager.PlaceObject(gameObject, new Vector2Int(gridPosition.x, gridPosition.y));
                }
            }
        }
        //Debug.Log("Tilemap scanned");
    }
}
