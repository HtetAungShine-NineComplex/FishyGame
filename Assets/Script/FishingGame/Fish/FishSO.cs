using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fish", menuName = "NPC/ Fish SO")]
public class FishSO : ScriptableObject
{
    public string FishName;
    public string description;
    public Sprite fish_2D;
    public int health = 20;
    public float speed = 2f;
}