using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedFishManager : MonoBehaviour
{
    private List<FishHealth> _generatedFishList = new List<FishHealth>();

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

    public FishHealth GetRandomFish()
    {
        if (_generatedFishList.Count == 0)
            return null;

        int randomIndex = Random.Range(0, _generatedFishList.Count);
        FishHealth selectedFish = _generatedFishList[randomIndex];

        // Check if the fish has been destroyed
        if (selectedFish == null)
        {
            // If the fish has been destroyed, remove it from the list
            _generatedFishList.RemoveAt(randomIndex);

            // Recursively call GetRandomFish to get a new fish
            return GetRandomFish();
        }

        return selectedFish;
    }

    public void AddFish(FishHealth fish)
    {
        _generatedFishList.Add(fish);
    }

    public void RemoveFish(FishHealth fish)
    {
        _generatedFishList.Remove(fish);
    }
}
