using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fish : MonoBehaviour
{
    [SerializeField] private Image fish_2D;
    [SerializeField] private Sprite[] fish_frames;

    private float frameRate = 0.2f;
    private int currentFrame;
    private float timer;

    public FishSO FishConfig;

    private void Start()
    {
        
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= frameRate)
        {
            timer -= frameRate;
            currentFrame = (currentFrame + 1) % fish_frames.Length;
            fish_2D.sprite = fish_frames[currentFrame];
        }
            
    }
}
