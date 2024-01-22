using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    public event Action<float> ProgressChanged;

    private List<AsyncOperation> _sceneLoadingOperation = new List<AsyncOperation>();

    private float _progress;

    public float Progress 
    {
        get
        {
            return _progress;
        }

        private set
        {
            ProgressChanged?.Invoke(value);
            _progress = value;
            
        }
    }

    public override void Awake()
    {
        base.Awake();

        //SceneManager.LoadScene((int)SceneIndex.LOADING_SCENE);
    }

    public void LoadSceneAsync(int sceneIndex, bool isAdditive)
    {
        if(isAdditive)
        {
            _sceneLoadingOperation.Add(SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive));
        }
        else
        {
            _sceneLoadingOperation.Add(SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single));
        }
        

        StartCoroutine(LoadSceneAsyncCoroutine());
    }

    IEnumerator LoadSceneAsyncCoroutine()
    {
        for (int i = 0; i < _sceneLoadingOperation.Count; i++)
        {
            while (!_sceneLoadingOperation[i].isDone)
            {
                Progress = 0;

                for (int j = 0; j < _sceneLoadingOperation.Count; j++)
                {
                    Progress += _sceneLoadingOperation[j].progress;
                }

                Progress = Progress / _sceneLoadingOperation.Count * 100f;
                Debug.Log(Progress);

                if (_sceneLoadingOperation[i].isDone)
                {
                    _sceneLoadingOperation.Remove(_sceneLoadingOperation[i]);
                }

                // Update loading bar
                yield return null;
            }
        }

        
    }

}
