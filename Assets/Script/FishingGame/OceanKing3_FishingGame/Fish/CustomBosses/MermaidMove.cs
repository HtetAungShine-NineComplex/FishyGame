using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MermaidMove : SpecialFishMove
{
    protected override void DeadPhase()
    {
        ShowCoinFX();
    }
}
