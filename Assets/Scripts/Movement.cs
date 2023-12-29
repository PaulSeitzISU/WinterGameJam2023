using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Movement : MonoBehaviour
{
    public Tilemap tilemap; // Reference to your Tilemap component
    private Vector3Int currentGridPosition;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>(); // Get the Tilemap component

        if (tilemap == null)
        {
            Debug.LogError("No Tilemap found in the scene.");
            return;
        }

        // Call this function to get the grid tile when needed
        currentGridPosition = GetGridTilePosition();
        Debug.Log("Grid Position: " + currentGridPosition);
    }

    // Update is called once per frame
    void Update()
    {
        if(tilemap == null)
        {
            tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>(); // Get the Tilemap component
            Debug.LogError("No Tilemap found in the scene. trying to find it again");
            return;
        }
        currentGridPosition = GetGridTilePosition();  
    }

    public Vector3Int GetGridTilePosition()
    {
        Bounds bounds = GetComponent<Collider2D>().bounds;
        Vector3 center = bounds.center;

        // Get the position in world space and convert it to the position on the tilemap
        Vector3Int cellPosition = tilemap.WorldToCell(center);

        return cellPosition;
    }

    public void MoveToGrid(Vector3Int gridPosition)
    {
        Vector3 worldPosition = tilemap.GetCellCenterWorld(gridPosition);
        transform.position = worldPosition;
    }

}
