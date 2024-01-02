using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyAiLongChase : MonoBehaviour
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
        //find path to player
        if(enemyBrain.PlayerList.Count == 0)
        {
            return;
        }
        Vector3Int closestPlayerPos = tilemap.WorldToCell(enemyBrain.ClosestPlayer().transform.position);
        List<AStar.AStarNode> shortest = aStar.FindPath(aStar.grid[movement.currentGridPosition.x, movement.currentGridPosition.y], aStar.grid[closestPlayerPos.x, closestPlayerPos.y]);

        //remove last 3 nodes from path
        shortest.RemoveRange(shortest.Count - (int)enemyBrain.attackDistanceMin, (int)enemyBrain.attackDistanceMin);

        //move to last node in path
        movement.MoveToGrid(tilemap.WorldToCell(closestPlayerPos), enemyBrain.chaseSpeed);

        
    }
}
