using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public bool Damage(int amount);
    public void InstantDie();
}
