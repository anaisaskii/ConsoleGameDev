using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public enum CharacterType
{
    Player,
    CommonEnemy1,
    CommonEnemy2,
    Boss1
}

public class Health : MonoBehaviour
{
    public CharacterType characterType;

    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float currentHealth;

    public UnityEvent OnDeath;
    public UnityEvent<float> OnDamage;

    public GameObject damageNumbers;



    //when you choose a character type in the inspector it'll set the health automatically
    private void OnValidate()
    {
        switch (characterType)
        {
            case CharacterType.Player:
                maxHealth = 100;
                break;
            case CharacterType.CommonEnemy1:
                maxHealth = 25;
                break;
            case CharacterType.CommonEnemy2:
                maxHealth = 50;
                break;
            case CharacterType.Boss1:
                maxHealth = 100;
                break;
        }

        currentHealth = maxHealth;
        
    }

    // Applies damage to character
    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        OnDamage?.Invoke(damage);

        if (damageNumbers)
        {
            ShowDamageNumbers(damage);
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

    }

    public virtual void Die()
    {
    }

    //Show the damage numbers
    void ShowDamageNumbers(float damage)
    {
        var number = Instantiate(damageNumbers, transform.position, Quaternion.identity, transform);
        number.GetComponent<TMP_Text>().text = damage.ToString();
    }

    // Get the current health
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }
}