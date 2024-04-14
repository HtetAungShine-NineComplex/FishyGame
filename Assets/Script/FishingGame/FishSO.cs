using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For type
public enum FishType
{
    ClownFish,
    BlueTang,
    Other
}

[CreateAssetMenu(fileName = "New Fish", menuName ="NPC/Fish")]
public class FishSO : ScriptableObject
{
    
    public FishType fishType;
    public Sprite FishIcon;

    //For Health
    [Range(100f, 10000f)] public int MaxHealth;
    public int Score;
    public int CoinSpawnAmount; //for fishes that spawn coin when killed

    //For Animation
    public Sprite[] FishFrames;
    public Sprite FishDefaultFrame;
    [Range(0.02f,1f)]public float FishFrameRate;

    //For Movement
    public float speed = 2f;
    public AnimationCurve SpeedCurve;
}
