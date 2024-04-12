using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SpecialFish : Fish
{
    [SerializeField] private Sprite[] _deadFrames;
    [SerializeField] private bool _hasDeadAnimations = true;

    protected bool _isDead = false;

    protected override void Update()
    {
        if(_isDead && _deadFrames.Length > 0)
        {
            Debug.Log("Dying");

            timer += Time.deltaTime;

            if (timer >= frameRate)
            {
                timer -= frameRate;
                currentFrame = (currentFrame + 1) % _deadFrames.Length;
                fish_2D.sprite = _deadFrames[currentFrame];
            }
        }
        else
        {
            base.Update();
        }

        
    }

    public override void OnDead()
    {
        _isDead = true;
        currentFrame = 0;
        _coll.enabled = false;
        _move.OnDead();
        if(!_hasDeadAnimations)
        {
            frameRate /= 4;
        }
        CoinManager.Instance.ShowCoin(GetComponent<RectTransform>().anchoredPosition, CoinSpawnAmount, Score);

        StartCoroutine(FadeFish(5f));
    }

    
}
