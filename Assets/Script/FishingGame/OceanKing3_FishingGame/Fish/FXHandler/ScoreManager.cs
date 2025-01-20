using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text totalScoreTxt;
    private int totalScore;
    private Coroutine scoreCoroutine;

    void Start()
    {
        // Initialize the score if needed
        totalScore = 0;
        totalScoreTxt.text = String.Format("{0:#,###0}", totalScore);
    }

    public void AddScore(int amount)
    {
        int targetScore = totalScore + amount;
        if (scoreCoroutine != null)
        {
            StopCoroutine(scoreCoroutine);
        }
        scoreCoroutine = StartCoroutine(UpdateScore(totalScore, targetScore, .3f)); // Adjust duration as needed
        totalScore = targetScore;
    }

    public void AddScore(int amount, float duration)
    {
        int targetScore = totalScore + amount;
        if (scoreCoroutine != null)
        {
            StopCoroutine(scoreCoroutine);
        }
        scoreCoroutine = StartCoroutine(UpdateScore(totalScore, targetScore, duration)); // Adjust duration as needed
        totalScore = targetScore;
    }

    private IEnumerator UpdateScore(int startScore, int endScore, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float currentScore = Mathf.Lerp(startScore, endScore, progress);
            totalScoreTxt.text = String.Format("{0:#,###0}", currentScore); ;

            yield return null;
        }
        totalScoreTxt.text = String.Format("{0:#,###0}", endScore); ;
    }

    public void ResetScore()
    {
        totalScore = 0;
        totalScoreTxt.text = String.Format("{0:#,###0}", totalScore);
    }
}
