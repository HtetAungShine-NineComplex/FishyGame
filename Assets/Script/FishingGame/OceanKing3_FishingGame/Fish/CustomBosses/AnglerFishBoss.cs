using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnglerFishBoss : Fish
{
    [SerializeField] private Transform _light;
    [SerializeField] private Transform _lightRoot;

    protected override void Start()
    {
        base.Start();
        _light = CanvasInstance.Instance.GetAnglerFishLight();
        _light.GetComponent<CanvasGroup>().alpha = 1;
    }

    protected override void Update()
    {
        //base.Update();

        if (_light == null) return;
        _light.transform.position = _lightRoot.position;
    }

    public override void OnDead()
    {
        base.OnDead();

        _light.GetComponent<CanvasGroup>().alpha = 0;
    }

    private void OnDestroy()
    {
        if (_light == null) return;
        _light.GetComponent<CanvasGroup>().alpha = 0;
    }
}
