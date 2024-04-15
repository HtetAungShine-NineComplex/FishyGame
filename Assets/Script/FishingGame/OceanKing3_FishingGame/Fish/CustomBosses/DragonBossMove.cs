using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBossMove : SpecialFishMove
{
    protected override void DeadPhase()
    {
        ShowCoinFX();
    }
}
