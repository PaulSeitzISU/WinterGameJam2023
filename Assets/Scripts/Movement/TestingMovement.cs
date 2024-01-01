using UnityEngine;

public class TestingMovement : MonoBehaviour
{
    public Movement movementScript; // Reference to your Movement script
    public Vector3Int targetGridPosition; // Set the target grid position in the inspector

    void Start()
    {


        movementScript = GetComponent<Movement>(); // Get the Movement script component
        if (movementScript == null)
        {
            Debug.Log("No Movement script found on this GameObject.");
            return;
        }

        // Call the MoveToGrid function with the targetGridPosition when the script starts
        movementScript.MoveToGrid(targetGridPosition);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Example: Press space to move to a new random grid position
        {
            Vector3Int randomGridPosition = GetRandomGridPosition();
            //Debug.Log("Moving to grid position: " + randomGridPosition);
            movementScript.MoveToGrid(randomGridPosition);
        }
    }

    Vector3Int GetRandomGridPosition()
    {
        int minX = -5; // Set your minimum X grid coordinate
        int maxX = 5;  // Set your maximum X grid coordinate
        int minY = -5; // Set your minimum Y grid coordinate
        int maxY = 5;  // Set your maximum Y grid coordinate

        int randomX = Random.Range(minX, maxX + 1);
        int randomY = Random.Range(minY, maxY + 1);

        return new Vector3Int(randomX, randomY, 0);
    }
}
