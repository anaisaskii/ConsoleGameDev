using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using JetBrains.Annotations;
using Unity.VisualScripting;

public class ResolutionManager : MonoBehaviour
{
    public TMP_Dropdown ResolutionList;
    public Toggle FullScreenToggle;
    public Toggle VSyncToggle;
    Resolution[] AllRes; //list of all resolutions

    bool isFullscreen = true;
    int selectedRes;
    List<Resolution> SelectedResList = new List<Resolution>();

    void Start()
    {
        isFullscreen = true;
        Debug.Log("Dropdown assigned? " + (ResolutionList != null));

        AllRes = Screen.resolutions;
        Debug.Log("Found " + AllRes.Length + " resolutions");

        List<string> resList = new List<string>();

        string newRes; //new resolution container

        foreach (Resolution res in AllRes)
        {
            newRes = res.width.ToString() + " x " + res.height.ToString();
            if (!resList.Contains(newRes))
            {
                resList.Add(newRes);
                SelectedResList.Add(res);
            }
            /*resList.Add(res.ToString());*/
        }

        ResolutionList.ClearOptions(); //clear options before adding

        ResolutionList.AddOptions(resList); //add screen res options to dropdown list
        //added options

    }
    public void ChangeRes()
    {
        selectedRes = ResolutionList.value;
        Screen.SetResolution(SelectedResList[selectedRes].width, SelectedResList[selectedRes].height, isFullscreen);

        Debug.Log("changed resolution!");
    }
    public void ChangeFullscreen()
    {
        isFullscreen = FullScreenToggle.isOn;
        Screen.SetResolution(SelectedResList[selectedRes].width, SelectedResList[selectedRes].height, isFullscreen);
    }

    public void SetVSync(bool isOn)
    {
        QualitySettings.vSyncCount = isOn ? 1 : 0;
        Debug.Log("VSync is now " + (isOn ? "ON" : "OFF"));
    }

}
