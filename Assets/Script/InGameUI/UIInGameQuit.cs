using TinAungKhant.UIManagement;
using UnityEngine;
using UnityEngine.UI;

public class UIInGameQuit : UIBase
{
    [SerializeField] private Button quitBtn;
    [SerializeField] private Button cancelBtn;

    protected override void OnShow(UIBaseData data = null)
    {
        base.OnShow(data);
        cancelBtn.onClick.AddListener(() => OnCLickCloseBtn());
        quitBtn.onClick.AddListener(() => OnClickQuitBtn());
    }

    private void OnClickQuitBtn()
    {
        //UIManager.Instance.ShowUI(GLOBALCONST.UI_LOADING);
        UIManager.Instance.CloseUI(GLOBALCONST.UI_INGAMEQUIT);
        SceneLoader.Instance.LoadSceneAsync((int)SceneIndex.MAIN_MENU, false);
        UIManager.Instance.ShowUI(GLOBALCONST.UI_MAIN_MENU);
        
    }
    private void OnCLickCloseBtn()
    {
        UIManager.Instance.CloseUI(GLOBALCONST.UI_INGAMEQUIT);
    }
}
