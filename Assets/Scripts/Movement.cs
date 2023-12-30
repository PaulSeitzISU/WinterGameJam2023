using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Movement : MonoBehaviour
{
    public Tilemap tilemap; // Reference to your Tilemap component
    public GridManager gridManager; // Reference to the GridManager
    private Vector2Int currentGridPosition;
    public bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>(); // Get the Tilemap component

        if (tilemap == null)
        {
            Debug.LogError("No Tilemap found in the scene.");
            return;
        }

        gridManager = GameObject.Find("Tilemap").GetComponent<GridManager>(); // Find the GridManager in the scene

        if (gridManager == null)
        {
            Debug.LogError("No GridManager found in the scene.");
            return;
        }

        // Register the object in the GridManager at the current position
        currentGridPosition = GetGridTilePosition();
        gridManager.PlaceObject(gameObject, currentGridPosition);
    }

    // Update is called once per frame
    void Update()
    {
        if (tilemap == null)
        {
            tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>(); // Get the Tilemap component
            Debug.LogError("No Tilemap found in the scene. trying to find it again");
            return;
        }

        Vector2Int newGridPosition = GetGridTilePosition();

        if (newGridPosition != currentGridPosition)
        {
            // Remove the object from the old grid position and place it in the new position
            gridManager.RemoveObject(currentGridPosition);
            gridManager.PlaceObject(gameObject, newGridPosition);

            currentGridPosition = newGridPosition;
        }
    }

    public Vector2Int GetGridTilePosition()
    {
        // Get the position in world space and convert it to the position on the tilemap
        Vector3Int cellPosition = tilemap.WorldToCell(transform.position);

        return new Vector2Int(cellPosition.x, cellPosition.y);
    }

    public void MoveToGrid(Vector3Int targetGridPosition)
    {
        //Debug.Log("Moving to grid position: " + targetGridPosition);
        if (isMoving)
        {
            Debug.LogWarning("Already moving.");
            return;
        }
        List<Vector3Int> path = CalculatePath(GetGridTilePosition(), new Vector2Int(targetGridPosition.x, targetGridPosition.y));

        if (path != null && path.Count > 0)
        {
            StartCoroutine(FollowPath(path));
        }
    }

    List<Vector3Int> CalculatePath(Vector2Int start, Vector2Int end)
    {
        List<Vector3Int> path = new List<Vector3Int>();

        Vector2Int direction = end - start;

        int x = start.x;
        int y = start.y;

        int xStep = (int)Mathf.Sign(direction.x);
        int yStep = (int)Mathf.Sign(direction.y);

        int steps = Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y));

        for (int i = 0; i <= steps; i++)
        {
            path.Add(new Vector3Int(x, y, 0));

            if (i < Mathf.Abs(direction.x))
                x += xStep;

            if (i < Mathf.Abs(direction.y))
                y += yStep;
        }

        return path;
    }

    IEnumerator FollowPath(List<Vector3Int> path)
    {
        isMoving = true;

        foreach (Vector3Int gridPos in path)
        {
            Vector3 targetWorldPosition = tilemap.GetCellCenterWorld(gridPos);

            // Check if the target tile is occupied
            if (gridManager.IsGridPositionOccupied(new Vector2Int(gridPos.x, gridPos.y)) && currentGridPosition != new Vector2Int(gridPos.x, gridPos.y))
            {
                Debug.LogWarning("Target tile is occupied. Stopping movement.");
                isMoving = false; // Stop movement
                yield break; // Exit the coroutine
            }

            while (transform.position != targetWorldPosition)
            {
                Vector3 currentWorldPosition = transform.position;

                // Move towards the target world position (center of the tile)
                transform.position = Vector3.MoveTowards(currentWorldPosition, targetWorldPosition, Time.deltaTime * 5f);

                yield return null;
            }
        }

        isMoving = false;
    }

}
