using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PS4;

public static class PlayerPreferences 
{
    static bool SAVED = false;

    // Start is called before the first frame update
    public static void Start()
    {
        #if UNITY_PS4
                if (!SAVED)
                {
                    UnityEngine.PS4.PS4PlayerPrefs.SetTitleStrings("TheFogGrowsStronger", "SaveData", "Data to be saved");
                    SAVED = true;
                }
        #endif
    }
}
