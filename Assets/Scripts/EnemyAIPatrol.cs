using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIPatrol : MonoBehaviour
{
    public Movement movement; // Reference to the Movement script
    public GridManager gridManager; // Reference to the GridManager script

    private bool movingToCorner = false;
    private bool isAtCorner = false;
    private int moveCount = 0;
    private int maxMoves = 5; // Number of moves before checking for game objects

    public int yOffSet;
    public int xOffSet;

    public int visibilityRadius;

    private Vector2Int startGridPosition;
    private Vector2Int targetGridPosition;

    private void Start()
    {
        movement = GetComponent<Movement>(); // Get the Movement component
        gridManager = GameObject.Find("Tilemap").GetComponent<GridManager>(); // Get the GridManager component
        startGridPosition = movement.GetGridTilePosition();
    }

    private void FixedUpdate()
    {
        // Move in a rectangle pattern
        if (!movingToCorner)
        {
            MoveToNextCorner();
        }
        else
        {

            // When at a corner, check if the enemy is within a specific tile
            if (!movement.isMoving)
            {
                movingToCorner = false;
            }
        }
    }

    private void MoveToNextCorner()
    {
        if (moveCount < maxMoves)
        {
            movingToCorner = true;

            switch (moveCount)
            {
                case 0:
                    targetGridPosition = new Vector2Int(startGridPosition.x + xOffSet, startGridPosition.y);
                    movement.MoveToGrid(new Vector3Int(startGridPosition.x + xOffSet, startGridPosition.y, 0));
                    
                    break;
                case 1:
                    targetGridPosition = new Vector2Int(startGridPosition.x + xOffSet, startGridPosition.y + yOffSet);
                    movement.MoveToGrid(new Vector3Int(startGridPosition.x + xOffSet, startGridPosition.y + yOffSet, 0));
                    break;
                case 2:
                    targetGridPosition = new Vector2Int(startGridPosition.x, startGridPosition.y + yOffSet);
                    movement.MoveToGrid(new Vector3Int(startGridPosition.x, startGridPosition.y + yOffSet, 0));
                    break;
                case 3:
                    targetGridPosition = new Vector2Int(startGridPosition.x, startGridPosition.y);
                    movement.MoveToGrid(new Vector3Int(startGridPosition.x, startGridPosition.y , 0));
                    break;
            }
            moveCount++;
        }
        else
        {
            moveCount = 0;
        }
    }

        private void CheckForGameObjects()
    {
        List<GameObject> objectsInRadius = gridManager.GetObjectsInRadius(movement.GetGridTilePosition(), visibilityRadius);
        if (objectsInRadius.Count > 0)
        {
            Debug.Log("Objects detected at corner: " + objectsInRadius.Count);
            foreach (GameObject obj in objectsInRadius)
            {
                Debug.Log("Object detected: " + obj.name);
            }
        }
    }

}

