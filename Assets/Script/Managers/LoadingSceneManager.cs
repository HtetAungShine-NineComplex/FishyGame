using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : GameSceneManager
{
    //public static LoadingSceneManager Instance;

    [SerializeField] private GameObject LoadingObject;
    [SerializeField] private LoadingBar _loadingBar;

    private float _progress;

    protected override void Awake()
    {
        base.Awake();

        _loadingBar.ResetLoadingBar();

        //SceneManager.LoadSceneAsync((int)SceneIndex.MAIN_MENU, LoadSceneMode.Additive);
        //SceneLoader.Instance.ProgressChanged += _loadingBar.SetLoadingValue;

    }

    private void Start()
    {
        StartCoroutine(StartLoading());
        //SceneLoader.Instance.LoadSceneAsync((int)SceneIndex.MAIN_MENU, false);
    }

    private void Update()
    {
        //_loadingBar.SetLoadingValue(SceneLoader.Instance.Progress);
        _loadingBar.SetLoadingValue(_progress);
    }

    private IEnumerator StartLoading()
    {
        _progress = 0;
        float elapsedTime = 0f;

        while (elapsedTime < 7f)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / 7;

            //progress *= 0.7f;

            /*loadingSlider.value = progress;
            loadingTxt.text = (int)(progress * 100) + "%";*/

            _progress = progress * 100 * 0.7f;
            

            yield return null;
        }

        // Loading completed, do any necessary actions here
        Debug.Log("Loading completed!");
        
        StartLoadScene();
    }

    private void StartLoadScene()
    {
        SceneLoader.Instance.LoadSceneAsync((int)SceneIndex.MAIN_MENU, false);
        SceneLoader.Instance.ProgressChanged += OnProgressChanged;
    }

    private void OnProgressChanged(float progress)
    {
        _progress += progress * 0.3f;
        Debug.Log($"Progress: {progress * 0.3f} and {_progress}. ");

        _loadingBar.SetLoadingValue(_progress);
    }
}