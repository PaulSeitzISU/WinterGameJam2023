using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    public GameObject[] PlayerList;
    public GameObject[] EnemyList;
    TilemapScanner tilemapScanner;

    private bool finishedTurn = false;
    public bool isTurn = false;

    // Time delay between enemy turns
    public float turnDelay = 1.0f; // Adjust this value as needed
    public float maxTimeStart = 2.0f;

    public float maxTime = 2.0f;
    public float currentTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        //get components

        //find all enemies
        EnemyList = GameObject.FindGameObjectsWithTag("Enemy");
        tilemapScanner = GameObject.Find("Tilemap").GetComponent<TilemapScanner>();
    }

    // Update is called once per frame
    void Update()
    {
        
        {

        }
        
        maxTime = maxTimeStart + EnemyList.Length ;
        //take turn on q
        
        if(isTurn && !finishedTurn)
        {
            isTurn = false;
            StartCoroutine(ExecuteEnemyTurns());
        }

        //fail safe
        if (isTurn && currentTime > maxTime)
        {
            currentTime = 0.0f;
            isTurn = false;
        } else if (isTurn)
        {
            currentTime += Time.deltaTime;
        }

    }
    void FixedUpdate()
    {
        if(EnemyList.Length == 0)
        {
            SceneManager.LoadScene("WinScene");
        }
    }

    IEnumerator ExecuteEnemyTurns()
    {
        finishedTurn = true;


        foreach (GameObject enemy in EnemyList)
        {
            tilemapScanner.ScanTilemap();
            
            //see if enemy gameobject is null if so remove from list

            if (enemy == null)
            {
                EnemyList = GameObject.FindGameObjectsWithTag("Enemy");
                continue;
            }




            EnemyBrain enemyBrain = enemy.GetComponent<EnemyBrain>();

            if (enemyBrain.currentState != EnemyState.Dead)
            {
                enemyBrain.TakeTurn();
                yield return new WaitForSeconds(turnDelay); // Introduce delay between enemy turns
            }
        }

        finishedTurn = false;
    }
}
