using UnityEngine;
using UnityEngine.Events;

//select an enemy type and stuff is done automatically wow
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

    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    public GameObject damageNumbers;

    public UnityEvent OnDeath;
    public UnityEvent<float> OnDamage;

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

        if (damageNumbers != null)
        {
            showDamageNumbers();
        }
        else
        {
            Debug.Log("damage numbers not being ran");
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void showDamageNumbers()
    {
        Instantiate(damageNumbers, transform.position, Quaternion.identity, transform);
        Debug.Log("damage numbers ran");
    }

    public virtual void Die()
    {
        //change this to destroy the prefab by default!!
        //then for the player we can have special conditions (will need a playerhealth script)
        GetComponent<MeshRenderer>().enabled = false;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }
}