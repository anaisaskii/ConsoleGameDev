using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public string itemName;

    //edit these in inspector for item then make it a prefab or something
    public float powerScaling = 1.0f; //strength
    public float speedScaling = 1.0f; //speed

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
