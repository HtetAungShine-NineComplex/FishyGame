using System.Collections;
using System.Collections.Generic;
using TinAungKhant.UIManagement;
using UnityEngine;
using UnityEngine.UI;

public class UIRoomSelection_SpongeBob : UIBase
{
    [SerializeField] private Button _backBtn;
    [SerializeField] private Button _selectChairBtn; //temporary use only


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
        UIManager.Instance.ShowUI(GLOBALCONST.UI_FISHING_GAME_SPONGEBOB);
        UIManager.Instance.CloseUI(GLOBALCONST.UI_ROOM_SELECT_SpongeBob);
    }

    private void EnterRoom() //temp
    {
        UIManager.Instance.ShowUI(GLOBALCONST.UI_LOADING);
        SceneLoader.Instance.LoadSceneAsync((int)SceneIndex.FISHING_GAME_SCENE_SPONGEBOB, false);
    }

}
