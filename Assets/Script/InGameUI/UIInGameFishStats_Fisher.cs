using TinAungKhant.UIManagement;
using UnityEngine;
using UnityEngine.UI;

public class UIInGameFishStats_Fisher : UIBase
{
    [SerializeField] private Button CloseBtn;

    protected override void OnShow(UIBaseData data = null)
    {
        base.OnShow(data);
        CloseBtn.onClick.AddListener(()=>OnCLickCloseBtn());
    }

    private void OnCLickCloseBtn()
    {
        UIManager.Instance.CloseUI(GLOBALCONST.UI_FISHSTATS_FISHER);
    }
}