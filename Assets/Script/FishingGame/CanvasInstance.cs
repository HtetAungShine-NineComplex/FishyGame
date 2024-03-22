using System.Collections;
using System.Collections.Generic;
using TinAungKhant.UIManagement;
using UnityEngine;

public class CanvasInstance : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Transform _foregroundSpawn;
    [SerializeField] private CannonHandler _cannonHandler;

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

    public Transform GetForegroundSpawn()
    {
        return _foregroundSpawn;
    }

    public CannonHandler GetCannonHandler()
    {
        return _cannonHandler;
    }
}
