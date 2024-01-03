using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using TMPro;

public class GreyAbilityHotbar : MonoBehaviour
{
    PlayerInputManager playerManager;

    [SerializeField] public Array attacjAbilityList ;
    [SerializeField] public Array moveAbilityList ;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.Find("InputManager").GetComponent<PlayerInputManager>();
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if(playerManager.currentSelection != null && playerManager.trackTurn[playerManager.currentSelection] != null)
    //     {
    //         DicTurn dicTurn = playerManager.trackTurn[playerManager.currentSelection];
    //         if(dicTurn.hasAttacked)
    //         {
    //             foreach(GameObject ability in attacjAbilityList)
    //             {
    //                 //ability.GetComponent<Image>().color = Color.grey;
    //             }
    //         }
    //         else
    //         {
    //         }
    //     }
        
    // }
}
