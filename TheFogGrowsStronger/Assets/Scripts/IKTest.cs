using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKTest : MonoBehaviour
{
    // Start is called before the first frame update
    public TwoBoneIKConstraint rightHandIK;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("IK Weight: " + rightHandIK.weight);
    }
}
