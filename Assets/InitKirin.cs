using System.Collections;
using System.Collections.Generic;
using TinAungKhant.UIManagement;
using UnityEngine;

public class InitKirin : MonoBehaviour
{
    private void Start()
    {
        UIManager.Instance.CloseAllOpeningUIs();
    }
}
