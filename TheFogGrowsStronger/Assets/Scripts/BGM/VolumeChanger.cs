using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.UI;

public class VolumeChanger : MonoBehaviour
{
    public GameObject MainVolSlide;
    //public GameObject SFXVolSlide;
    //public GameObject BGMVolSlide;

    private Bus masterBus;
    private Bus sfxBus;
    private Bus BGMBus;

    void Awake()
    {
        masterBus = RuntimeManager.GetBus("bus:/");

        //sfxBus = RuntimeManager.GetBus("bus:/SFX Group");

        //BGMBus = RuntimeManager.GetBus("bus:/BGM group");

    }

    void Update()
    {
        masterBus.setVolume(MainVolSlide.GetComponent<Slider>().value); //change to be slider value

        //sfxBus.setVolume(SFXVolSlide.GetComponent<Slider>().value); //change to be slider value

        //BGMBus.setVolume(VFXVolSlide.GetComponent<Slider>().value); //change to be slider value
    }
}
