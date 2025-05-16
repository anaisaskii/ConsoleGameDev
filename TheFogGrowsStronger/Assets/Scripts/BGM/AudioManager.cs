using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    private EventInstance currentMusicInstance;
    private MusicArea currentArea = MusicArea.Plains;

    void Start()
    {
        PlayMusicForArea(currentArea);
    }

    public void SetArea(MusicArea newArea)
    {
        if (newArea == currentArea) return;

        currentArea = newArea;
        SwitchMusic(newArea);
    }

    private void SwitchMusic(MusicArea area)
    {
        // Stop the current music
        currentMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        currentMusicInstance.release();

        // Start new music
        PlayMusicForArea(area);
    }

    private void PlayMusicForArea(MusicArea area)
    {
        string eventPath = GetMusicEventPath(area);
        currentMusicInstance = RuntimeManager.CreateInstance(eventPath);
        currentMusicInstance.start();
    }

    private string GetMusicEventPath(MusicArea area)
    {
        switch (area)
        {
            case MusicArea.Forest: return "event:/BGM/BGM_Forest";
            case MusicArea.Plains: return "event:/BGM/BGM_Fog";
            default: return "event:/BGM/BGM_Forest";
        }
    }

    private void OnDestroy()
    {
        currentMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        currentMusicInstance.release();
    }
}