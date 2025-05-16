using System.Collections;
using System.Collections.Generic;
using BTAI;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LanguageButton : MonoBehaviour
{
    private bool active = false; //on/off switch

    public GameObject LanguagePrompt;
    public GameObject MainMenu;

    public void ChangeLocale(int localeID)
    {
        if (active == true) //make sure it is only called once
            return;
        
        StartCoroutine(SetLocale(localeID));
    }

    IEnumerator SetLocale(int _localeID)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        active = false;
    }

    public void ConfirmLang()
    {
        LanguagePrompt.SetActive(false);
        MainMenu.SetActive(true);
    }
 }