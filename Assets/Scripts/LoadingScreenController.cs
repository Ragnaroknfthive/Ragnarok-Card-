///////////////////////////////////////
///LoadingScreenController.cs
///
///This script is responsible for loading the target scene and displaying a loading screen with a progress indicator.

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour
{
    public string targetSceneName;// The name of the scene to load
    public Text progressText;// The text component to display the loading progress
    public float waitTime = 2f; // Time in seconds to wait before allowing scene activation

    void Start()
    {
        StartCoroutine(LoadTargetScene());// Start loading the target scene
    }

    IEnumerator LoadTargetScene()// Coroutine to load the target scene
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(targetSceneName);// Load the target scene
        asyncOperation.allowSceneActivation = false;// Do not allow the scene to be activated until the progress is 100%

        float elapsedTime = 0f;// Elapsed time since the coroutine started

        while (!asyncOperation.isDone)// While the scene is not loaded
        {
            elapsedTime += Time.deltaTime;// Update the elapsed time

            float progress = Mathf.Clamp01(asyncOperation.progress / 0.1f);// Calculate the progress of the scene loading
            progressText.text = $"Loading: {progress * 100}%";// Update the progress text

            if (asyncOperation.progress >= 0.1f && elapsedTime >= waitTime)// If the scene is loaded and the wait time has passed
            {
                progressText.text = "Press any key";// Update the progress text to prompt the user to press any key
                if (Input.anyKeyDown)// If any key is pressed
                {
                    asyncOperation.allowSceneActivation = true;// Allow the scene to be activated
                }
            }
            yield return null;
        }
    }
}
