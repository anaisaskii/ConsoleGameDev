using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    private EventInstance currentMusicInstance;
    private MusicArea currentArea = MusicArea.Plains;

    //Play music when the player spawns in
    void Start()
    {
        PlayMusicForArea(currentArea);
    }

    // set the area from the BGMTrigger script
    public void SetArea(MusicArea newArea)
    {
        if (newArea == currentArea) return; //ignore if it's the same

        currentArea = newArea;
        SwitchMusic(newArea);
    }

    private void SwitchMusic(MusicArea area)
    {
        // Stop the current music
        currentMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        currentMusicInstance.release();

        
        PlayMusicForArea(area);
    }

    //create an instance and start music
    private void PlayMusicForArea(MusicArea area)
    {
        string eventPath = GetMusicEventPath(area);
        currentMusicInstance = RuntimeManager.CreateInstance(eventPath);
        currentMusicInstance.start();
    }

    // BGM paths, default is forest
    private string GetMusicEventPath(MusicArea area)
    {
        switch (area)
        {
            case MusicArea.Forest: return "event:/BGM/BGM_Forest";
            case MusicArea.Plains: return "event:/BGM/BGM_Fog";
            default: return "event:/BGM/BGM_Forest";
        }
    }

    //cleanup
    private void OnDestroy()
    {
        currentMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        currentMusicInstance.release();
    }
}