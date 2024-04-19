using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KirinSpecialFish : SpecialFish
{
    public override void OnDead()
    {
        base.OnDead();

        CanvasInstance.Instance.GetBossRewardShower().ShowReward(Score);
    }
}
