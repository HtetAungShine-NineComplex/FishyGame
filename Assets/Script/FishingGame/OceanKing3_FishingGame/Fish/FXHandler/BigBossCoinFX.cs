using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBossCoinFX : MonoBehaviour
{
    [SerializeField] private ScoreManager _scoreManager;

    private void Start()
    {
        Destroy(gameObject, 6f);
    }

    public void SetScore(int score)
    {
        _scoreManager.AddScore(score, 4);
    }
}
