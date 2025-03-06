using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 30f;
    public float damage = 10f;
    public float lifetime = 5f;
    public GameObject hitEffectPrefab;

    private Vector3 startPos;
    private bool hasHit = false;

    private void Start()
    {
        startPos = transform.position;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (!hasHit)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;

        hasHit = true;

        // Apply damage if the hit object has a Health component
        Health health = collision.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }

        // Spawn hit effect
        if (hitEffectPrefab != null)
        {
            ContactPoint contact = collision.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Instantiate(hitEffectPrefab, contact.point, rot);
        }

        // Destroy projectile
        Destroy(gameObject);
    }
}