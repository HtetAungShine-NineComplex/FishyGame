using System.Collections;
using System.Collections.Generic;
using TinAungKhant.UIManagement;
using UnityEngine;
using UnityEngine.UI;

public class GameplayButton : MonoBehaviour
{
    [SerializeField] private Button _btn;

    private void OnEnable()
    {
        _btn.onClick.AddListener(GoToGameplay);
    }

    private void GoToGameplay()
    {
        UIManager.Instance.ShowUI(GLOBALCONST.UI_LOADING);
        SceneLoader.Instance.LoadSceneAsync((int)SceneIndex.FISHING_GAME_SCENE, false);
    }
}
