using System.Collections;
using System.Collections.Generic;
using TinAungKhant.UIManagement;
using UnityEngine;

public class InitMainMenu : MonoBehaviour
{
    void Start()
    {
        UIManager.Instance.ShowUI(GLOBALCONST.UI_MAIN_MENU);
    } 
}
