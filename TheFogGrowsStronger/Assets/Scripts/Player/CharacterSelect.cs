using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    private GameObject runner;
    private GameObject hunter;

    public Button selectButton;

    public static int selectedCharacterIndex;

    // Start is called before the first frame update
    void Start()
    {
        //get characters in scene
        runner = GameObject.Find("Runner");
        hunter = GameObject.Find("huntress_mesh");

        HideCharacters();
    }

    //Set the character
    public void SetCharacterRunner()
    {
        selectedCharacterIndex = 1;
        HideCharacters();
        runner.GetComponent<SkinnedMeshRenderer>().enabled = true;
    }

    public void SetCharacterHunter()
    {
        selectedCharacterIndex = 2;
        HideCharacters();
        hunter.GetComponent<SkinnedMeshRenderer>().enabled = true;
    }

    //Hide all
    private void HideCharacters()
    {
        runner.GetComponent<SkinnedMeshRenderer>().enabled = false;
        hunter.GetComponent<SkinnedMeshRenderer>().enabled = false;
    }
}
