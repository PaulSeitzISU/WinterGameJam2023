using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    GameObject currentSelection;
    [SerializeField]float selectionRadius = 1f;
    [SerializeField][Range(0,4)]int currentState; // 0 is movement, 1 is Split, 2 is Spit, 3 is Rush, 4 is Leap
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentSelection == null)
            {
                Selection();
            }else if (currentState == 0)
            {

            }
            else if (currentState == 1)
            {

            }
            else if (currentState == 2)
            {

            }
            else if (currentState == 3)
            {

            }
            else if (currentState == 4)
            {

            }
            else {Selection();}
        }
    }
    void Selection()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
            return;
        }

        if (!currentSelection)
        {
            currentSelection = thisSelection;
            currentSelection.GetComponent<PlayerController>().Selected();
            //Debug.Log(hitInfo.collider.gameObject);
            //currentSelection = overlaps[0].gameObject;
        }
        else if (thisSelection == currentSelection) { return; }
        else
        {
            currentSelection.GetComponent<PlayerController>().DeSelection();
            currentSelection = thisSelection;
            currentSelection.GetComponent<PlayerController>().Selected();
        }
    }
    void Move()
    {

    }
    void Split()
    {

    }
    void Spit()
    {

    }
    void Rush()
    {

    }
    void Leap()
    {

    }
}
