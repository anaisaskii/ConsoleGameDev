using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsSwitcher : MonoBehaviour
{
    public GameObject VisualSettings;
    public GameObject VolumeSettings;
    public GameObject AccessibilitySettings;
    public GameObject SelectSettingsPage;

    public GameObject UI_FirstButtonVisual;
    public GameObject UI_FirstButtonVolume;
    public GameObject UI_FirstButtonAccessibility;
    public GameObject UI_FirstButtonSelectSettings;

    // Call this from UI buttons
    public void ShowVisualSettings()
    {
        SetActivePage(VisualSettings);
        VolumeSettings.SetActive(false);
        AccessibilitySettings.SetActive(false);
        SelectSettingsPage.SetActive(false);
        UI_FirstButtonVisual.GetComponent<TMP_Dropdown>().Select();
    }

    public void ShowVolumeSettings()
    {
        SetActivePage(VolumeSettings);
        VisualSettings.SetActive(false);
        AccessibilitySettings.SetActive(false);
        SelectSettingsPage.SetActive(false);
        UI_FirstButtonVolume.GetComponent<Slider>().Select();
    }

    public void ShowAccessibilitySettings()
    {
        SetActivePage(AccessibilitySettings);
        VisualSettings.SetActive(false);
        VolumeSettings.SetActive(false);
        SelectSettingsPage.SetActive(false);
        UI_FirstButtonAccessibility.GetComponent<Button>().Select();
    }

    public void BackToSelectPage()
    {
        VisualSettings.SetActive(false);
        VolumeSettings.SetActive(false);
        AccessibilitySettings.SetActive(false);
        SelectSettingsPage.SetActive(true);
        UI_FirstButtonSelectSettings.GetComponent<Button>().Select();
    }

    private void SetActivePage(GameObject activePage)
    {
        activePage.SetActive(true);
    }
}
