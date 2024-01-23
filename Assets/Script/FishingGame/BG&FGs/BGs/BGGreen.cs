using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGGreen : GameBG
{
    [SerializeField] private Animator _animator;

    protected override void OnEnterNormalStage(int mapIndex)
    {
        base.OnEnterNormalStage(mapIndex);
        if (mapIndex != this.mapIndex) return;
        gameObject.SetActive(true);
    }

    protected override void OnEnterBossStage(int mapIndex)
    {
        base.OnEnterBossStage(mapIndex);
        if (mapIndex != this.mapIndex) return;

        _animator.SetBool("IsBossFight", true);
    }

    protected override void OnEnterBonusStage(int mapIndex)
    {
        _animator.SetBool("IsBossFight", false);

        base.OnEnterBonusStage(mapIndex);
        if (mapIndex == this.mapIndex)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }

        
    }
}
