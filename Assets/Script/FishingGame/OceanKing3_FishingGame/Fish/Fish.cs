using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fish : MonoBehaviour
{
    [SerializeField] protected RectTransform _rectTransform;
    [SerializeField] private FishSO fishSO;
    [SerializeField] protected Move _move;
    [SerializeField] protected Collider2D _coll;

    [SerializeField] protected Image fish_2D;
    [SerializeField] protected CanvasGroup _fishGlow;

    protected Sprite[] fish_frames;
    protected float frameRate = 0.05f;

    protected int currentFrame;
    protected float timer;

    public int Score { get; private set; }
    public int CoinSpawnAmount { get; private set; }

    public Sprite FishIcon { get; private set; }
    public FishType Type { get; private set; }

    protected virtual void Start()
    {
        //fish_2D = GetComponent<Image>();

        fish_2D.sprite = fishSO.FishDefaultFrame;
        fish_frames = fishSO.FishFrames;
        frameRate = fishSO.FishFrameRate;
        Score = fishSO.Score;
        CoinSpawnAmount = fishSO.CoinSpawnAmount;
        FishIcon = fishSO.FishIcon;
        Type = fishSO.fishType;
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

    public virtual void OnHit()
    {
        if(_fishGlow == null) return;

        StartCoroutine(HitEffect());
    }

    IEnumerator HitEffect()
    {

        float duration = 0.3f; // Duration of the fade in/out
        float elapsed = 0f; // Time elapsed since the start of the fade

        float startAlpha = 0f; // Starting alpha (completely transparent)
        float endAlpha = 1f; // Ending alpha (completely opaque)

        _fishGlow.alpha = startAlpha;

        while (elapsed < duration)
        {
            // Calculate the current time ratio
            float t = elapsed / duration;

            // Lerp the alpha and apply it
            _fishGlow.alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            // Update the elapsed time
            elapsed += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Now we lerp back to the original alpha
        elapsed = 0f;
        while (elapsed < duration)
        {
            // Calculate the current time ratio
            float t = elapsed / duration;

            // Lerp the alpha and apply it
            _fishGlow.alpha = Mathf.Lerp(endAlpha, startAlpha, t);

            // Update the elapsed time
            elapsed += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the alpha is set back to the original value
        _fishGlow.alpha = startAlpha;
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

    public void ResetFrames()
    {
        currentFrame = 0;
        timer = 0;
    }
}
