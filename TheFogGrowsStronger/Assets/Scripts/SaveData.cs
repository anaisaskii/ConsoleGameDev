using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int level;
    public float health;
    public Vector3 position;
}

public class SaveData : MonoBehaviour
{
    private string filePath;

    void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "playerdata.json");
    }

    public void WriteData(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, true); // true = pretty print
        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log("Data saved to: " + filePath);
        }
        catch (IOException ioEx)
        {
            Debug.LogError("Error saving file: " + ioEx.Message);
            // Show UI message
        }
    }

    public PlayerData LoadData(string filepath)
    {
        if (File.Exists(filepath))
        {
            string json = File.ReadAllText(filepath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("Data loaded from: " + filepath);
            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found!");
            return null;
        }
    }
}
