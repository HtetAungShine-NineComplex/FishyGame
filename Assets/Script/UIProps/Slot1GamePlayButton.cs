using System.Collections;
using System.Collections.Generic;
using TinAungKhant.UIManagement;
using UnityEngine;
using UnityEngine.UI;

public class Slot1GamePlayButton: MonoBehaviour
{
    [SerializeField] private Button _btn;
    [SerializeField] private int sceneIndex = 14;

    private void OnEnable()
    {
        _btn.onClick.AddListener(GoToGameplay);
    }

    private void GoToGameplay()
    {
        UIManager.Instance.ShowUI(GLOBALCONST.UI_LOADING);
        //UIManager.Instance.CloseUI(GLOBALCONST.UI_ROOM_SELECT_DragonBall);
        SceneLoader.Instance.LoadSceneAsync(sceneIndex, false);
    }
}
