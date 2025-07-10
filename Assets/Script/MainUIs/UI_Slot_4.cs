using System.Collections;
using System.Collections.Generic;
using TinAungKhant.UIManagement;
using UnityEngine;
using UnityEngine.UI;

public class UI_Slot_4 : UIBase
{
    [SerializeField] private Button _backBtn;

    protected override void OnShow(UIBaseData Data = null)
    {
        base.OnShow(Data);

        _backBtn.onClick.AddListener(BackToMenu);
    }

    protected override void OnClose()
    {
        base.OnClose();

        _backBtn?.onClick.RemoveListener(BackToMenu);
    }

    private void BackToMenu()
    {
        UIManager.Instance.CloseUI(GLOBALCONST.UI_SLOT_4);
        UIManager.Instance.ShowUI(GLOBALCONST.UI_MAIN_MENU);
        
    }
}
