using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedFishManager : MonoBehaviour
{
    private List<Fish> _generatedFishList = new List<Fish>();

    public static GeneratedFishManager Instance;
    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    public void AddFish(Fish fish)
    {
        _generatedFishList.Add(fish);
    }

    public void RemoveFish(Fish fish)
    {
        _generatedFishList.Remove(fish);
    }
}
