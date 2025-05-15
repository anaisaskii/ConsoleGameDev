using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsSwitcher : MonoBehaviour
{
    public GameObject VisualSettings;
    public GameObject VolumeSettings;
    public GameObject AccessibilitySettings;
    public GameObject SelectSettingsPage;

    // Call this from UI buttons
    public void ShowVisualSettings()
    {
        SetActivePage(VisualSettings);
        VolumeSettings.SetActive(false);
        AccessibilitySettings.SetActive(false);
        SelectSettingsPage.SetActive(false);

    }

    public void ShowVolumeSettings()
    {
        SetActivePage(VolumeSettings);
        VisualSettings.SetActive(false);
        AccessibilitySettings.SetActive(false);
        SelectSettingsPage.SetActive(false);
    }

    public void ShowAccessibilitySettings()
    {
        SetActivePage(AccessibilitySettings);
        VisualSettings.SetActive(false);
        VolumeSettings.SetActive(false);
        SelectSettingsPage.SetActive(false);
    }

    public void BackToSelectPage()
    {
        VisualSettings.SetActive(false);
        VolumeSettings.SetActive(false);
        AccessibilitySettings.SetActive(false);
        SelectSettingsPage.SetActive(true);
    }

    private void SetActivePage(GameObject activePage)
    {
        activePage.SetActive(true);
    }
}
