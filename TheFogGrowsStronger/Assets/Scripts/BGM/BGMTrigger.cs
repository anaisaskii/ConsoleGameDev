using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMTrigger : MonoBehaviour
{
    public MusicArea area;

    public AudioManager audioManager;

    //sets music when player enters box collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioManager.SetArea(area);
        }
    }
}
