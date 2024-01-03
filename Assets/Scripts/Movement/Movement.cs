using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Movement : MonoBehaviour
{
    public Tilemap tilemap; // Reference to your Tilemap component
    public GridManager gridManager; // Reference to the GridManager
    public AStar aStar; // Reference to the AStar script
    public Vector2Int currentGridPosition;
    public bool isMoving = false;

    void Awake()
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

        aStar = GameObject.Find("Tilemap").GetComponent<AStar>(); // Find the AStar script in the scene

        if (aStar == null)
        {
            Debug.LogError("No AStar script found in the scene.");
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {


        // Register the object in the GridManager at the current position
        currentGridPosition = GetGridTilePosition(transform.position);
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

        Vector2Int newGridPosition = GetGridTilePosition(transform.position);

        if (newGridPosition != currentGridPosition)
        {
            // Remove the object from the old grid position and place it in the new position
            gridManager.RemoveObject(currentGridPosition);
            gridManager.PlaceObject(gameObject, newGridPosition);

            currentGridPosition = newGridPosition;
        }
    }

    public Vector2Int GetGridTilePosition(Vector3 worldPosition)
    {
        // Get the position in world space and convert it to the position on the tilemap
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

        return new Vector2Int(cellPosition.x, cellPosition.y);
    }

    public bool MoveToGrid(Vector3Int targetGridPosition, int depth = -1)
    {
        //Debug.Log("Moving to grid position: " + targetGridPosition);
        if (isMoving)
        {
            Debug.LogWarning("Already moving.");
            return false;
        }
        List<Vector3Int> path = CalculatePath(GetGridTilePosition(transform.position), new Vector2Int(targetGridPosition.x, targetGridPosition.y));

        if (path == null && path.Count < 0)
        {
            return false;
        }

        if (path != null && depth == 0)
        {
            StartCoroutine(FollowPath(path,  path.Count -1));
            return true;
        }

        if (path != null && path.Count > 0)
        {
            StartCoroutine(FollowPath(path, depth));
            return true;
        }
        return false;
    }

    // List<Vector3Int> CalculatePath(Vector2Int start, Vector2Int end)
    // {
    //     List<Vector3Int> path = new List<Vector3Int>();

    //     Vector2Int direction = end - start;

    //     int x = start.x;
    //     int y = start.y;

    //     int xStep = (int)Mathf.Sign(direction.x);
    //     int yStep = (int)Mathf.Sign(direction.y);

    //     int steps = Mathf.Max(Mathf.Abs(direction.x), Mathf.Abs(direction.y));

    //     for (int i = 0; i <= steps; i++)
    //     {
    //         path.Add(new Vector3Int(x, y, 0));

    //         if (i < Mathf.Abs(direction.x))
    //             x += xStep;

    //         if (i < Mathf.Abs(direction.y))
    //             y += yStep;
    //     }

    //     return path;
    // }

    List<Vector3Int> CalculatePath(Vector2Int start, Vector2Int end)
    {
        //Debug.Log("start" + start.x + " " + start.y + "end" + end.x + " " + end.y );
        //Debug.Log(aStar.FindPath(aStar.grid[start.x, start.y], aStar.grid[end.x, end.y]));
        List<AStar.AStarNode> tempPath = aStar.FindPath(aStar.grid[start.x + (gridManager.gridSize/2), start.y + (gridManager.gridSize/2)], aStar.grid[end.x + (gridManager.gridSize/2), end.y + (gridManager.gridSize/2)]);
  
        List<Vector3Int> convertPath = new List<Vector3Int>();

        if (tempPath == null)
        {
            //Debug.Log("No path found.");
            return convertPath;
        }
        foreach (AStar.AStarNode node in tempPath)
        {
            convertPath.Add(new Vector3Int(node.x - ( gridManager.gridSize/2 ), node.y - ( gridManager.gridSize/2 ), 0));
            //Debug.Log(node.x - ( gridManager.gridSize/2 ) + " " + (node.y - ( gridManager.gridSize/2 )));
        }

        return convertPath;

    }

    IEnumerator FollowPath(List<Vector3Int> path, int depth = -1)
    {
        isMoving = true;

        foreach (Vector3Int gridPos in path)
        {
            if (depth == 0)
            {
                isMoving = false;
                yield break;
            }
            else if (depth > 0)
            {
                depth--;
            }
            
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
