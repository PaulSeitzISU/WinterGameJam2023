using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] GameObject selectionObject;
    [SerializeField] GameObject indicator;
    GameObject currentSelection;
    GameObject currentIndicator;

    GameObject primaryIndicator;
    GameObject secondaryIndicator;

    [SerializeField] float selectionRadius = 1f;
    [SerializeField][Range(0,4)] int currentState; // 0 is movement, 1 is Split, 2 is Spit, 3 is Rush, 4 is Leap
    [SerializeField] GameObject[] abilitySprites = new GameObject[5];
    bool switching;
    [Range(0, 3)] int directionFacing; //0 is up, 1 is right, 2 is down, 3 is left
    Vector3[] directionFacingVectors = new Vector3[4] { new Vector3(1, 0), new Vector3(0, 1), new Vector3(-1, 0), new Vector3(0, -1) };
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
        if (currentIndicator != null && currentSelection != null) {
            //Debug.Log(currentIndicator.transform.position);
            //Debug.Log(currentSelection.transform.position);
            //float angle = Vector2.SignedAngle(, );
            float angle = Mathf.Rad2Deg * (Mathf.Atan2(currentIndicator.transform.position.y - currentSelection.transform.position.y, currentIndicator.transform.position.x - currentSelection.transform.position.x));
            if(angle > 45 && angle <= 135)
            {
                directionFacing = 0;
            }
            else if (angle > -45 && angle <= 45)
            {
                directionFacing = 1;
            }
            else if (angle >-135  && angle <= -45)
            {
                directionFacing = 2;
            }
            else if ((angle > -180 && angle <= -135) || (angle <= 180 && angle > 135))
            {
                directionFacing = 3;
            }
            //Debug.Log(angle);
            //Debug.Log(directionFacing);
        }
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
            if(currentSelection != null) { 
                //currentSelection.GetComponent<PathSearch>().Search(gameObject,5);
                if (currentState == 0 && !switching)
                {
                    currentSelection.GetComponent<PlayerController>().Move(MousePositionOnGrid(mousePosition));
                }
                else if (currentState == 1 && currentSelection.GetComponent<PlayerController>().isPlayer() && !switching) // this is a player only ability
                {
                    if(directionFacing==0 || directionFacing == 2) { 
                        currentSelection.GetComponent<PlayerController>().Split(true);
                    }
                    else if (directionFacing == 1 || directionFacing == 3)
                    {
                        currentSelection.GetComponent<PlayerController>().Split(false);
                    }
                }
                else if (currentState == 2 && currentSelection.GetComponent<PlayerController>().isPlayer() && !switching) // this is a player only ability
                {
                    currentSelection.GetComponent<PlayerController>().Spit();
                }
                else if (currentState == 3 && !switching)
                {
                    currentSelection.GetComponent<PlayerController>().Rush();
                }
                else if (currentState == 4 && !switching)
                {
                    currentSelection.GetComponent<PlayerController>().Leap();
                }
            }
            Selection(mousePosition);
        }
        ShowIndicator(currentState);
        switching = false;
    }
    void ShowIndicator(int state)
    {
        if (currentSelection != null && currentIndicator == null)
        {
            if(currentState == 0)
            {
                currentIndicator = Instantiate(indicator, MousePositionOnGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition)), Quaternion.identity);
            }
            else if(currentState == 1) {
                primaryIndicator = Instantiate(indicator, currentSelection.transform.position+directionFacingVectors[directionFacing], Quaternion.identity);
                secondaryIndicator = Instantiate(indicator, currentSelection.transform.position + (-1*directionFacingVectors[directionFacing]), Quaternion.identity);
            }
            else if(currentState == 2) { }   
            else if(currentState == 3) { } 
            else if(currentState == 4) { }   
        }
        else
        {
            if (currentSelection == null) 
            {
                ClearIndicator();
                return; 
            }
            currentIndicator.transform.position = (Vector3Int)currentSelection.GetComponent<Movement>().GetGridTilePosition(MousePositionOnGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
            if(currentState == 1)
            {
                if (primaryIndicator != null)
                {
                    primaryIndicator.transform.position = currentSelection.transform.position + directionFacingVectors[directionFacing];
                }
                else
                {
                    primaryIndicator = Instantiate(indicator, currentSelection.transform.position + directionFacingVectors[directionFacing], Quaternion.identity);
                }
                if (secondaryIndicator != null)
                {
                    secondaryIndicator.transform.position = currentSelection.transform.position + (-1 * directionFacingVectors[directionFacing]);
                }
                else
                {
                    secondaryIndicator = Instantiate(indicator, currentSelection.transform.position + (-1 * directionFacingVectors[directionFacing]), Quaternion.identity);
                }
            }
            else if(currentState == 2) 
            { 
            
            }else if (currentState == 3)
            {

            }

        }
    }
    void ClearIndicator()
    {
        if(currentIndicator != null)Destroy(currentIndicator);
        if(secondaryIndicator != null) Destroy(secondaryIndicator);
    }
    Vector3Int MousePositionOnGrid(Vector3 mousePosition)
    {
        Vector3Int gridPosition = new Vector3Int(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y),0);
        
        return gridPosition;
    }
    GameObject Selection(Vector3 mousePosition)
    {
        mousePosition += new Vector3(0, 0, 10); // brings the mouse position to 0 on the z for selection purposes
        //Debug.Log(mousePosition);
        GameObject thisSelection = null;
        try
        {
            thisSelection = Physics2D.CircleCast(mousePosition, selectionRadius, Vector3.zero, 0).collider.gameObject; // does a circle cast around the mouse click to select the first collider found
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
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
            switching = true;
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
