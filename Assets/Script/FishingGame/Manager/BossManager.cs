using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    [SerializeField] private FishManager[] _bosses;

    private void Start()
    {
        WaveManager.Instance.EnterBossStage += OnEnterBossStage;
    }

    private void OnEnterBossStage(int mapIndex)
    {
        _bosses[mapIndex].SpawnFishFromStart();
    }
}
