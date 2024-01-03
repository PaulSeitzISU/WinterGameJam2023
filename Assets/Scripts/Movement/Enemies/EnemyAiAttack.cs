using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAiAttack : MonoBehaviour
{
    EnemyBrain enemyBrain;
    Projectile projectile;
    public int damage = 10;

    void Start()
    {
        enemyBrain = GetComponent<EnemyBrain>();
        projectile = GetComponent<Projectile>();
        
    }
    public void SwingAll()
    {
        if(projectile != null)
        {
            foreach(GameObject player in enemyBrain.PlayerList)
            {
                projectile.FireProjectile(player, damage);
                //Debug.Log("Shoot");
            }
        }
    }

    public void Swing()
    {
        if(projectile != null)
        {
            projectile.FireProjectile(enemyBrain.ClosestPlayer(), damage);
            //Debug.Log("Shoot");
        }
        else
        {
            enemyBrain.ClosestPlayer().GetComponent<Health>().TakeDamageAndCheckIfDead(damage);
        }
    }


}
