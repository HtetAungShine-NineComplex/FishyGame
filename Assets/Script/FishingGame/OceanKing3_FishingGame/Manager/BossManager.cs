using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    [SerializeField] private FishManager _phoenixBoss;
    [SerializeField] private FishManager[] _firstMapBosses;
    [SerializeField] private FishManager[] _secondMapBosses;
    [SerializeField] private FishManager[] _thirdMapBosses;

    public bool isActive = true;

    private void Start()
    {
        if (!isActive)
            { return; }

        WaveManager.Instance.EnterBossStage += OnEnterBossStage;
    }

    private void OnEnterBossStage(int mapIndex)
    {
        StartCoroutine(SpawnBosses(mapIndex));
    }

    IEnumerator SpawnBosses(int mapIndex)
    {
        yield return new WaitForSeconds(5f);

        switch (mapIndex)
        {
            case 0: //phoenix
                _phoenixBoss.SpawnFishFromStart();
                break;

            case 1: //first map
                for (int i = 0; i < _firstMapBosses.Length; i++)
                {
                    _firstMapBosses[i].SpawnFishFromStart();
                }
                break;

            case 2: //second map
                for (int i = 0; i < _secondMapBosses.Length; i++)
                {
                    _secondMapBosses[i].SpawnFishFromStart();
                }
                break;

            case 3: //third map
                for (int i = 0; i < _thirdMapBosses.Length; i++)
                {
                    _thirdMapBosses[i].SpawnFishFromStart();
                }
                break;

            default:
                break;
        }

    }
}
