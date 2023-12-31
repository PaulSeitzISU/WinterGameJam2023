using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathSearch : MonoBehaviour
{
    AStar aStar;
    public Tilemap searchmap; // Reference to your Tilemap component
    public GridManager gridManager; // Reference to the GridManager

    public TileBase walkTile;

    public List<Vector2Int> masterScan = new List<Vector2Int>();
    public List<Vector2Int> chaceScan = new List<Vector2Int>();

    // Start is called before the first frame update
    void Start()
    {
        aStar = GameObject.Find("Tilemap").GetComponent<AStar>(); // Find the AStar script in the scene
        gridManager = GameObject.Find("Tilemap").GetComponent<GridManager>(); // Find the GridManager in the scene
    }

    public void Search(GameObject self, int depthRadius)
    {
        Debug.Log("Searching");
        masterScan.Clear();

        Vector2Int gridPosition = new Vector2Int((int)self.transform.position.x + (gridManager.gridSize/2), (int)self.transform.position.z + (gridManager.gridSize/2));

        for (int x = -depthRadius; x < depthRadius; x++)
        {
            for (int y =  -depthRadius; y < depthRadius; y++)
            {
                Debug.Log("Searching at " + x + " " + y + " from " + gridPosition.x + " " + gridPosition.y +  "   " + (x + gridPosition.x) + " " + (y + gridPosition.y));
                List<AStar.AStarNode> scans = aStar.FindPath(aStar.grid[gridPosition.x, gridPosition.y], aStar.grid[x + gridPosition.x, y + gridPosition.y]);

                if (scans == null)
                {
                    continue;
                }
                chaceScan.Clear();
                chaceScan = scans.ConvertAll(item => new Vector2Int(item.x, item.y));

                foreach (Vector2Int scanPos in chaceScan)
                {
                    if (!masterScan.Contains(scanPos)) 
                    {
                        masterScan.Add(scanPos);
                    }
                }
            }   
        }
        DisplaySearch();
    }

    public void DisplaySearch()
    {
        ClearDisplay();
        foreach (Vector2Int scan in masterScan)
        {
            searchmap.SetTile(new Vector3Int(scan.x - (gridManager.gridSize/2), scan.y  - (gridManager.gridSize/2), 0), walkTile);
        }
    }

    public void ClearDisplay()
    {
        searchmap.ClearAllTiles();
    }
}

