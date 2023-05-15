using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour
{
    public string targetSceneName;
    public Text progressText;
    public float waitTime = 2f; // Time in seconds to wait before allowing scene activation

    void Start()
    {
        StartCoroutine(LoadTargetScene());
    }

    IEnumerator LoadTargetScene()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(targetSceneName);
        asyncOperation.allowSceneActivation = false;

        float elapsedTime = 0f;

        while (!asyncOperation.isDone)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(asyncOperation.progress / 0.1f);
            progressText.text = $"Loading: {progress * 100}%";

            if (asyncOperation.progress >= 0.1f && elapsedTime >= waitTime)
            {
                progressText.text = "Press any key";
                if (Input.anyKeyDown)
                {
                    asyncOperation.allowSceneActivation = true;
                }
            }
            yield return null;
        }
    }
}
