using UnityEngine;
using UnityEngine.UI;

public class FillStatusBar : MonoBehaviour
{
    public Health characterHealth;
    public Image healthBar;

    private Slider slider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        float fillValue = characterHealth.currentHealth / characterHealth.maxHealth;
        slider.value = fillValue;
    }
}
