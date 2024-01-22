using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fish", menuName ="NPC/Fish")]
public class FishSO : ScriptableObject
{
    //For type
    public enum FishType
    {
        Small,
        Medium,
        Special,
        Boss
    }
    public FishType fishType;

    //For Health
    [Range(100f, 10000f)] public int MaxHealth;

    //For Animation
    public Sprite[] FishFrames;
    public Sprite FishDefaultFrame;
    [Range(0.1f,1f)]public float FishFrameRate;

    //For Movement
    public float speed = 2f;
    public AnimationCurve SpeedCurve;
}
