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
    private static AsyncOperation _loadingAsyncOperation;

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
        
        _loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString());

        while (!_loadingAsyncOperation.isDone)
        {
            yield return null;
        }
    }

    public static float GetLoadingProgress()
    {
        return _loadingAsyncOperation?.progress ?? 1f;
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
