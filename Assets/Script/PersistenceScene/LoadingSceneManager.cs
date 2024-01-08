using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    public static LoadingSceneManager Instance;

    private List<AsyncOperation> sceneLoadingOperation = new List<AsyncOperation>();
    private float totalSceneProgress;

    [SerializeField] private GameObject LoadingObject;
    [SerializeField] private Slider loadingSlider;

    private void Awake()
    {
        Instance = this;
        //SceneManager.LoadSceneAsync((int)SceneIndex.MAIN_MENU, LoadSceneMode.Additive);
    }

    private void Start()
    {
        LoadScene();
    }
    private void LoadScene()
    {
        LoadingObject.SetActive(true);

        //sceneLoadingOperation.Add(SceneManager.UnloadSceneAsync((int)SceneIndex.LOADING_SCENE));
        sceneLoadingOperation.Add(SceneManager.LoadSceneAsync((int)SceneIndex.MAIN_MENU, LoadSceneMode.Additive));
        //sceneLoadingOperation.Add(SceneManager.LoadSceneAsync((int)SceneIndex.FISHING_GAME, LoadSceneMode.Additive));

        StartCoroutine(IGameSceneLoadProgress());
    }

    private IEnumerator IGameSceneLoadProgress()
    {
        for(int i = 0; i < sceneLoadingOperation.Count; i++)
        {
            while (!sceneLoadingOperation[i].isDone)
            {
                totalSceneProgress = 0;
                foreach(AsyncOperation sceneloading in sceneLoadingOperation)
                {
                    totalSceneProgress += sceneloading.progress;
                }
                totalSceneProgress = (totalSceneProgress / sceneLoadingOperation.Count) * 100f;
                loadingSlider.value = Mathf.RoundToInt(totalSceneProgress);
                yield return null;
            }
        }

        LoadingObject.SetActive(true);
    }
}
