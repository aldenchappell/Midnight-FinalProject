using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    private class LoadingMonobehaviour : MonoBehaviour{}
    public enum Scene
    {
        MAINMENU,
        LOADING,
        LOBBY,
        FLOORONE,
        FLOORTWO,
        FLOORTHREE
    }

    private static Action _onLoaderCallback;
    private static AsyncOperation loadingAsyncOperation;

    public static void Load(Scene scene)
    {
        _onLoaderCallback = () =>
        {
            GameObject loadingGameObject = new GameObject("Loading Game Object");
            loadingGameObject.AddComponent<LoadingMonobehaviour>().StartCoroutine(LoadSceneAsync(scene));
        };
        
        SceneManager.LoadScene(Scene.LOADING.ToString());
    }

    private static IEnumerator LoadSceneAsync(Scene scene)
    {
        yield return null;
        
        loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString());

        while (!loadingAsyncOperation.isDone)
        {
            yield return null;
        }
    }

    public static float GetLoadingProgress()
    {
        return loadingAsyncOperation?.progress ?? 1f;
    }

    public static void LoaderCallback()
    {
        if (_onLoaderCallback != null)
        {
            _onLoaderCallback();
            _onLoaderCallback = null;
        }
    }
}
