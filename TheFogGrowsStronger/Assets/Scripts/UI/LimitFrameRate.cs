using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Limits framerate to 60fps

public class LimitFrameRate : MonoBehaviour
{
    public int target = 60;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = target;
    }

    void Update()
    {
        if (Application.targetFrameRate != target)
            Application.targetFrameRate = target;
    }

    public void backtomainmenu()
    {

        SceneManager.LoadScene(0);
    }
}
