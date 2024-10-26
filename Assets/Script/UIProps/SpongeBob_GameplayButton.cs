using System.Collections;
using System.Collections.Generic;
using TinAungKhant.UIManagement;
using UnityEngine;
using UnityEngine.UI;
public class SpongeBob_GameplayButton : MonoBehaviour
{
    [SerializeField] private Button _btn;

    private void OnEnable()
    {
        _btn.onClick.AddListener(GoToGameplay);
    }

    private void GoToGameplay()
    {
        UIManager.Instance.ShowUI(GLOBALCONST.UI_ROOM_SELECT_SpongeBob);
        UIManager.Instance.CloseUI(GLOBALCONST.UI_FISHING_GAME_DRAGONBALL);
    }
}
