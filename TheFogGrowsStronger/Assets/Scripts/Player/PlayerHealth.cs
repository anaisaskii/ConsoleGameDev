using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    [Header("UI")]
    [SerializeField] private Image healthBarFill;

    private void Start()
    {
        UpdateHealthBar();
    }

    public override void TakeDamage(float damage)
    {
        
        UpdateHealthBar();       //update ui
        base.TakeDamage(damage); //call base logic

        Debug.Log("current health: " + currentHealth);
    }

    public override void Die()
    {
        base.Die(); 
        //Player-specific death logic
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = GetCurrentHealth() / GetMaxHealth();
        }
    }
}