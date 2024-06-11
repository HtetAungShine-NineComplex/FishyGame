using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpFish : Fish
{
    [SerializeField] private GameObject _explodePrefab;

    public override void OnDead()
    {
        base.OnDead();

        Instantiate(_explodePrefab, transform.position, Quaternion.identity, CanvasInstance.Instance.GetForegroundSpawn());
    }
}
