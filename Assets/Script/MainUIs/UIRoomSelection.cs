using System.Collections;
using System.Collections.Generic;
using TinAungKhant.UIManagement;
using UnityEngine;
using UnityEngine.UI;

public class UIRoomSelection : UIBase
{
    [SerializeField] private Button _backBtn;
    [SerializeField] private Button _selectChairBtn; //temporary use only

    private bool _sceneChanged = false;

    protected override void OnShow(UIBaseData Data = null)
    {
        base.OnShow(Data);

        _backBtn.onClick.AddListener(BackToFishingGameUI);
        _selectChairBtn.onClick.AddListener(EnterRoom);
    }

    protected override void OnClose()
    {
        base.OnClose();
        _backBtn.onClick.RemoveAllListeners();
        _selectChairBtn.onClick.RemoveAllListeners();
    }

    private void BackToFishingGameUI()
    {
        _sceneChanged = false;
        UIManager.Instance.ShowUI(GLOBALCONST.UI_FISHING_GAME);
        UIManager.Instance.CloseUI(GLOBALCONST.UI_ROOM_SELECT);
    }

    private void EnterRoom() //temp
    {
        _sceneChanged = true;
        UIManager.Instance.ShowUI(GLOBALCONST.UI_LOADING);
        SceneLoader.Instance.LoadSceneAsync((int)SceneIndex.FISHING_GAME_SCENE, false);
    }

    private void OnDisable()
    {
        if(_sceneChanged)
        {
            UIManager.Instance.CloseAllOpeningUIs();
        }
    }
}
