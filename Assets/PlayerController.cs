using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject selectionRing;
    [SerializeField] GameObject playerlet;

    Health health;
    //[SerializeField] GameObject projectile;
    bool selected;

    [SerializeField] bool isPlayerlet;
    [SerializeField] int startingSlime;
    [SerializeField] int totalSlime;
    [SerializeField] int damage;
    [SerializeField] int splitCost;  


    Projectile projectile;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
        health.maxHealth = startingSlime;
        health.currentHealth = startingSlime;
        totalSlime = startingSlime;
        projectile = GetComponent<Projectile>();
    }

    // Update is called once per frame
    void Update()
    {
        totalSlime  = health.currentHealth;
    }

    void FixedUpdate()
    {
        if (health.currentHealth <= 0)
        {
            //go to lose screen
            SceneManager.LoadScene("LoseScene");
        }
    }

    public void AddSlime(int slimeNow)
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
        if(health.currentHealth > splitCost)
        {
            health.currentHealth -= splitCost;
        }
        else
        {
            return;
        }
        
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
    public void Rush(GameObject target)
    {
        projectile.FireProjectile(target, damage * 2);

        Debug.Log("Rushing!");
    }
    public void Leap()
    {
        Debug.Log("Leaping!");
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Health Pickup"))
        {
            health.Heal(health.maxHealth / 3);
            Destroy(collider.gameObject);
        }
    }
}
