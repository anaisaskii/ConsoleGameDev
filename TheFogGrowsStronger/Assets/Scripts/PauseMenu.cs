using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject firstSelectedButton; //immediate select upon open

    private bool isPaused = false;

    void Update()
    {
        //check for OPTIONS button on ps4 controller
        if (Input.GetKeyDown(KeyCode.JoystickButton12) || Input.GetKeyDown(KeyCode.M))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; //pause the game time
            pauseMenu.SetActive(true);

            //set the selected button for controller navigation
            EventSystem.current.SetSelectedGameObject(null); // Reset selection
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
        else
        {
            Time.timeScale = 1f; // Resume game
            pauseMenu.SetActive(false);

            // Optional: clear selected UI element
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
    public void closepausemenu()
    {
        pauseMenu.SetActive(false);
    }
}