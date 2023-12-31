using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    public GameObject[,] gridObjects; // 2D array to store game objects on the grid
    public int gridSize = 100; // Define grid size along the X-axis

    public Tilemap tilemap; // Reference to the Tilemap component
    public TileBase emptyTile; // Tile to represent an empty grid cell
    public TileBase occupiedTile; // Tile to represent an occupied grid cell

    public bool debugMode;
    private void Awake()
    {
        // Initialize the grid with the specified size using arrays
        gridObjects = new GameObject[gridSize, gridSize];

        // No need to initialize the array elements to null as arrays in C# are initialized to default values (null for reference types)

        // tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>(); // Get the Tilemap component
    }

    void FixedUpdate()
    {
        if (debugMode)
        {
            UpdateGridVisuals();
        }
    }

    // Function to place an object at a specific grid position
    public void PlaceObject(GameObject obj, Vector2Int gridPosition)
    {
        int x = gridPosition.x + (gridSize / 2);
        int y = gridPosition.y + (gridSize / 2);

        //Debug.Log("placing object at " + x + " " + y);

        if (IsValidGridPosition(new Vector2Int(x, y)))
        {
            if (gridObjects[x, y] == null) // Check if the grid cell is empty
            {
                gridObjects[x, y] = obj;
            }
            else
            {
                Debug.Log("Grid cell is already occupied.");
            }
        }
        else
        {
            Debug.Log("Invalid grid position.");
        }
    }

    public void RemoveObject(Vector2Int gridPosition)
    {
        int x = gridPosition.x + (gridSize / 2);
        int y = gridPosition.y + (gridSize / 2);

        if (IsValidGridPosition(new Vector2Int(x, y)))
        {
            if (gridObjects[x, y] != null) // Check if the grid cell is occupied
            {
                gridObjects[x, y] = null;
            }
            else
            {
                Debug.LogWarning("Grid cell is already empty.");
            }
        }
        else
        {
            Debug.LogWarning("Invalid grid position.");
        }
    }

    // Function to check if a grid position is valid
    public bool IsValidGridPosition(Vector2Int gridPosition)
    {
        int x = gridPosition.x;
        int y = gridPosition.y;

        if (x < 0 || x >= gridSize || y < 0 || y >= gridSize)
        {
            Debug.LogWarning("Grid position is out of bounds.");
            return false;
        }
        return true;
    }
    // Function to check if a grid position is occupied by an object
    public bool IsGridPositionOccupied(Vector2Int gridPosition)
    {
        int x = gridPosition.x + (gridSize / 2);
        int y = gridPosition.y + (gridSize / 2);

            //Debug.Log("grid position" + x + " " + y);
            
            if( gridObjects[x, y] != null)
            {   
                //Debug.Log("gameobject in space" + gridObjects[x][y]);
                return true;
            } 
            else 
            {
                return false;
            }
         
            
    }

    // Function to get the object at a specific grid position
    public GameObject GetObjectAtGridPosition(Vector2Int gridPosition)
    {
        int x = gridPosition.x + (gridSize / 2);
        int y = gridPosition.y + (gridSize / 2);

 
            return gridObjects[x, y];
        

    }


    // Function to perform a radius search and return objects within the specified range of a grid position
    public List<GameObject> GetObjectsInRadius(Vector2Int gridPosition, int radius, GameObject ignoreObject = null)
    {
        List<GameObject> objectsInRadius = new List<GameObject>();

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                int checkX = gridPosition.x + x;
                int checkY = gridPosition.y + y;
                //Debug.Log("checking grid position" + checkX + " " + checkY );
            
                if (IsValidGridPosition(new Vector2Int(checkX + gridSize/2, checkY + gridSize/2)))
                {
                    GameObject obj = GetObjectAtGridPosition(new Vector2Int(checkX , checkY));

                    //Debug.Log("object in radius" + obj);
                    if (obj != null)
                    {
                        if (obj != ignoreObject)
                        {
                            objectsInRadius.Add(obj);
                        }
                    }
                }
            }
        }

        return objectsInRadius;
    }

    public void UpdateGridVisuals()
    {
        for (int x = -gridSize/2; x < gridSize/2; x++)
        {
            for (int y = -gridSize/2; y < gridSize/2; y++)
            {

                // Check if the grid cell is occupied
                bool isOccupied = IsGridPositionOccupied(new Vector2Int(x, y));

                // Change the tile based on occupancy
                if (isOccupied)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), occupiedTile);
                    //Debug.Log("Tile is occupied at " + x + " " + y);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), emptyTile);
                }
            }
        }
    }
}
