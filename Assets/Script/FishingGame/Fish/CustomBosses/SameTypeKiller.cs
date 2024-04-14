using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SameTypeKiller : Fish
{
    public override void OnDead()
    {
        base.OnDead();
        GeneratedFishManager.Instance.KillAllSameTypeFishes(Type);
    }
}
