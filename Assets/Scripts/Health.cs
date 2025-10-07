using UnityEngine;

//attach script to character object
public class Health : MonoBehaviour
{
    public float  maxHealth;
    public float currentHealth;
    
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

    //put this is food script (food heals monsters by x amount)
    void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
    
    //put this in script for items that could upgrade your monster's max health
    void UpgradeMaxHealth(int amount)
    {
        maxHealth += amount;
    }
}
