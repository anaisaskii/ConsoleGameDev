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

    void ShowDamageNumbers(float damage)
    {
        var number = Instantiate(damageNumbers, transform.position, Quaternion.identity, transform);
        number.GetComponent<TMP_Text>().text = damage.ToString();
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }
    /*void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        
        *//*Vector3 scale = healthBarFill.rectTransform.localScale;
        scale.x = currentHealth / maxHealth;
        healthBarFill.rectTransform.localScale = scale;*//*
    }
    }*/
}