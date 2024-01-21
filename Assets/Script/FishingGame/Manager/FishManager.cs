using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    public GameObject fishPrefab;
    public float spawnInterval = 1f;
    public int maxFish = 20;
    public Transform parentTF;

    private int fishSpawned;
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
            spawnPoint = SpawnpointManager.Instance.GetRandomSpawnPoint();
            endpoint = SpawnpointManager.Instance.GetRandomEndPoint(spawnPoint);

            //fishMove.startPoint_T.position = spawnPoint;
            //fishMove.destoryPoint_T.position = endpoint;

            GameObject fish = Instantiate(fishPrefab, spawnPoint, Quaternion.identity,CanvasInstance.Instance.GetMainCanvas().transform);
            Move move = fish.GetComponent<Move>();
            move.MoveFish(spawnPoint, endpoint);

            fishSpawned++;
            fishAlive++;

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
