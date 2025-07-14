using System.Collections;
using System.Collections.Generic;
using TinAungKhant.UIManagement;
using UnityEngine;

public class TempSlotQuitter : MonoBehaviour
{
    public void QuitSlot()
    {
        SceneLoader.Instance.LoadSceneAsync((int)SceneIndex.MAIN_MENU, false);
        UIManager.Instance.ShowUI(GLOBALCONST.UI_MAIN_MENU);
    }
}
