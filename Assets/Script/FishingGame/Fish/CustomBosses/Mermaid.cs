using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mermaid : Fish
{
    public override void OnDead()
    {
        base.OnDead();

        MermaidFX.Instance.PlayFX();
    }
}
