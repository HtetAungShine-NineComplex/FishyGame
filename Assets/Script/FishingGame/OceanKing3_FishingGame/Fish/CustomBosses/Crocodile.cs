using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crocodile : Fish
{
    [SerializeField] private Sprite[] _biteFrames;
    [SerializeField] private Collider2D _biteColl;
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
            if(collision.gameObject.CompareTag("Cannon"))
            {
                Debug.Log("Bite");
                BGShaker.Instance.Shake(4);
                _isBiting = true;
                currentFrame = 0;
                StartCoroutine(_move.ChangeSpeedSmoothly(4f));
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
        yield return new WaitForSeconds(3f);

        _canBite = true;
    }
}
