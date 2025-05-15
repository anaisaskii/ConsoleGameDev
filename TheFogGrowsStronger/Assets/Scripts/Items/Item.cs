using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public string itemName;

    //edit these in inspector for item then make it a prefab or something
    public float powerScaling = 1.0f; //strength
    public float speedScaling = 1.0f; //speed

    public float totalDamage = 0;
    public float totalRunSpeed = 0;
    public float totalWalkSpeed = 0;
    public int item_gem = 0;
    public int item_mag = 0;
    public int item_syringe = 0;
    public int item_pill = 0;

    public virtual void ApplyEffect(GameObject player)
    {
        Debug.Log($"Applying {itemName} power-up!");
    }

    public virtual void Update()
    {
        this.transform.Rotate(0f, 2 * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Check if player touches it
        {
            ApplyEffect(other.gameObject);  // Apply the effect

            Destroy(transform.parent.gameObject);  // Remove the power-up after pickup
        }
    }
}
