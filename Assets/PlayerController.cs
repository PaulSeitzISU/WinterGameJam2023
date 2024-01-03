using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject selectionRing;
    [SerializeField] GameObject playerlet;
    //[SerializeField] GameObject projectile;
    bool selected;

    [SerializeField] bool isPlayerlet;
    [SerializeField] int startingSlime;
    [SerializeField] int totalSlime;
    [SerializeField] int damage;


    Projectile projectile;

    // Start is called before the first frame update
    void Start()
    {
        SetSlime(startingSlime);
        projectile = GetComponent<Projectile>();
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
    public void Split(bool horizontal)
    {
        Debug.Log("Splitting!");
        if (horizontal)
        {
            Instantiate(playerlet, transform.position + Vector3.left, Quaternion.identity);

            Instantiate(playerlet, transform.position + Vector3.right, Quaternion.identity);
        }else if (!horizontal)
        {
            Instantiate(playerlet, transform.position + Vector3.up, Quaternion.identity);

            Instantiate(playerlet, transform.position + Vector3.down, Quaternion.identity);
        }
    }
    public void Spit(GameObject target)
    {
        Debug.Log("Spitting!");
        projectile.FireProjectile(target, damage);
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
