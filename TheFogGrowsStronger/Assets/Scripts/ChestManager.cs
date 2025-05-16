using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public class ChestManager : MonoBehaviour
{
    public List<Transform> chestLocations;

    public GameObject chestPrefab;

    public GameObject player;

    public List<GameObject> itemPrefab;

    //spawn in a set number of random chests
    //place chests


    void Start()
    {
        //chooses 10 of the locations at random
        List<Transform> regularChests = chestLocations.OrderBy(x => Random.value).Take(6).ToList();

        foreach (Transform t in regularChests)
        {
            Instantiate(chestPrefab, t);
        }


    }

    // Update is called once per frame
    void Update()
    {
        //if user presses top button and it hits a chest AND they ahve enough money, open it and spawn a random item
        if (Gamepad.current != null && Gamepad.current.buttonNorth.wasPressedThisFrame)
        {
            if (player.GetComponent<PlayerController>().cash >= 0)
            {
                RaycastHit hit;
                Vector3 rayOrigin = player.transform.position + Vector3.up * 0.5f;
                if (Physics.Raycast(rayOrigin, player.transform.forward, out hit, 10f))
                {

                    if (hit.collider.CompareTag("Chest"))
                    {
                        Debug.Log("Hit chest by tag!");
                        Animator animator = hit.collider.GetComponent<Animator>();
                        animator.SetTrigger("OpenChest");
                        SpawnItem(hit.collider.transform.position);
                    }
                }
            }
        }
    }

    void SpawnItem(Vector3 spawnPosition)
    {
        GameObject spawnedItem = Instantiate(itemPrefab[Random.Range(0, 4)], spawnPosition + Vector3.up * 2f, Quaternion.identity);

        Rigidbody rb = spawnedItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 launchDirection = Vector3.up + Random.insideUnitSphere * 0.5f;
            rb.AddForce(launchDirection * 5f, ForceMode.Impulse);
        }
    }
}
