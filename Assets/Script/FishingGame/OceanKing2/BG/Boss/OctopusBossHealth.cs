using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusBossHealth : FishHealth
{
    protected override void Start()
    {
        base.Start();
        WaveManager.Instance.EnterBossStage += OnEnterBossStage;
    }

    public override void Die()
    {
        if (_audio != null) _audio.Play();
        _isDead = true;
        _fish.OnDead();
        GeneratedFishManager.Instance.RemoveFish(this);
    }

    private void OnEnterBossStage(int index)
    {
        SetCurrentHealthToMax();
        _isDead = false;
    }
}
