using System.Collections;
using System.Collections.Generic;
using TinAungKhant.UIManagement;
using UnityEngine;

public class CanvasInstance : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    public static CanvasInstance Instance;

    private void Awake()
    {
        if(Instance == null) 
        { 
            Instance = this; 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Canvas GetMainCanvas()
    {
        return canvas;
    }
}
