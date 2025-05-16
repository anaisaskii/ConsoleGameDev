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

    public SaveData savedata;
    private string filePath;

    private bool isPaused = false;
    public GameObject pauseMenu;


    void Start()
    {
        //Check selected character from character select screen
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

    //Pause/Unpause game
    //Set game time to 0, pauses AI and movement, keeps sound playing

    void PauseGame()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true); 
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void UnpauseGame()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

}
