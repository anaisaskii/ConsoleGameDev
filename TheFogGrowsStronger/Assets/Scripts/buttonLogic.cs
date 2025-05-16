using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class buttonLogic : MonoBehaviour
{
    public GameObject UI_MainMenu;     
    public GameObject UI_SettingsMenu;  
    public GameObject UI_SelectSettings;

    public CinemachineVirtualCamera mainMenuCam; 
    public CinemachineVirtualCamera settingsCam; 

    public EventSystem eventSystem;

    public GameObject UI_FirstButtonSettings;
    public GameObject UI_FirstButtonMainMenu;

    public void OpenSettingsMenu()
    {
        // Disable the Main Menu
        UI_MainMenu.SetActive(false);

        UI_FirstButtonSettings.GetComponent<Button>().Select();

        // Enable the Settings
        UI_SettingsMenu.SetActive(true);
        UI_SelectSettings.SetActive(true);

        Debug.Log("Opening settings menu");

        // Switch to the Settings cam
        settingsCam.gameObject.SetActive(true);
    }

    public void BackToMainMenu()
    {
        // Disable the Settings
        UI_SettingsMenu.SetActive(false);

        eventSystem.firstSelectedGameObject = UI_FirstButtonMainMenu;
        UI_FirstButtonMainMenu.GetComponent<Button>().Select();

        // Enable the Main
        UI_MainMenu.SetActive(true);

        // Switch to the Main menu
        settingsCam.gameObject.SetActive(false);
    }

    public void Playbutton()
    {
        SceneManager.LoadScene(2);
    }
    public void CharacterSelect()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitButton()
    {
        Application.Quit();
        
    }
}
