using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject[] PlayerList;
    public GameObject[] EnemyList;


    // Start is called before the first frame update
    void Start()
    {
        //get components

        //find all enemies
        EnemyList = GameObject.FindGameObjectsWithTag("Enemy");

    }

    // Update is called once per frame
    void Update()
    {
        //take turn on q
        if (Input.GetKeyDown(KeyCode.Q))
        {
            EnemyTurn();
        }
    }

    public void EnemyTurn()
    {
        //for each enemy
        foreach (GameObject enemy in EnemyList)
        {
            //get enemy script
            EnemyBrain enemyBrain = enemy.GetComponent<EnemyBrain>();
            //if enemy is alive
            if (enemyBrain.currentState != EnemyState.Dead)
            {
                //do enemy turn
                enemyBrain.TakeTurn();
            }
        }
    }
}
