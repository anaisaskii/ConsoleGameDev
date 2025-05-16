using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.UI;

public class VolumeChanger : MonoBehaviour
{
    public GameObject MainVolSlide;

    private Bus masterBus;

    // gets the main FMOD bus (controls all volume)
    void Awake()
    {
        masterBus = RuntimeManager.GetBus("bus:/");

    }

    // sets the bus volume to be the slider value
    void Update()
    {
        masterBus.setVolume(MainVolSlide.GetComponent<Slider>().value); //change to be slider value
    }
}
