using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject selectionRing;
    bool selected;
    [SerializeField] bool isPlayerlet;
    [SerializeField] int startingSlime;
    [SerializeField] int totalSlime;
    // Start is called before the first frame update
    void Start()
    {
        SetSlime(startingSlime);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void SetSlime(int slimeNow)
    {
        totalSlime = slimeNow;
    }
    public bool isPlayer()
    {
        return !isPlayerlet;
    }
    public void Selected()
    {
        selectionRing.SetActive(true);
    }
    public void DeSelection()
    {
        selectionRing.SetActive(false);
    }
    public void Move(Vector3Int gridPosition)
    {
        Debug.Log("Moving!");
        GetComponent<Movement>().MoveToGrid(gridPosition);
    }
    public void Split()
    {
        Debug.Log("Splitting!");
    }
    public void Spit()
    {
        Debug.Log("Spitting!");
    }
    public void Rush()
    {
        Debug.Log("Rushing!");
    }
    public void Leap()
    {
        Debug.Log("Leaping!");
    }
}
