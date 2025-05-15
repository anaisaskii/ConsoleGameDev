using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;

//spawn in player and set up game
public class GameManager : MonoBehaviour
{
    public GameObject runner;
    public GameObject hunter;

    private GameObject currentPlayer;

    public GameObject playerParent;

    public Button saveGameButton;

    public SaveData savedata;

    public TextMeshProUGUI fpsText;

    private float deltaTime = 0.0f;

    private string filePath;

    private bool isPaused = false;
    public GameObject pauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        switch (CharacterSelect.selectedCharacterIndex)
        {
            case 1:
                currentPlayer = Instantiate(runner, playerParent.transform);
                break;
            case 2:
                currentPlayer = Instantiate(hunter, playerParent.transform);
                break;
            case 0: //fallback
                currentPlayer = Instantiate(hunter, playerParent.transform);
                break;
        }

        UnpauseGame();

        //Set attributes based on save data
        //filePath = Path.Combine(Application.persistentDataPath, "playerdata.json");
        //PlayerData data = savedata.LoadData(filePath);
        //Debug.Log(data.health);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            PlayerPrefs.SetInt("Vsync", 1);
            PlayerPrefs.Save();
            UnityEngine.PS4.PS4PlayerPrefs.SaveToByteArray();

            if (isPaused)
                UnpauseGame();
            else
                PauseGame();
        }

        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = PlayerPrefs.GetInt("Vsync").ToString();

    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true); 
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Unpauses the game
    void UnpauseGame()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

}
