using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIPatrolRandom : MonoBehaviour
{   
    public Movement movement; // Reference to the Movement script
    public GridManager gridManager; // Reference to the GridManager script

    public int range = 5;

    private bool isMovingRandomly = false;

    private Vector2Int startGridPosition;
    private Vector2Int targetGridPosition;

    private void Start()
    {
        movement = GetComponent<Movement>(); // Get the Movement component
        gridManager = GameObject.Find("Tilemap").GetComponent<GridManager>(); // Get the GridManager component
        startGridPosition = movement.GetGridTilePosition(transform.position);
    }

    public void Patrol()
    {
        if (!isMovingRandomly)
        {
            MoveRandomly();
        }
        else
        {
            // Check if the enemy has finished moving randomly
            if (!movement.isMoving)
            {
                isMovingRandomly = false;
            }
        }
    }

    private void MoveRandomly()
    {
        isMovingRandomly = true;

        Vector2Int randomDirection = new Vector2Int(Random.Range(-range, range), Random.Range(-range, range));
        targetGridPosition = startGridPosition + randomDirection;

        // Ensure the target position stays within grid bounds
        targetGridPosition.x = Mathf.Clamp(targetGridPosition.x, -(gridManager.gridSize/2) + 1, (gridManager.gridSize/2) - 1);
        targetGridPosition.y = Mathf.Clamp(targetGridPosition.y, -(gridManager.gridSize/2) + 1, (gridManager.gridSize/2) - 1);

        movement.MoveToGrid(new Vector3Int(targetGridPosition.x, targetGridPosition.y, 0));

    }
}
