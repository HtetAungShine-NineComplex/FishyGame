using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    public GameObject fishPrefab;
    public float spawnInterval = 1f;
    public int maxFish = 20;
    public Transform parentTF;

    private int fishSpawned = 0;
    private int fishDestroyed;
    private int fishAlive;
    private Move fishMove;

    Vector3 spawnPoint;
    Vector3 endpoint;
    private void Start()
    {

        StartCoroutine(SpawnFishCoroutine());
    }
    private void Awake()
    {
        fishMove = fishPrefab.GetComponent<Move>();
        
    }
    IEnumerator SpawnFishCoroutine()
    {


        while (fishSpawned < maxFish)
        {
            yield return new WaitForSeconds(spawnInterval);

            spawnPoint = SpawnpointManager.Instance.GetRandomSpawnPoint();
            endpoint = SpawnpointManager.Instance.GetRandomEndPoint(spawnPoint, SpawnpointManager.Instance.GetSpawnPosition());


            //fishMove.startPoint_T.position = spawnPoint;
            //fishMove.destoryPoint_T.position = endpoint;

            GameObject fish = Instantiate(fishPrefab, spawnPoint, Quaternion.identity,parentTF);
            Move move = fish.GetComponent<Move>();
            move.SetPoints(spawnPoint, endpoint);
            move.spawnPosition = SpawnpointManager.Instance.GetSpawnPosition();

            fishSpawned++;
            fishAlive++;

            
        }
    }
}
