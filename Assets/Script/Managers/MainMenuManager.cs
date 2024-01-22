using System.Collections;
using System.Collections.Generic;
using TinAungKhant.UIManagement;
using UnityEngine;

public class MainMenuManager : GameSceneManager
{

    void Start()
    {
        UIManager.Instance.ShowUI(GLOBALCONST.UI_MAIN_MENU);
        BGSoundManager.Instance.PlaySound(BGSoundManager.Instance._mainMenuSound);
    }

}
