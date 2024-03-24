using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCrab : Fish
{
    [SerializeField] private Sprite[] _deathFrames;

    private bool _isDead = false;

    private float _deathFrameRate = 0.03f;

    private int currentDeathFrame = 0;
    private float timerDeath = 0;

    protected override void Update()
    {
        if (!_isDead)
        {
            base.Update();
        }
        else
        {
            Debug.Log("BossCrab Death Update");
            timerDeath += Time.deltaTime;

            if (timerDeath >= _deathFrameRate)
            {
                timerDeath -= _deathFrameRate;
                currentDeathFrame = (currentDeathFrame + 1) % _deathFrames.Length;
                fish_2D.sprite = _deathFrames[currentDeathFrame];
            }
        }
    }

    public override void OnDead()
    {
        base.OnDead();
        _isDead = true;
    }
}
