using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    // This method initializes the health to its maximum value
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Method to take damage
    public bool TakeDamageAndCheckIfDead(int damageAmount)
    {
        currentHealth -= damageAmount;

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
