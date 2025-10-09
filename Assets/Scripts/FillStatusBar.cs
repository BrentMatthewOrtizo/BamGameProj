using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FillStatusBar : MonoBehaviour
{
    private Health characterHealth;

    public Image fillBackground; //if we want to change background of health bar

    private Slider slider;
    
    public void SetCharacter(GameObject newCharacter)
    {
        characterHealth = newCharacter.GetComponent<Health>();
    }
    
    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (characterHealth != null)
        {
            //health bar logic
            float fillValue = characterHealth.currentHealth / characterHealth.maxHealth;
            slider.value = fillValue;

            if (slider.value <= slider.minValue)
            {
                slider.value = slider.minValue;
            }
        }
            
    }
}
