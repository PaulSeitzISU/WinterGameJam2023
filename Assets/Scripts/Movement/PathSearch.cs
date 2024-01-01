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
    public TileBase AttackTile;

    public List<Vector2Int> masterScan = new List<Vector2Int>();
    public List<Vector2Int> chaceScan = new List<Vector2Int>();

    // Start is called before the first frame update
    void Start()
    {
        aStar = GameObject.Find("Tilemap").GetComponent<AStar>(); // Find the AStar script in the scene
        gridManager = GameObject.Find("Tilemap").GetComponent<GridManager>(); // Find the GridManager in the scene

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Search(gameObject, 3);
        }
    }

    public void Search(GameObject obj, int depthRadius)
    {
        Debug.Log("Searching");
        masterScan.Clear();
        Vector3Int gridPosition = searchmap.WorldToCell(obj.transform.position) + new Vector3Int((gridManager.gridSize/2), (gridManager.gridSize/2), 0);
        //Vector2Int gridPosition = new Vector2Int((int)obj.transform.position.x + (gridManager.gridSize/2), (int)obj.transform.position.y + (gridManager.gridSize/2));

        for (int x = -depthRadius; x < depthRadius + 1; x++)
        {
            for (int y =  -depthRadius ; y < depthRadius + 1; y++)
            {
                Debug.Log("Searching at " + x + " " + y + " from " + gridPosition.x + " " + gridPosition.y +  "   " + (x + gridPosition.x) + " " + (y + gridPosition.y) + " " + aStar.grid[gridPosition.x, gridPosition.y] + " " + aStar.grid[x + gridPosition.x, y + gridPosition.y]);
                
                List<AStar.AStarNode> scans = aStar.FindPath(aStar.grid[gridPosition.x, gridPosition.y], aStar.grid[x + gridPosition.x, y + gridPosition.y]);

                if (scans == null)
                {
                    Debug.Log("No path found." + x + " " + y);
                    continue;
                }

                chaceScan.Clear();
                chaceScan = scans.ConvertAll(item => new Vector2Int(item.x, item.y));
                //list chace scan

                foreach (Vector2Int scanPos in chaceScan)
                {
                    //Debug.Log("Adding " + scanPos.x + " " + scanPos.y);

                    if (!masterScan.Contains(scanPos)) 
                    {
                        masterScan.Add(scanPos);
                    } else
                    {
                        //Debug.Log("Already contains " + scanPos.x + " " + scanPos.y);
                    }
                }
            }   
        }
        DisplaySearch(obj);
    }

    public void DisplaySearch(GameObject obj)
    {
        ClearDisplay();
        foreach (Vector2Int scan in masterScan)
        {
            Debug.Log("Displaying at " + scan.x + " " + scan.y);
            searchmap.SetTile(new Vector3Int(scan.x - (gridManager.gridSize/2), scan.y  - (gridManager.gridSize/2), 0), walkTile);
        }

        foreach (Vector2Int scan in masterScan)
        {
            //set the tile i am standing on to a walk tile
            if (scan == new Vector2Int((int)obj.transform.position.x + (gridManager.gridSize/2), (int)obj.transform.position.y + (gridManager.gridSize/2)))
            {
                searchmap.SetTile(new Vector3Int(scan.x - (gridManager.gridSize/2), scan.y - (gridManager.gridSize/2), 0), walkTile);
            }

            //find tiles that are up down left right of the scan and if there is no tile in them then place a attack tile
            if (!masterScan.Contains(new Vector2Int(scan.x + 1, scan.y)))
            {
                searchmap.SetTile(new Vector3Int(scan.x + 1 - (gridManager.gridSize/2), scan.y - (gridManager.gridSize/2), 0), AttackTile);
            }
            if (!masterScan.Contains(new Vector2Int(scan.x - 1, scan.y)))
            {
                searchmap.SetTile(new Vector3Int(scan.x - 1 - (gridManager.gridSize/2), scan.y - (gridManager.gridSize/2), 0), AttackTile);
            }
            if (!masterScan.Contains(new Vector2Int(scan.x, scan.y + 1)))
            {
                searchmap.SetTile(new Vector3Int(scan.x - (gridManager.gridSize/2), scan.y + 1 - (gridManager.gridSize/2), 0), AttackTile);
            }
            if (!masterScan.Contains(new Vector2Int(scan.x, scan.y - 1)))
            {
                searchmap.SetTile(new Vector3Int(scan.x - (gridManager.gridSize/2), scan.y - 1 - (gridManager.gridSize/2), 0), AttackTile);
            }
        }
    }

    public void ClearDisplay()
    {
        searchmap.ClearAllTiles();
    }
}

