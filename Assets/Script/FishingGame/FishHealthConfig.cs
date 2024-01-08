using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FishHealthConfig")]
public class FishHealthConfig : ScriptableObject
{
    [Range(10f, 100f)] public int MaxHealth;

    [Range(10f, 100f)] public int HealthThreshold;

    public string FishName;

    public string description;
    public Sprite fish_2D;
    public int health = 20;
    public float speed = 2f;
}
