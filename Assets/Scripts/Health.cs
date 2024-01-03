using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    [SerializeField] private UnityEvent AnimationEvent;
    [SerializeField] private UnityEvent AnimationEventDead;


    EnemyBrain enemyBrain;


    // This method initializes the health to its maximum value
    void Start()
    {
        enemyBrain = GetComponent<EnemyBrain>();
        currentHealth = maxHealth;
    }

    // Method to take damage
    public bool TakeDamageAndCheckIfDead(int damageAmount)
    {
        currentHealth -= damageAmount;
        AnimationEvent.Invoke();
        return CheckIfDead();
    }

    //check if dead
    public bool CheckIfDead()
    {
        if (currentHealth <= 0)
        {
            if(enemyBrain != null)
            {
                enemyBrain.currentState = EnemyState.Dead;
                enemyBrain.UpdateState();
                AnimationEventDead.Invoke();
            }
            return true;
        }
        return false;
    }

    // Method to heal
    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
    }


}
