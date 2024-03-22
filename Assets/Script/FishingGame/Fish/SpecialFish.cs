using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SpecialFish : Fish
{
    public override void OnDead()
    {
        _coll.enabled = false;
        _move.OnDead();
        frameRate /= 4;
        CoinManager.Instance.ShowCoin(GetComponent<RectTransform>().anchoredPosition, CoinSpawnAmount, Score);

        StartCoroutine(FadeFish(5f));
    }

    
}
