using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


public class EnemyAiFlee : MonoBehaviour
{
    AStar aStar;
    public Tilemap tilemap; // Reference to your Tilemap component
    public GridManager gridManager; // Reference to the GridManager
    public EnemyBrain enemyBrain;
    public Movement movement;
    public List<Vector2Int> chaceScan = new List<Vector2Int>();


    // Start is called before the first frame update
    void Start()
    {
        aStar = GameObject.Find("Tilemap").GetComponent<AStar>(); // Find the AStar script in the scene
        gridManager = GameObject.Find("Tilemap").GetComponent<GridManager>(); // Find the GridManager in the scene
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>(); // Get the Tilemap component
        enemyBrain = GetComponent<EnemyBrain>();
        movement = GetComponent<Movement>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Flee()
    {
        Vector2Int bestPos = FleeSearch(enemyBrain.patrolSpeed);
        if(bestPos != Vector2Int.zero)
        {
            movement.MoveToGrid(new Vector3Int(bestPos.x - (gridManager.gridSize/2),bestPos.y - (gridManager.gridSize/2),0), enemyBrain.patrolSpeed);
        } else
        {
            Debug.Log("Panic!!! no best move found");
        }

    }

    public Vector2Int FleeSearch(int depthRadius)
    {
        Debug.Log("Searching");

        Vector3Int gridPosition = tilemap.WorldToCell(transform.position) + new Vector3Int(gridManager.gridSize/2, gridManager.gridSize/2, 0);
        //Vector2Int gridPosition = new Vector2Int((int)obj.transform.position.x + (gridManager.gridSize/2), (int)obj.transform.position.y + (gridManager.gridSize/2));

        HashSet<Vector2Int> scanSet = new HashSet<Vector2Int>();
        Vector2Int bestPos = Vector2Int.zero;

        int fartherest = 0;

        for (int x = -depthRadius; x < depthRadius + 1; x++)
        {
            for (int y =  -depthRadius ; y < depthRadius + 1; y++)
            {
                //check scan
                if(scanSet.Contains(new Vector2Int(x + gridPosition.x, y + gridPosition.y)))
                {
                    continue;
                }

                List<AStar.AStarNode> scans = aStar.FindPath(aStar.grid[gridPosition.x, gridPosition.y], aStar.grid[x + gridPosition.x, y + gridPosition.y]);

                if (scans == null)
                {
                    Debug.Log("No path found." + x + " " + y);
                    continue;
                }

                //save scan

                chaceScan.Clear();
                chaceScan = scans.ConvertAll(item => new Vector2Int(item.x, item.y));

                if(chaceScan.Count < depthRadius)
                {
                    chaceScan = chaceScan.Take(depthRadius).ToList();
                }

                if(chaceScan.Count == 0)
                {  
                    Debug.Log("No path found." + x + " " + y);
                    continue;
                }
                //list chace scan

                Vector2Int playerPos = movement.GetGridTilePosition(enemyBrain.ClosestPlayer().transform.position) + new Vector2Int((gridManager.gridSize/2), (gridManager.gridSize/2));
                

                //Debug.Log(chaceScan[chaceScan.Count - 1].x + "  " + chaceScan[chaceScan.Count - 1].y + "    " + playerPos.x + "  " + playerPos.y);
                //Debug.Log(aStar.grid[72, 83] + "  " + aStar.grid[playerPos.x,playerPos.y]);
                List<AStar.AStarNode> scanCheck = aStar.FindPath(aStar.grid[playerPos.x,playerPos.y], aStar.grid[chaceScan[chaceScan.Count - 1].x, chaceScan[chaceScan.Count - 1].y]);
                Debug.Log(scanCheck);
                if(scanCheck != null && scanCheck.Count > fartherest)
                {
                    Debug.Log("Found path with " + scanCheck.Count + " nodes.");
                    fartherest = scanCheck.Count;
                } else
                {
                    if(scanCheck != null)
                    {
                        Debug.Log("Found path with " + scanCheck.Count + " nodes but failed.");
                    }
                    Debug.Log("No path found.");
                    continue;
                }

                foreach (Vector2Int scanPos in chaceScan)
                {
                    if (!scanSet.Contains(scanPos)) 
                    {
                        scanSet.Add(scanPos);
                        bestPos = chaceScan[chaceScan.Count - 1];
                    } 
                }
            }   
        }

        if(fartherest > enemyBrain.attackDistanceMin)
        {
            enemyBrain.TransitionToState(EnemyState.Attack);
            enemyBrain.UpdateState();
        }
        return bestPos;
    }
}
