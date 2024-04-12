using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LionturtleMove : SpecialFishMove
{
    protected override void DeadPhase()
    {
        ShowCoinFX();
    }
}
