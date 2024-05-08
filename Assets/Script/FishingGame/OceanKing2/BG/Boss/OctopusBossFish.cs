using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusBossFish : Fish
{
    [SerializeField] private Fish[] _shadowAndGlows;

    protected override void Start()
    {
        base.Start();
        WaveManager.Instance.EnterBossStage += OnBossStageEnter;
    }

    public override void OnDead()
    {
        foreach (Fish fish in _shadowAndGlows)
        {
            fish.gameObject.SetActive(false);
        }
        _coll.enabled = false;
        frameRate /= 4;
        StartCoroutine(FadeFish(0.5f));
        CoinManager.Instance.ShowCoin(_rectTransform.anchoredPosition, CoinSpawnAmount, Score);
    }

    private void OnBossStageEnter(int index)
    {
        if(_coll.enabled == false)
        {
            foreach (Fish fish in _shadowAndGlows)
            {
                fish.gameObject.SetActive(true);
                fish.ResetFrames();
            }
            ResetFrames();
            frameRate *= 4;
            _coll.enabled = true;
            fish_2D.color = new Color(255, 255, 255, 255);
        }
    }
}
