using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    public GridManager gridManager; // Reference to the GridManager
    private List<Vector2Int> occupiedTiles = new List<Vector2Int>(); // List to store occupied tiles

    private void Start()
    {
        //gridManager = GameObject.Find("Tilemap").GetComponent<GridManager>(); // Find the GridManager in the scene

        if (gridManager == null)
        {
            Debug.LogError("No GridManager found in the scene.");
            return;
        }

        // Get the occupied tiles and inform the GridManager
        FindOccupiedTiles();
        InformGridManager();
    }

    private void FindOccupiedTiles()
    {
        // Loop through the Collider2D attached to the GameObject
        foreach (Collider2D collider in GetComponents<Collider2D>())
        {
            // Get the bounds of the collider
            Bounds bounds = collider.bounds;

            // Calculate the occupied tiles using the bounds
            Vector3 bottomLeft = new Vector3(bounds.min.x, bounds.min.y, 0f);
            Vector3 topRight = new Vector3(bounds.max.x, bounds.max.y, 0f);

            // Convert the world space positions to grid positions
            Vector3Int bottomLeftCell = gridManager.tilemap.WorldToCell(bottomLeft);
            Vector3Int topRightCell = gridManager.tilemap.WorldToCell(topRight);

            // Loop through each grid position within the bounds and add them to the occupiedTiles list
            for (int x = bottomLeftCell.x; x <= topRightCell.x; x++)
            {
                for (int y = bottomLeftCell.y; y <= topRightCell.y; y++)
                {
                    occupiedTiles.Add(new Vector2Int(x, y));
                }
            }
        }
    }

    private void InformGridManager()
    {
        // Inform the GridManager about the occupied tiles
        foreach (Vector2Int tile in occupiedTiles)
        {
            //Debug.Log("Informing GridManager about occupied tile: " + tile);
            gridManager.PlaceObject(gameObject, tile);
        }
    }
}
