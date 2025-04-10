using System.Collections;
using System.Collections.Generic;
using TinAungKhant.UIManagement;
using UnityEngine;
using UnityEngine.UI;

public class LiKuiGamePlayButton : MonoBehaviour
{
    [SerializeField] private Button _btn;

    private void OnEnable()
    {
        _btn.onClick.AddListener(GoToGameplay);
    }

    private void GoToGameplay()
    {
        UIManager.Instance.ShowUI(GLOBALCONST.UI_ROOM_SELECT_LIKUI);
        UIManager.Instance.CloseUI(GLOBALCONST.UI_FISHING_GAME_LIKUI);
    }
}
