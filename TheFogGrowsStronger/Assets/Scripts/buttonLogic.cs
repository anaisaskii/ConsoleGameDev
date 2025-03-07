using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class buttonLogic : MonoBehaviour
{
    public GameObject UI_MainMenu;      //(inspector) assign main menu ui
    public GameObject UI_SettingsMenu;  //(inspector) assign settings menu ui
    public CinemachineVirtualCamera mainMenuCam;  //(inspector) assign main menu cam
    public CinemachineVirtualCamera settingsCam;  //(inspector) assign settings menu ui

    public EventSystem eventSystem;

    public GameObject UI_FirstButtonSettings;
    public GameObject UI_FirstButtonMainMenu;

    public void OpenSettingsMenu()
    {
        // Disable the Main Menu UI
        UI_MainMenu.SetActive(false);

        eventSystem.firstSelectedGameObject = UI_FirstButtonSettings;

        // Enable the Settings Menu UI
        UI_SettingsMenu.SetActive(true);

        // Switch to the Settings camera
        /*mainMenuCam.Priority = 0;  // Lower priority to deactivate
        settingsCam.Priority = 1;  // Increase priority to activate*/
        settingsCam.gameObject.SetActive(true); //set active
    }

    public void BackToMainMenu()
    {
        // Disable the Settings Menu UI
        UI_SettingsMenu.SetActive(false);

        eventSystem.firstSelectedGameObject = UI_FirstButtonMainMenu;

        // Enable the Main Menu UI
        UI_MainMenu.SetActive(true);

        // Switch to the Main Menu camera
        settingsCam.gameObject.SetActive(false); //set active
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
