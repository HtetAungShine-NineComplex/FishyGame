using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crocodile : Fish
{
    [SerializeField] private Sprite[] _biteFrames;
    private bool _isBiting = false;
    private bool _canBite = false;

    private int _curBiteFrame = 0;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(BiteDelay());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (_canBite)
        {
            Debug.Log("Bite");

            if(collision.gameObject.CompareTag("Cannon"))
            {
                _isBiting = true;
                currentFrame = 0;
                StartCoroutine(_move.ChangeSpeedSmoothly(6f));
            }
        }
    }

    protected override void Update()
    {
        timer += Time.deltaTime;

        if (timer >= frameRate)
        {
            if(_isBiting)
            {
                Debug.Log("Playing Bite Frames");
                timer -= frameRate;
                _curBiteFrame = _curBiteFrame + 1;
                if(_curBiteFrame >= _biteFrames.Length)
                {
                    _isBiting = false;
                    _curBiteFrame = 0;
                }
                else
                {
                    fish_2D.sprite = _biteFrames[_curBiteFrame];
                }
            }
            else
            {
                timer -= frameRate;
                currentFrame = (currentFrame + 1) % fish_frames.Length;
                fish_2D.sprite = fish_frames[currentFrame];
            }
        }

    }

    IEnumerator BiteDelay()
    {
        if(_move.spawnPosition == SpawnPosition.Top || _move.spawnPosition == SpawnPosition.Bottom)
        {
            yield return new WaitForSeconds(3f);
        }
        else
        {
            yield return new WaitForSeconds(8f);
        }

        _canBite = true;
    }
}
