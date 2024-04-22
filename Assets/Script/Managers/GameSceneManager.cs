using System.Collections;
using System.Collections.Generic;
using TinAungKhant.UIManagement;
using UnityEngine;

public abstract class GameSceneManager : MonoBehaviour
{
    [SerializeField] protected Canvas _mainUICanvas;

    protected virtual void Awake()
    {
        /*if(_mainUICanvas != null)
        {
            UIManager.Instance.SetUIRoot(_mainUICanvas.transform);
        }*/
    }
}
