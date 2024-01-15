using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fish : MonoBehaviour
{
    [SerializeField] private FishSO fishSO;

    private Image fish_2D;
    private Sprite[] fish_frames;
    private float frameRate = 0.05f;

    private int currentFrame;
    private float timer;

    private void Start()
    {
        fish_2D = GetComponent<Image>();

        fish_2D.sprite = fishSO.FishDefaultFrame;
        fish_frames = fishSO.FishFrames;
        frameRate = fishSO.FishFrameRate;
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
