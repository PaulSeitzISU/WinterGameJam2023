using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerInputManager playerManager;
    public EnemyManager enemyManager;
    public bool isPlayerTurn = true;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(!enemyManager.isTurn && !playerManager.isTurn && isPlayerTurn)
        {
            playerManager.UpdatePlayerList();
            isPlayerTurn = false;
            playerManager.isTurn = true;
            Debug.Log("Player turn");
        }

        if(!playerManager.isTurn && !enemyManager.isTurn && !isPlayerTurn)
        {
            isPlayerTurn = true;
            enemyManager.isTurn = true;
            Debug.Log("Enemy turn");
        }

        
    }
}
