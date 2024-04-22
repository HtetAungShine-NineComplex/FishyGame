using System.Collections;
using System.Collections.Generic;
using TinAungKhant.UIManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : GameSceneManager
{

    void Start()
    {
        //UIManager.Instance.ShowUI(GLOBALCONST.UI_MAIN_MENU);
        BGSoundManager.Instance.PlaySound(BGSoundManager.Instance._mainMenuSound);
    }

    void initMainMenuUI(Scene scene, LoadSceneMode mode)
    {
        UIManager.Instance.ShowUI(GLOBALCONST.UI_MAIN_MENU);
    }
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += initMainMenuUI;
    }


    void OnDisable()
    {
        SceneManager.sceneLoaded -= initMainMenuUI;
    }
}
