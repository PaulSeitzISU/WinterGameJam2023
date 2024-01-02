using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIPatrolRectangle : MonoBehaviour
{
    public Movement movement; // Reference to the Movement script
    public GridManager gridManager; // Reference to the GridManager script

    public int walkingRange = 5;

    private bool movingToCorner = false;
    private bool isAtCorner = false;
    private int moveCount = 0;
    private int maxMoves = 5; // Number of moves before checking for game objects

    public int yOffSet;
    public int xOffSet;

    private Vector2Int startGridPosition;
    private Vector2Int targetGridPosition;

    void Start()
    {
        startGridPosition = movement.GetGridTilePosition(transform.position);
       // Debug.Log(startGridPosition + " start grid position");
        movement = GetComponent<Movement>(); // Get the Movement component
        gridManager = GameObject.Find("Tilemap").GetComponent<GridManager>(); // Get the GridManager component

    }

    public void Patrol()
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
                if(movement.GetGridTilePosition(transform.position) != targetGridPosition)
                {
                    moveCount--;
                }

                movingToCorner = false;
                Patrol();
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
                    movement.MoveToGrid(new Vector3Int(startGridPosition.x + xOffSet, startGridPosition.y, 0), walkingRange);
                    targetGridPosition = new Vector2Int(startGridPosition.x + xOffSet, startGridPosition.y);
                    break;
                case 1:
                    movement.MoveToGrid(new Vector3Int(startGridPosition.x + xOffSet, startGridPosition.y + yOffSet, 0), walkingRange);
                    targetGridPosition = new Vector2Int(startGridPosition.x + xOffSet, startGridPosition.y + yOffSet);
                    break;
                case 2:
                    movement.MoveToGrid(new Vector3Int(startGridPosition.x, startGridPosition.y + yOffSet, 0), walkingRange);
                    targetGridPosition = new Vector2Int(startGridPosition.x, startGridPosition.y + yOffSet);
                    break;
                case 3:
                    movement.MoveToGrid(new Vector3Int(startGridPosition.x, startGridPosition.y , 0), walkingRange);
                    targetGridPosition = new Vector2Int(startGridPosition.x, startGridPosition.y);
                    break;
            }
            //Debug.Log(startGridPosition + " " + targetGridPosition);
            moveCount++;
        }
        else
        {
            moveCount = 0;
        }
    }
}

