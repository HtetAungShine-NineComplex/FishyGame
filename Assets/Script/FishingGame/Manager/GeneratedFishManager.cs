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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        FishHealth fish = collision.GetComponent<FishHealth>();
        if (fish != null)
        {
            AddFish(fish);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        FishHealth fish = collision.GetComponent<FishHealth>();
        if (fish != null)
        {
            RemoveFish(fish);
        }
    }

    public List<FishHealth> GetGeneratedFishList()
    {
        return _generatedFishList;
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
        if(_generatedFishList.Contains(fish))
        {
            _generatedFishList.Remove(fish);
        }
    }

    public bool HasFish(FishHealth fish) => _generatedFishList.Contains(fish);
}

