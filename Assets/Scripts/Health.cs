using UnityEngine;

//attach script to monster Object
public class Health : MonoBehaviour
{
    public int  maxHealth;
    public int currentHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHealth = 3;
        currentHealth = maxHealth;
    }

    void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            //call function or animation that switches to dead monster sprite
        }
    }

    void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
    
    void UpgradeMaxHealth(int amount)
    {
        maxHealth += amount;
    }
}
