using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    //this is unrelated but i dont wanna make a new script just for this one piece of logic
    public void backtomainmenu()
    {

        SceneManager.LoadScene(0);
    }
}
