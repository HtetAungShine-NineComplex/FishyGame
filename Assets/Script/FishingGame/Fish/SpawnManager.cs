using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject fishPrefab;
    public float spawnInterval = 2f;
    public int maxFish = 10;

    private int fishSpawned;
    private int fishDestroyed;
    private int fishAlive;

    void Start()
    {
        StartCoroutine(SpawnFishRoutine());
    }

    IEnumerator SpawnFishRoutine()
    {
        while (fishSpawned < maxFish)
        {
            Vector3 spawnPoint = GetRandomSpawnPoint();
            Vector3 endPoint = GetRandomEndPoint(spawnPoint);

            GameObject fish = Instantiate(fishPrefab, spawnPoint, Quaternion.identity);
            //fish.GetComponent<FishController>().SetEndPoint(endPoint);

            fishSpawned++;
            fishAlive++;

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    Vector3 GetRandomSpawnPoint()
    {
        float randomX = 0f;
        float randomY = 0f;

        // Randomly select one of the screen borders
        int border = Random.Range(0, 4);

        float screenX = Screen.width;
        float screenY = Screen.height;

        switch (border)
        {
            case 0: // Top
                randomX = Random.Range(0f, screenX);
                randomY = screenY;
                break;
            case 1: // Bottom
                randomX = Random.Range(0f, screenX);
                randomY = 0f;
                break;
            case 2: // Left
                randomX = 0f;
                randomY = Random.Range(0f, screenY);
                break;
            case 3: // Right
                randomX = screenX;
                randomY = Random.Range(0f, screenY);
                break;
        }

        Vector3 spawnPoint = Camera.main.ScreenToWorldPoint(new Vector3(randomX, randomY, 10f));
        return spawnPoint;
    }

    Vector3 GetRandomEndPoint(Vector3 spawnPoint)
    {
        // Modify this logic based on your game requirements
        // For simplicity, let's just move the endpoint in the opposite direction
        return spawnPoint - new Vector3(10f, 0f, 0f);
    }

    public void FishDestroyed()
    {
        fishDestroyed++;
        fishAlive--;
    }

    // You can add methods to get statistics (fishSpawned, fishDestroyed, fishAlive).
}

