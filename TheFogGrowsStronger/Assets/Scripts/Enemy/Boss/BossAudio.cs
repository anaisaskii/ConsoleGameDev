using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

// Audio to play boss footstepd
// The function is called by their footstep animation event

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
