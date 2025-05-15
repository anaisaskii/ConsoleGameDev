using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SettingsMenuData
{
    public SettingsMenuData()
    {
        resolutionX = 1920;
        resolutionY = 1080;
        vsync = 0;
        volume = 50f;
        language = 0;
    }

    public int resolutionX;
    public int resolutionY;
    public int vsync;
    public float volume;
    public int language;

}

public class SaveData : MonoBehaviour
{
    public void Awake()
    {
        PlayerPreferences.Start();
        DontDestroyOnLoad(this.gameObject);
    }
    public void WriteData(SettingsMenuData data)
    {
        PlayerPrefs.SetInt("ResX", data.resolutionX);
        PlayerPrefs.SetInt("ResY", data.resolutionY);
        PlayerPrefs.SetInt("Vsync", data.vsync);
        PlayerPrefs.SetInt("Language", data.language);
        PlayerPrefs.SetFloat("Volume", data.volume);

        PlayerPrefs.Save();

        UnityEngine.PS4.PS4PlayerPrefs.SaveToByteArray();
    }

    public void LoadData()
    {
        PlayerPrefs.GetInt("ResX");
        PlayerPrefs.GetInt("ResY");
        PlayerPrefs.GetInt("Vsync");
        PlayerPrefs.GetInt("Language");
        PlayerPrefs.GetFloat("Volume");
    }

    
}
