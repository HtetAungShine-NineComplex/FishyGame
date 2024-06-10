using System.Collections;
using System.Collections.Generic;
using TinAungKhant.UIManagement;
using UnityEngine;

public class CanvasInstance : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Transform _foregroundSpawn;
    [SerializeField] private Transform _midGroundSpawn;
    [SerializeField] private CannonHandler _cannonHandler;
    [SerializeField] private BossRewardShower _bossRewardShower; //for Kirin
    [SerializeField] private Transform _anglerFishLight;

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

    public Transform GetMidGroundSpawn()
    {
        return _midGroundSpawn;
    }

    public CannonHandler GetCannonHandler()
    {
        return _cannonHandler;
    }

    public BossRewardShower GetBossRewardShower()
    {
        return _bossRewardShower;
    }

    public Transform GetAnglerFishLight()
    {
        return _anglerFishLight;
    }
}
