using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

//spawn in player and set up game
public class GameManager : MonoBehaviour
{
    public GameObject runner;
    public GameObject hunter;

    public GameObject playerParent;

    public Button saveGameButton;

    public SaveData savedata;
    private string filePath;

    private bool isPaused = false;
    public GameObject pauseMenu;


    // Start is called before the first frame update
    void Start()
    {
        switch (CharacterSelect.selectedCharacterIndex)
        {
            case 1:
                Instantiate(runner, playerParent.transform);
                playerParent.GetComponent<PlayerController>().enabled = false;
                playerParent.GetComponent<PlayerControllerRunner>().enabled = true;
                break;
            case 2:
                Instantiate(hunter, playerParent.transform);
                playerParent.GetComponent<PlayerController>().enabled = true;
                playerParent.GetComponent<PlayerControllerRunner>().enabled = false;
                break;
            case 0: //fallback
                Instantiate(hunter, playerParent.transform);
                playerParent.GetComponent<PlayerController>().enabled = true;
                playerParent.GetComponent<PlayerControllerRunner>().enabled = false;
                break;
        }

        saveGameButton.onClick.AddListener(SaveAndExitGame);

        UnpauseGame();

    }

    public void SaveAndExitGame()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton9)) 
        {
            if (isPaused)
                UnpauseGame();
            else
                PauseGame();
        }
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
