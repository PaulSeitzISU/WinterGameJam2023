using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    [SerializeField] private UnityEvent AnimationEvent;


    // This method initializes the health to its maximum value
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Method to take damage
    public bool TakeDamageAndCheckIfDead(int damageAmount)
    {
        currentHealth -= damageAmount;
        AnimationEvent.Invoke();

        if (currentHealth <= 0)
        {
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
