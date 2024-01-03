using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAiAttack : MonoBehaviour
{
    EnemyBrain enemyBrain;
    public int damage = 10;

    void Start()
    {
        enemyBrain = GetComponentInParent<EnemyBrain>();
    }

    public void Swing()
    {
        enemyBrain.ClosestPlayer().GetComponent<Health>().TakeDamageAndCheckIfDead(damage);
    }
}
