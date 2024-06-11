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
    public bool _isHorizontal;
    public bool _isVertical;
    public bool _isGoFromMiddle;

    protected int fishSpawned = 0;
    private int fishDestroyed;
    private int fishAlive;
    private Move fishMove;

    Vector3 spawnPoint;
    Vector3 endpoint;

    protected bool _isBonusRound = false;
    private Coroutine _currentCoroutine;

    protected virtual void Start()
    {
        _currentCoroutine = null;

        /*if(!_isMainBoss)
        {
            SpawnFishFromStart();
        }*/

        WaveManager.Instance.EnterBonusStage += OnEnterBonusStage;
        WaveManager.Instance.EnterNormalStage += OnEnterNormalStage;
    }
    private void Awake()
    {
        fishMove = fishPrefab.GetComponent<Move>();
        
    }

    public virtual void SpawnFishFromStart()
    {
        fishSpawned = 0;
        _currentCoroutine = StartCoroutine(SpawnFishCoroutine());
    }

    IEnumerator SpawnFishCoroutine()
    {
        if(_isBonusRound)
        {
            yield break;
        }

        while (fishSpawned < maxFish)
        {
            if(!_isMainBoss)
            {
                yield return new WaitForSeconds(spawnInterval);
            }

            if(_isHorizontal)
            {
                spawnPoint = SpawnpointManager.Instance.GetRandomSpawnPointHorizontal();
            }
            else if (_isVertical)
            {
                spawnPoint = SpawnpointManager.Instance.GetRandomSpawnPointVertical();
            }
            else if(_isGoFromMiddle)
            {
                spawnPoint = SpawnpointManager.Instance.GetScreenMidPointHorizontal();
            }
            else
            {
                spawnPoint = SpawnpointManager.Instance.GetRandomSpawnPoint();
            }

            if(_isGoFromMiddle)
            {
                endpoint = SpawnpointManager.Instance.GetRandomEndPointMid(spawnPoint, SpawnpointManager.Instance.GetSpawnPosition());
            }
            else
            {
                endpoint = SpawnpointManager.Instance.GetRandomEndPoint(spawnPoint, SpawnpointManager.Instance.GetSpawnPosition());
            }
            //fishMove.startPoint_T.position = spawnPoint;
            //fishMove.destoryPoint_T.position = endpoint;

            GameObject fish = Instantiate(fishPrefab, spawnPoint, Quaternion.identity,parentTF);
            Move move = fish.GetComponent<Move>();
            move.SetPoints(spawnPoint, endpoint);
            move.spawnPosition = SpawnpointManager.Instance.GetSpawnPosition();

            //GeneratedFishManager.Instance.AddFish(fish.GetComponent<FishHealth>());

            fishSpawned++;
            fishAlive++;

            if (_isMainBoss)
            {
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }

    protected virtual void OnEnterBonusStage(int index)
    {
        Debug.Log("Enter Bonus Stage and spawning fishes stopped");

        _isBonusRound = true;
        if(_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }
    }

    protected virtual void OnEnterNormalStage(int index)
    {
        Debug.Log("Enter Normal Stage and spawning fishes started");
        _isBonusRound = false;
        if (!_isMainBoss)
        {
            SpawnFishFromStart();
        }
    }
}
