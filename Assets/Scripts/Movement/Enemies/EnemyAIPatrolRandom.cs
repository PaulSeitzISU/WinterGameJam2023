using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIPatrolRandom : MonoBehaviour
{   
    public Movement movement; // Reference to the Movement script
    public GridManager gridManager; // Reference to the GridManager script
    EnemyBrain enemyBrain;
    private bool isMovingRandomly = false;

    private Vector2Int startGridPosition;
    private Vector2Int targetGridPosition;
    private int moveAttempts = 0;
    private int maxMoveAttempts = 10;


    private void Start()
    {
        movement = GetComponent<Movement>(); // Get the Movement component
        gridManager = GameObject.Find("Tilemap").GetComponent<GridManager>(); // Get the GridManager component
        startGridPosition = movement.GetGridTilePosition(transform.position);
        enemyBrain = GetComponent<EnemyBrain>();
    }

    public void Patrol()
    {
        if (!isMovingRandomly)
        {
            moveAttempts = 0;
            MoveRandomly();
        }
        else
        {
            // Check if the enemy has finished moving randomly
            if (!movement.isMoving)
            {
                isMovingRandomly = false;
                Patrol();
            }
        }
    }

    private void MoveRandomly()
    {
        isMovingRandomly = true;

        Vector2Int randomDirection = new Vector2Int(Random.Range(-enemyBrain.patrolSpeed, enemyBrain.patrolSpeed), Random.Range(-enemyBrain.patrolSpeed, enemyBrain.patrolSpeed));
        targetGridPosition = startGridPosition + randomDirection;

        // Ensure the target position stays within grid bounds
        targetGridPosition.x = Mathf.Clamp(targetGridPosition.x, -(gridManager.gridSize/2) + 1, (gridManager.gridSize/2) - 1);
        targetGridPosition.y = Mathf.Clamp(targetGridPosition.y, -(gridManager.gridSize/2) + 1, (gridManager.gridSize/2) - 1);

        bool moved = movement.MoveToGrid(new Vector3Int(targetGridPosition.x, targetGridPosition.y, 0), enemyBrain.patrolSpeed);
        
        //Debug.Log("Enemy moving to " + targetGridPosition + ". Moved: " + moved);
        if(!moved && moveAttempts < maxMoveAttempts)
        {
            moveAttempts++;
            //Debug.Log("Enemy cannot move to " + targetGridPosition + ". Trying again.");
            MoveRandomly();
        }
    }
}
