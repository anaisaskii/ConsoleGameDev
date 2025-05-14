using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private AsyncOperation asyncLoad;

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(LoadSceneInBackground("MainMapScene"));
    }

    public void MainMapScene()
    {
        asyncLoad.allowSceneActivation = true;
    }

    IEnumerator LoadSceneInBackground(string sceneName)
    {
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            //Debug.Log("Loading progress: " + (asyncLoad.progress * 100) + "%");
            yield return null;
        }
    }
}
