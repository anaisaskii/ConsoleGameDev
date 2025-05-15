using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    [Tooltip("Name or index of the scene to load after this one")]
    public string sceneToLoad = "MainMenuScene";

    [Tooltip("Reference to the TextMeshProUGUI component")]
    public TextMeshProUGUI loadingText;

    private void Start()
    {
        if (loadingText == null)
            Debug.LogError(" LoadingText not assigned!", this);

        StartCoroutine(LoadAsyncScene());
    }

    private IEnumerator LoadAsyncScene()
    {
        // Begin to load the Scene you specify
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        // Prevent automatic activation so we can show 100%
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // Progress is 0.0 to 0.9. 0.9 means “loaded, waiting for activation”
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingText.text = $"{(progress * 100f):F0}%";

            // When load has finished (progress ≥ 0.9)
            if (operation.progress >= 0.9f)
            {
                loadingText.text = "100%";
                // Optionally wait a short time or for user input here
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
