using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fish : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private FishSO fishSO;
    [SerializeField] protected Move _move;
    [SerializeField] protected Collider2D _coll;

    [SerializeField] protected Image fish_2D;
    private Sprite[] fish_frames;
    protected float frameRate = 0.05f;

    private int currentFrame;
    private float timer;

    public int Score { get; private set; }
    public int CoinSpawnAmount { get; private set; }

    protected virtual void Start()
    {
        //fish_2D = GetComponent<Image>();

        fish_2D.sprite = fishSO.FishDefaultFrame;
        fish_frames = fishSO.FishFrames;
        frameRate = fishSO.FishFrameRate;
        Score = fishSO.Score;
        CoinSpawnAmount = fishSO.CoinSpawnAmount;
    }

    protected virtual void Update()
    {
        timer += Time.deltaTime;

        if (timer >= frameRate)
        {
            timer -= frameRate;
            currentFrame = (currentFrame + 1) % fish_frames.Length;
            fish_2D.sprite = fish_frames[currentFrame];
        }
            
    }

    public virtual void OnDead()
    {
        _coll.enabled = false;
        _move.OnDead();
        frameRate /= 4;
        StartCoroutine(FadeFish(0.5f));
        CoinManager.Instance.ShowCoin(_rectTransform.anchoredPosition, CoinSpawnAmount, Score);
    }

    protected IEnumerator FadeFish(float fadeDelay)
    {
        yield return new WaitForSeconds(fadeDelay);

        //fish_2D.color = Color.Lerp(fish_2D.color, new Color(255, 255, 255, 0), 1f * Time.deltaTime);
        Color curColor = fish_2D.color;
        while (curColor.a > 0)
        {
            curColor.a = Mathf.Lerp(curColor.a, 0, 2f * Time.deltaTime);
            fish_2D.color = curColor;
            yield return new WaitForEndOfFrame();
        }
        
    }
}
