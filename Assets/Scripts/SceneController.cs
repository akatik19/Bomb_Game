using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    public event Action BeforeSceneUnload;
    public event Action AfterSceneLoad;

    public void LoadScene(string level)
    {
        StartCoroutine(LoadSceneAsync(level));
    }

    public void UnloadScene()
    {
        StartCoroutine(UnloadSceneAsync());
    }

    private IEnumerator LoadSceneAsync(string level)
    {
        yield return UnloadSceneAsync();

        yield return SceneManager.LoadSceneAsync(level, LoadSceneMode.Additive);
        if (AfterSceneLoad != null)
            AfterSceneLoad();

        SceneManager.SetActiveScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
    }

    private IEnumerator UnloadSceneAsync()
    {
        if (SceneManager.sceneCount > 1)
        {
            if (BeforeSceneUnload != null)
                BeforeSceneUnload();
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
