using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] GameObject selectionObject;
    GameObject currentSelection;
    [SerializeField] GameObject indicator;
    GameObject currentIndicator;
    [SerializeField] float selectionRadius = 1f;
    [SerializeField][Range(0,4)] int currentState; // 0 is movement, 1 is Split, 2 is Spit, 3 is Rush, 4 is Leap
    [SerializeField] GameObject[] abilitySprites = new GameObject[5];
    // Start is called before the first frame update
    void Start()
    {
        ChangeState();
    }
    void StartTurn()
    {
        ChangeState();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.mouseScrollDelta != Vector2.zero)
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                ChangeState(currentState++);
            }
            if (Input.mouseScrollDelta.y < 0)
            {
                ChangeState(currentState--);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeState();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeState(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeState(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeState(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ChangeState(4);
        }
        if (Input.GetMouseButtonDown(0)) // The mouse left click input
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (currentSelection == null)
            {
                Selection(mousePosition);
                currentSelection.GetComponent<PathSearch>().Search(gameObject,5);
            }
            else if (currentState == 0)
            {
                currentSelection.GetComponent<PlayerController>().Move(MousePositionOnGrid(mousePosition));
            }
            else if (currentState == 1 && currentSelection.GetComponent<PlayerController>().isPlayer()) // this is a player only ability
            {
                currentSelection.GetComponent<PlayerController>().Split();
            }
            else if (currentState == 2 && currentSelection.GetComponent<PlayerController>().isPlayer()) // this is a player only ability
            {
                currentSelection.GetComponent<PlayerController>().Spit();
            }
            else if (currentState == 3)
            {
                currentSelection.GetComponent<PlayerController>().Rush();
            }
            else if (currentState == 4 )
            {
                currentSelection.GetComponent<PlayerController>().Leap();
            }
            else {Selection(mousePosition);}
        }
        if (currentSelection != null && currentIndicator == null) { 
            currentIndicator = Instantiate(indicator,MousePositionOnGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition)),Quaternion.identity);
        }
        else
        {
            if (currentSelection == null) return;
            currentIndicator.transform.position = (Vector3Int)currentSelection.GetComponent<Movement>().GetGridTilePosition(MousePositionOnGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        }
    }
    Vector3Int MousePositionOnGrid(Vector3 mousePosition)
    {
        Vector3Int gridPosition = new Vector3Int(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y),0);
        
        return gridPosition;
    }
    GameObject Selection(Vector3 mousePosition)
    {
        mousePosition += new Vector3(0, 0, 10); // brings the mouse position to 0 on the z for selection purposes
        Debug.Log(mousePosition);
        GameObject thisSelection = null;
        try
        {
            thisSelection = Physics2D.CircleCast(mousePosition, selectionRadius, Vector3.zero, 0).collider.gameObject; // does a circle cast around the mouse click to select the first collider found
        }
        catch (System.Exception e)
        {
            Debug.Log("No selected object");
        }
        if (thisSelection == null)
        {
            if (currentSelection != null)
            {
                currentSelection.GetComponent<PlayerController>().DeSelection();
                currentSelection = null;
            }
            return currentSelection;
        }

        if (!currentSelection)
        {
            currentSelection = thisSelection;
            currentSelection.GetComponent<PlayerController>().Selected();
        }
        else if (thisSelection == currentSelection) { return currentSelection; }
        else
        {
            currentSelection.GetComponent<PlayerController>().DeSelection();
            currentSelection = thisSelection;
            currentSelection.GetComponent<PlayerController>().Selected();
        }
        return thisSelection;
    }
    void ChangeState(int state = 0)
    {

        currentState = state;

        if (currentState > 4)
        {
            currentState = 0;
        }
        else if (currentState < 0)
        {
            currentState = 4;
        }
        selectionObject.transform.position = abilitySprites[currentState].transform.position;
    }
}
