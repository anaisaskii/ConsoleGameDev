using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//spawn in player and set up game
public class GameManager : MonoBehaviour
{
    public GameObject runner;
    public GameObject hunter;

    public GameObject playerParent;

    public SaveData savedata;
    private string filePath;

    PlayerData newData = new PlayerData
    {
        playerName = "PlayerOne",
        level = 5,
        health = 72.5f,
        position = new Vector3(1f, 2f)
    };

    // Start is called before the first frame update
    void Start()
    {
        switch (CharacterSelect.selectedCharacterIndex)
        {
            case 1:
                Instantiate(runner, playerParent.transform);
                break;
            case 2:
                Instantiate(hunter, playerParent.transform);
                break;
            case 0: //fallback
                Instantiate(runner, playerParent.transform);
                break;
        }

        //Set attributes based on save data
        filePath = Path.Combine(Application.persistentDataPath, "playerdata.json");
        PlayerData data = savedata.LoadData(filePath);
        Debug.Log(data.health);
    }

    public void SaveDataToJSON()
    {
        savedata.WriteData(newData);
        Debug.Log("Written Data!");
    }

}
