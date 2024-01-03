using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DicTurn
{
    private bool _hasAttacked = false;
    private bool _hasMoved = false;

    public bool Done { get { return _hasAttacked && _hasMoved; } }

    public bool hasAttacked
    {
        get { return _hasAttacked; }
        set { _hasAttacked = value; }
    }

    public bool hasMoved
    {
        get { return _hasMoved; }
        set { _hasMoved = value; }
    }
}

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] GameObject selectionObject;
    [SerializeField] GameObject indicator;
    public GameObject currentSelection;
    Vector3 indcatorOffset = new Vector3(0.5f, 0.5f, 0);
    GameObject currentIndicator;

    GameObject primaryIndicator;
    GameObject secondaryIndicator;

    [SerializeField] float selectionRadius = 1f;
    [SerializeField][Range(0,4)] int currentState; // 0 is movement, 1 is Split, 2 is Spit, 3 is Rush, 4 is Leap

    public GameObject selectionSelection;
    [SerializeField] GameObject[] abilitySprites = new GameObject[5];
    bool switching;
    [Range(0, 3)] int directionFacing; //0 is up, 1 is right, 2 is down, 3 is left
    Vector3[] directionFacingVectors = new Vector3[4] { new Vector3(0, 1), new Vector3(1, 0), new Vector3(0, -1), new Vector3(-1, 0) };

    GridManager gridManager;
    Tilemap tilemap; // Reference to your Tilemap component
    GameObject targetCurrent;
    public bool isTurn = true;
    Camera cam;

    public Dictionary<GameObject, DicTurn> trackTurn = new Dictionary<GameObject, DicTurn>();

    // Start is called before the first frame update
    void Start()
    {
        UpdatePlayerList();

        gridManager = GameObject.Find("Tilemap").GetComponent<GridManager>(); // Find the GridManager in the scene
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>(); // Get the Tilemap component
        ChangeState();
        cam = Camera.main;

        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        cam.GetComponent<CameraFollow>().target = playerTransform;
        cam.transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, cam.transform.position.z);
    }

/*    private void FixedUpdate()
    {
        UpdatePlayerList();
    }
*/
    public void UpdatePlayerList()
    {
        trackTurn.Clear();
        //find all players
        GameObject[] PlayerList = GameObject.FindGameObjectsWithTag("Player");
        //add them to the dictionary
        foreach (GameObject player in PlayerList)
        {
            if (!trackTurn.ContainsKey(player))
            {
                trackTurn.Add(player, new DicTurn());
            }
        }
    }

    public void StartTurn()
    {
        ChangeState();
    }


    // Update is called once per frame
    void Update()
    {
        foreach (KeyValuePair<GameObject, DicTurn> entry in trackTurn)
        {
            if (entry.Value.Done)
            {
                entry.Key.GetComponentInChildren<SpriteRenderer>().color = Color.gray;
            } else
            {
                entry.Key.GetComponentInChildren<SpriteRenderer>().color = Color.white;
            }
        }

        if(currentSelection != null)
        {
            cam.GetComponent<CameraFollow>().target = currentSelection.transform;

        } else if (currentSelection == null && trackTurn.Count > 0 && cam.GetComponent<CameraFollow>().target == null)
        {
            cam.GetComponent<CameraFollow>().target = trackTurn.Keys.GetEnumerator().Current.transform;
        } else if (currentSelection == null && trackTurn.Count == 0)
        {
            cam.GetComponent<CameraFollow>().target = trackTurn.ToArray()[0].Key.transform;
        }
        
        if(!isTurn)
        {
        return;        
        }

        bool tempBool = false;

        //check if it is the players turn
        foreach (KeyValuePair<GameObject, DicTurn> entry in trackTurn)
        {
            if (!entry.Value.Done)
            {
                tempBool = true;
            }
        }

        isTurn = tempBool;

        if (currentSelection != null) {
            //Debug.Log(currentIndicator.transform.position);
            //Debug.Log(currentSelection.transform.position);
            //float angle = Vector2.SignedAngle(, );

            //Debug.Log(Input.mousePosition + " " + currentSelection.transform.position);
            Vector3 mousePositionTemp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float angle = Mathf.Rad2Deg * (Mathf.Atan2(mousePositionTemp.y - currentSelection.transform.position.y, mousePositionTemp.x - currentSelection.transform.position.x));
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
        //Debug.Log(currentState + " " + currentIndicator);
        if(currentIndicator != null)
        {
            //Debug.Log("Checking for target    " + currentState + " state");
            Vector3Int tempVec = tilemap.WorldToCell(currentIndicator.transform.position);

            GameObject target = gridManager.GetObjectAtGridPosition(new Vector2Int(tempVec.x, tempVec.y ));

            if (currentState == 2)
            {
                if (target != null && target.tag == "Enemy")
                {
                    //turn currentSelection sprite green 
                    currentIndicator.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                    //Debug.Log("Targeting enemy");
                    targetCurrent = target;
                }
                else
                {
                    //turn currentSelection sprite red
                    currentIndicator.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                    targetCurrent = null;
                }
            }
            else if (currentState == 0)
            {
                if (CanMoveToSelector())
                {
                    currentIndicator.gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;
                }
                else
                {
                    currentIndicator.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
            else
            {
                //turn currentSelection sprite cyan
                currentIndicator.gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;
                //Debug.Log("Not targeting enemy");

            }


            
        } 
        if(primaryIndicator != null)
            {
                 //Debug.Log("Checking for target    " + currentState + " state");
            Vector3Int tempVec = tilemap.WorldToCell(primaryIndicator.transform.position);

            GameObject target = gridManager.GetObjectAtGridPosition(new Vector2Int(tempVec.x, tempVec.y ));
                if(currentState == 3)
                {
                    if(target != null && target.tag == "Enemy")
                    {
                        //turn currentSelection sprite green 
                        primaryIndicator.gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                        //Debug.Log("Targeting enemy");
                        targetCurrent = target;
                    }
                    else
                    {
                        //turn currentSelection sprite red
                        primaryIndicator.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                        targetCurrent = null;
                    }
                }
                else
                {
                    //turn currentSelection sprite red
                    primaryIndicator.gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;
                    //Debug.Log("Not targeting enemy");

                }
            }



        #region Inputs

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
        // else if (Input.GetKeyDown(KeyCode.Alpha5))
        // {
        //     ChangeState(4);
        // }
        if (Input.GetMouseButtonDown(0)) // The mouse left click input
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(currentSelection != null) { 
                //currentSelection.GetComponent<PathSearch>().Search(gameObject,5);
                if (currentState == 0 && !switching && trackTurn[currentSelection].hasMoved == false && CanMoveToSelector())
                {
                    currentSelection.GetComponent<PlayerController>().Move(PositionOnGrid(mousePosition));
                    trackTurn[currentSelection].hasMoved = true;
                }
                else if (currentState == 1 && currentSelection.GetComponent<PlayerController>().isPlayer() && !switching && trackTurn[currentSelection].hasAttacked == false) // this is a player only ability
                {
                    if(directionFacing==0 || directionFacing == 2) { 
                        currentSelection.GetComponent<PlayerController>().Split(true);
                    }
                    else if (directionFacing == 1 || directionFacing == 3)
                    {
                        currentSelection.GetComponent<PlayerController>().Split(false);
                    }
                    trackTurn[currentSelection].hasAttacked = true;
                }
                else if (currentState == 2 && currentSelection.GetComponent<PlayerController>().isPlayer() && !switching && trackTurn[currentSelection].hasAttacked == false) // this is a player only ability
                {
                    if (targetCurrent != null)
                    {
                        currentSelection.GetComponent<PlayerController>().Spit(targetCurrent);
                    }
                    trackTurn[currentSelection].hasAttacked = true;
                }
                else if (currentState == 3 && !switching && trackTurn[currentSelection].hasAttacked == false)
                {

                    currentSelection.GetComponent<PlayerController>().Rush(targetCurrent);
                    trackTurn[currentSelection].hasAttacked = true;
                }
                else if (currentState == 4 && !switching && trackTurn[currentSelection].hasAttacked == false)
                {
                    currentSelection.GetComponent<PlayerController>().Leap();
                    trackTurn[currentSelection].hasAttacked = true;
                }
                else
                {
                    Selection(mousePosition);
                }
            }
            else
            {
                Selection(mousePosition);
            }
        }

        #endregion

        ShowIndicator(currentState);
        switching = false;
    }
   void ShowIndicator(int state)
{
    if (currentSelection != null && currentIndicator == null)
    {
        if(trackTurn == null) return;
        if (currentState == 0 && trackTurn[currentSelection].hasMoved == false)  
        {
            currentIndicator = Instantiate(indicator, PositionOnGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition)) + indcatorOffset, Quaternion.identity);
        }
        else if (currentState == 1 && trackTurn[currentSelection].hasAttacked == false && currentSelection.GetComponent<PlayerController>().isPlayer())
        {
            if (primaryIndicator == null)
            {
                primaryIndicator = Instantiate(indicator, currentSelection.transform.position + directionFacingVectors[directionFacing], Quaternion.identity);
                //Debug.Log("Primary Indicator Created" + primaryIndicator.transform.position + " " + currentSelection.transform.position + " " + directionFacingVectors[directionFacing]);
            }
            else
            {
                primaryIndicator.transform.position = currentSelection.transform.position + directionFacingVectors[directionFacing];
                //Debug.Log("Primary Indicator Moved" + primaryIndicator.transform.position + " " + currentSelection.transform.position + " " + directionFacingVectors[directionFacing]);
            }

            if (secondaryIndicator == null)
            {
                secondaryIndicator = Instantiate(indicator, currentSelection.transform.position + (-1 * directionFacingVectors[directionFacing]), Quaternion.identity);
            }
            else
            {
                secondaryIndicator.transform.position = currentSelection.transform.position + (-1 * directionFacingVectors[directionFacing]);
            }
        }
        else if (currentState == 2 && trackTurn[currentSelection].hasAttacked == false && currentSelection.GetComponent<PlayerController>().isPlayer())
        {
            if (currentIndicator == null)
            {
                currentIndicator = Instantiate(indicator, PositionOnGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition))  + indcatorOffset, Quaternion.identity);
            }
            else
            {
                currentIndicator.transform.position = PositionOnGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition))  + indcatorOffset;
            }
        }
        else if (currentState == 3 && trackTurn[currentSelection].hasAttacked == false) 
        { 
            if (primaryIndicator == null)
            {
                primaryIndicator = Instantiate(indicator, currentSelection.transform.position + directionFacingVectors[directionFacing], Quaternion.identity);
                //Debug.Log("Primary Indicator Created" + primaryIndicator.transform.position + " " + currentSelection.transform.position + " " + directionFacingVectors[directionFacing]);
            }
            else
            {
                primaryIndicator.transform.position = currentSelection.transform.position + directionFacingVectors[directionFacing];
                //Debug.Log("Primary Indicator Moved" + primaryIndicator.transform.position + " " + currentSelection.transform.position + " " + directionFacingVectors[directionFacing]);
            }
        }
        else if (currentState == 4 && trackTurn[currentSelection].hasAttacked == false) 
        {

         }
    }
    else
    {
        // Existing indicators, move them to the updated position
        if (currentSelection == null)
        {
            ClearIndicator();
            return;
        }
        currentIndicator.transform.position = (Vector3Int)currentSelection.GetComponent<Movement>().GetGridTilePosition(PositionOnGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        if (currentState == 1)
        {
            if (primaryIndicator != null)
            {
                primaryIndicator.transform.position = currentSelection.transform.position + directionFacingVectors[directionFacing];
            }
            if (secondaryIndicator != null)
            {
                secondaryIndicator.transform.position = currentSelection.transform.position + (-1 * directionFacingVectors[directionFacing]);
            }
        }
        else if (currentState == 2)
        {
            if (primaryIndicator != null)
            {
                primaryIndicator.transform.position = PositionOnGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }
        else if (currentState == 3) {
            if (primaryIndicator != null)
            {
                primaryIndicator.transform.position = currentSelection.transform.position + directionFacingVectors[directionFacing];
            }
         }
        else if (currentState == 4) { }
    }
}

    void ClearIndicator()
    {
        if (currentIndicator != null)Destroy(currentIndicator);
        if (primaryIndicator != null) Destroy(primaryIndicator);
        if (secondaryIndicator != null) Destroy(secondaryIndicator);
    }
    Vector3Int PositionOnGrid(Vector3 mousePosition)
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
        catch (System.Exception)
        {
            Debug.Log("No selected object");
        }

        if (thisSelection == null || (thisSelection != null && thisSelection.tag != "Player"))
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
        
        // Assuming 'selectionSelection' is the GameObject containing a RectTransform of the element you want to move,
        // and 'abilitySprites' is an array of GameObjects, each containing a RectTransform representing the abilities.

        // Get the RectTransform component from the 'selectionSelection' GameObject
        RectTransform selectionSelectionRectTransform = selectionSelection.GetComponent<RectTransform>();

        // Get the RectTransform of the current ability sprite based on 'currentState'
        RectTransform currentAbilityRect = abilitySprites[currentState].GetComponent<RectTransform>();

        // Set the anchored position of 'selectionSelection' to be centered at the position of 'currentAbilityRect'
        selectionSelectionRectTransform.anchoredPosition = currentAbilityRect.anchoredPosition - new Vector2(0.8f,1f);



        ClearIndicator();
        //Debug.Log("Current State: " + currentState);
        //selectionObject.transform.position = abilitySprites[currentState].transform.position;
    }    

    private bool CanMoveToSelector()
    {
        Movement movement = currentSelection.GetComponent<Movement>();
        return movement.CalculatePath(
            movement.GetGridTilePosition(currentSelection.transform.position),
            movement.GetGridTilePosition(currentIndicator.transform.position)
        ).Count <= currentSelection.GetComponent<PlayerController>().moveDistance;
    }
}