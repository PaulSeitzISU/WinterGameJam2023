using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject[] PlayerList;
    public GameObject[] EnemyList;
    TilemapScanner tilemapScanner;

    private bool finishedTurn = false;
    public bool isTurn = false;

    // Time delay between enemy turns
    public float turnDelay = 1.0f; // Adjust this value as needed

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
        //take turn on q
        if (Input.GetKeyDown(KeyCode.Q) && !finishedTurn)
        {
            StartCoroutine(ExecuteEnemyTurns());
        }
        if(isTurn && !finishedTurn)
        {
            isTurn = false;
            StartCoroutine(ExecuteEnemyTurns());
        }
    }

    IEnumerator ExecuteEnemyTurns()
    {
        finishedTurn = true;


        foreach (GameObject enemy in EnemyList)
        {
            tilemapScanner.ScanTilemap();


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
