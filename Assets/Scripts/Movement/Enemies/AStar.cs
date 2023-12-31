using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public class AStarNode
    {
        public int x;
        public int y;
        public bool isWalkable;
        public int gCost;
        public int hCost;
        public AStarNode parent;

        public int FCost { get { return gCost + hCost; } }

        public AStarNode(bool walkable, int gridX, int gridY)
        {
            isWalkable = walkable;
            x = gridX;
            y = gridY;
        }
    }

    GridManager gridManager;

    public AStarNode[,] grid;
    private void Start()
    {
        gridManager = GameObject.Find("Tilemap").GetComponent<GridManager>(); // Find the GridManager in the scene
        // Initialize the grid
        InitializeGrid();
    }

    public void InitializeGrid()
    {
        grid = new AStarNode[gridManager.gridSize, gridManager.gridSize];

        // Example: Assign values to the grid nodes (walkable or not)
        for (int x = 0; x < gridManager.gridSize; x++)
        {
            for (int y = 0; y < gridManager.gridSize; y++)
            {
                bool isWalkable = !gridManager.IsGridPositionOccupied(new Vector2Int(x - (gridManager.gridSize/2), y - (gridManager.gridSize/2)));
                grid[x, y] = new AStarNode(isWalkable, x, y);
            }
        }

        // For demonstration purposes, perform the A* algorithm here
        AStarNode startNode = grid[0, 0];
        AStarNode targetNode = grid[gridManager.gridSize - 1, gridManager.gridSize - 1];

        List<AStarNode> path = FindPath(startNode, targetNode);
        if (path != null)
        {
            Debug.Log("Found path with " + path.Count + " nodes.");
        }
        else
        {
            Debug.Log("No path found.");
        }
    }

    public List<AStarNode> FindPath(AStarNode startNode, AStarNode targetNode)
    {
        List<AStarNode> openSet = new List<AStarNode>();
        HashSet<AStarNode> closedSet = new HashSet<AStarNode>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            AStarNode currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (AStarNode neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
        return null; // No path found
    }


    List<AStarNode> RetracePath(AStarNode startNode, AStarNode endNode)
    {
        List<AStarNode> path = new List<AStarNode>();
        AStarNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
            //Debug.Log(currentNode.x + " " + currentNode.y + " " + currentNode.parent.x + " " + currentNode.parent.y);
        }
        path.Reverse();

        return path;
    }

    List<AStarNode> GetNeighbors(AStarNode node)
    {
        List<AStarNode> neighbors = new List<AStarNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.x + x;
                int checkY = node.y + y;

                if (checkX >= 0 && checkX < gridManager.gridSize && checkY >= 0 && checkY < gridManager.gridSize)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

    int GetDistance(AStarNode nodeA, AStarNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.x - nodeB.x);
        int dstY = Mathf.Abs(nodeA.y - nodeB.y);

        return dstX + dstY;
    }
}
