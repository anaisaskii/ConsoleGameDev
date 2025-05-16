using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumbers : MonoBehaviour
{
    public float DestroyTime = 5f;
    public Vector3 offset = new Vector3(0, 10, 0);
    public Vector3 randomizepos = new Vector3(1, 0, 0);

    private Camera mainCamera;

    void Start()
    {
        Destroy(gameObject, DestroyTime);
        transform.position += offset;
        transform.position += new Vector3(
            Random.Range(-randomizepos.x, randomizepos.x),
            Random.Range(-randomizepos.y, randomizepos.y),
            Random.Range(-randomizepos.z, randomizepos.z)
        );

        mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera != null)
        {
            transform.LookAt(mainCamera.transform);
            transform.forward = -transform.forward;
        }
    }
}
