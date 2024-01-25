using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    public GameObject fishPrefab;
    public float spawnInterval = 1f;
    public int maxFish = 20;
    public Transform parentTF;
    public bool _isMainBoss;

    private int fishSpawned = 0;
    private int fishDestroyed;
    private int fishAlive;
    private Move fishMove;

    Vector3 spawnPoint;
    Vector3 endpoint;

    private void Start()
    {

        if(!_isMainBoss)
        {
            StartCoroutine(SpawnFishCoroutine());
        }
    }
    private void Awake()
    {
        fishMove = fishPrefab.GetComponent<Move>();
        
    }

    public void SpawnFishFromStart()
    {
        fishSpawned = 0;
        StartCoroutine(SpawnFishCoroutine());
    }

    IEnumerator SpawnFishCoroutine()
    {


        while (fishSpawned < maxFish)
        {
            if(!_isMainBoss)
            {
                yield return new WaitForSeconds(spawnInterval);
            }

            if(_isMainBoss)
            {
                spawnPoint = SpawnpointManager.Instance.GetRandomSpawnPointVertical();
            }
            else
            {
                spawnPoint = SpawnpointManager.Instance.GetRandomSpawnPoint();
            }

            endpoint = SpawnpointManager.Instance.GetRandomEndPoint(spawnPoint);


            //fishMove.startPoint_T.position = spawnPoint;
            //fishMove.destoryPoint_T.position = endpoint;

            GameObject fish = Instantiate(fishPrefab, spawnPoint, Quaternion.identity,parentTF);
            Move move = fish.GetComponent<Move>();
            move.SetPoints(spawnPoint, endpoint);

            GeneratedFishManager.Instance.AddFish(fish.GetComponent<FishHealth>());

            fishSpawned++;
            fishAlive++;

            if (_isMainBoss)
            {
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }
}
