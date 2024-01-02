using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyAIChase : MonoBehaviour
{
    public Movement movement;
    public GridManager gridManager;
    public EnemyBrain enemyBrain;
    public Tilemap tilemap;
    AStar aStar;

    void Start()
    {
        movement = GetComponent<Movement>();
        gridManager = GameObject.Find("Tilemap").GetComponent<GridManager>();
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        enemyBrain = GetComponent<EnemyBrain>();
        aStar = GameObject.Find("Tilemap").GetComponent<AStar>();
    }

    public void Chase()
    {

        Vector3Int closestPlayerPos = tilemap.WorldToCell(enemyBrain.ClosestPlayer().transform.position);
        
        List<AStar.AStarNode> shortest = aStar.FindPath(aStar.grid[movement.currentGridPosition.x, movement.currentGridPosition.y], aStar.grid[closestPlayerPos.x + 1, closestPlayerPos.y]);
        Vector3Int gridDir = Vector3Int.up;
        List<AStar.AStarNode> tempList = aStar.FindPath(aStar.grid[movement.currentGridPosition.x, movement.currentGridPosition.y], aStar.grid[closestPlayerPos.x, closestPlayerPos.y]);
        if(tempList != null && tempList.Count < shortest.Count)
        {
                shortest = tempList;
                gridDir = Vector3Int.down;
        }
        tempList = aStar.FindPath(aStar.grid[movement.currentGridPosition.x, movement.currentGridPosition.y], aStar.grid[closestPlayerPos.x, closestPlayerPos.y + 1]);
        if(tempList != null && tempList.Count < shortest.Count)
        {
                shortest = tempList;
                gridDir = Vector3Int.left;
        }
        tempList = aStar.FindPath(aStar.grid[movement.currentGridPosition.x, movement.currentGridPosition.y], aStar.grid[closestPlayerPos.x, closestPlayerPos.y - 1]);
        if(tempList != null && tempList.Count < shortest.Count)
        {
                shortest = tempList;
                gridDir = Vector3Int.right;
        }

        movement.MoveToGrid(tilemap.WorldToCell(closestPlayerPos) + gridDir);  
    }
}
