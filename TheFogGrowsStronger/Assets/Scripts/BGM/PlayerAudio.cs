using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField]
    private string FootstepEvent = "event:/Footsteps/Footsteps";
    private EventInstance footstepInstance;

    [SerializeField]
    private LayerMask groundLayer;

    //Called by animation event on footstep animation
    public void PlayFootstep()
    {
        footstepInstance = RuntimeManager.CreateInstance(FootstepEvent);

        FootstepSurfaces currentSurface = DetectGroundSurface();

        //set variable in fmod
        //set parameter by name for local parameters
        footstepInstance.setParameterByName("Ground Type", (float)SurfaceToParameterValue(currentSurface));

        //start the sound effect and release the memory
        footstepInstance.start();
        footstepInstance.release();

    }

    //shoot raycast to check ground type
    private FootstepSurfaces DetectGroundSurface()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, 1.5f, groundLayer))
        {
            Debug.DrawRay(transform.position, Vector3.down, Color.blue, 2.0f);

            string tag = hit.collider.tag;

            if (tag == "Gravel") return FootstepSurfaces.Gravel;
            if (tag == "Grass") return FootstepSurfaces.Grass;
            if (tag == "Stone") return FootstepSurfaces.Stone;
        }
        // Default fallback
        return FootstepSurfaces.Grass;
    }

    //converts the surface value to corresponding fmod parameter value
    private int SurfaceToParameterValue(FootstepSurfaces surface)
    {
        switch (surface)
        {
            case FootstepSurfaces.Grass: return 0;
            case FootstepSurfaces.Stone: return 1;
            case FootstepSurfaces.Gravel: return 2;
            default: return 0;
        }
    }
}