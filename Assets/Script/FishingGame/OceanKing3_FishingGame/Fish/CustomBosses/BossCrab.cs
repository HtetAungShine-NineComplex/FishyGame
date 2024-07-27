using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCrab : SpecialFish
{
    protected override void Start()
    {
        base.Start();

        WaveManager.Instance.ExitBossStage += OnBossStageExit;
    }

    private void OnDestroy()
    {
        WaveManager.Instance.ExitBossStage -= OnBossStageExit;
    }

    private void OnBossStageExit(int index)
    {
        Destroy(_rectTransform.gameObject);
    }
}
