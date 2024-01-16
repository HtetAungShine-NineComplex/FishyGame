using System.Collections;
using System.Collections.Generic;
using TinAungKhant.UIManagement;
using UnityEngine;
using UnityEngine.UI;

public enum GameType
{
    FishingGame
}

public class GameUIButton : MonoBehaviour
{
    [SerializeField] private Button _btn;
    [SerializeField] private GameType _gameType; //the game type that will be loaded on click

    private void OnEnable()
    {
        _btn.onClick.AddListener(GoToGameUI);
    }

    private void OnDisable()
    {
        _btn.onClick.RemoveAllListeners();
    }

    private void GoToGameUI()
    {
        UIManager.Instance.CloseUI(GLOBALCONST.UI_MAIN_MENU);
        UIManager.Instance.ShowUI(GLOBALCONST.UI_FISHING_GAME);
    }
}
