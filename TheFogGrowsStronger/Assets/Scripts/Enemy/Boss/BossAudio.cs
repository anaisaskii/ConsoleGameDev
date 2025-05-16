using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class BossAudio : MonoBehaviour
{
    public void PlayFootstep()
    {
        EventInstance instance = RuntimeManager.CreateInstance("event:/Enemy/Golem_Footstep");
        RuntimeManager.AttachInstanceToGameObject(instance, transform, GetComponent<Rigidbody>());

        instance.start();
        instance.release();
    }
}
