using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : GameSceneManager
{
    //public static LoadingSceneManager Instance;

    [SerializeField] private GameObject LoadingObject;
    [SerializeField] private LoadingBar _loadingBar;

    protected override void Awake()
    {
        base.Awake();

        _loadingBar.ResetLoadingBar();

        //SceneManager.LoadSceneAsync((int)SceneIndex.MAIN_MENU, LoadSceneMode.Additive);
        //SceneLoader.Instance.ProgressChanged += _loadingBar.SetLoadingValue;
        
    }

    private void Start()
    {
        SceneLoader.Instance.LoadSceneAsync((int)SceneIndex.MAIN_MENU, false);
    }

    private void Update()
    {
        _loadingBar.SetLoadingValue(SceneLoader.Instance.Progress);
    }
}
