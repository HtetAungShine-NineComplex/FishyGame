using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    public GameObject fishPrefab;
    public float spawnInterval = 1f;
    public int maxFish = 20;

    private int fishSpawned;
    private int fishDestroyed;
    private int fishAlive;

    private void Start()
    {
        //StartCoroutine(SpawnFishCoroutine());
    }

   /* IEnumerator SpawnFishCoroutine()
    {
        while(fishSpawned < maxFish)
        {
            Vector3 spawnPoint = SpawnpointManager.Instance.GetRandomSpawnPoint();

        }
    }*/
}
