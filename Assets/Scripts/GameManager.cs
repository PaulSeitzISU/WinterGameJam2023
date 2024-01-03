using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerInputManager playerManager;
    public EnemyManager enemyManager;
    public bool isPlayerTurn = false;
    public GameObject buttonEndTurn;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(!enemyManager.isTurn && !playerManager.isTurn && isPlayerTurn)
        {
            buttonEndTurn.SetActive(true);
            playerManager.UpdatePlayerList();
            isPlayerTurn = false;
            playerManager.StartTurn();
            playerManager.isTurn = true;
            Debug.Log("Player turn");
        }

        if(!playerManager.isTurn && !enemyManager.isTurn && !isPlayerTurn)
        {
            buttonEndTurn.SetActive(false);
            isPlayerTurn = true;
            enemyManager.isTurn = true;
            Debug.Log("Enemy turn");
        }
    }

    public void EndPlayerTurn()
    {
        playerManager.isTurn = false;
        
    }
}
