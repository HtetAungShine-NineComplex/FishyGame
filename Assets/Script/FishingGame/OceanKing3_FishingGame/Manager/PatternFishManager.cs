using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public enum PatternType
{
    Straight,
    Circle,
    Triangle,
    Static
}

public class PatternFishManager : FishManager
{
    [SerializeField] public float _delay ;
    [SerializeField] private float _interval ;
    [SerializeField] private float _waveInterval ; //0 if not wave
    [SerializeField] private float _waveFishCount ; //0 if not wave
    [SerializeField] private Transform _startPoint; //vertical
    [SerializeField] private Transform _centerPoint; //center
    [SerializeField] private Transform _endPoint; //vertical
    [SerializeField] private PatternType _type;
    [SerializeField] private Transform[] _triangleShapePoints;
    [SerializeField] private bool _stopAfterFirstWave;
    [SerializeField] private float _constanstSpeed;

    private Coroutine _bonusPatternCoroutine;

    public override void SpawnFishFromStart()
    {
        switch (_type)
        {
            case PatternType.Straight:
                _bonusPatternCoroutine = StartCoroutine(SpawnFishPatternVertical(_delay));
                break;
            case PatternType.Circle:
                _bonusPatternCoroutine = StartCoroutine(SpawnFishPatternCircle(_delay));
                break;

            case PatternType.Triangle:
                _bonusPatternCoroutine = StartCoroutine(SpawnFishPatternTriangle(_delay));
                break;

            case PatternType.Static:
                SpawnStatic();
                break;

            default:
                break;
        }

        
    }

    IEnumerator SpawnFishPatternVertical(float delay)
    {
        yield return new WaitForSeconds(delay);

        while (fishSpawned < maxFish)
        {
            //fishMove.startPoint_T.position = spawnPoint;
            //fishMove.destoryPoint_T.position = endpoint;

            GameObject fish = Instantiate(fishPrefab, _startPoint.position, Quaternion.identity, parentTF);
            Move move = fish.GetComponent<Move>();
            move.SetPoints(_startPoint.position, _endPoint.position, 0); //no curve
            move.isPatternFish = true;
            move.speed = _constanstSpeed;
            //move.spawnPosition = SpawnpointManager.Instance.GetSpawnPosition();

            //GeneratedFishManager.Instance.AddFish(fish.GetComponent<FishHealth>());

            fishSpawned++;
            if (_stopAfterFirstWave)
            {
                break;
            }
            else
            {
                yield return new WaitForSeconds(_interval);
            }
        }
    }

    IEnumerator SpawnFishPatternCircle(float delay)
    {
        yield return new WaitForSeconds(delay);

        Debug.Log("Spawning circle");

        while (fishSpawned < maxFish)
        {
            for (int i = 0; i < _waveFishCount; i++)
            {
                GameObject fish = Instantiate(fishPrefab, _startPoint.position, Quaternion.identity, parentTF);
                Move move = fish.GetComponent<Move>();
                move.SetPointsForCircleShape(_startPoint.position, _centerPoint.position, _endPoint.position); //no curve
                fishSpawned++;
                move.isPatternFish = true;
                yield return new WaitForSeconds(_interval);
            }

            if (_stopAfterFirstWave)
            {
                break;
            }
            else
            {
                yield return new WaitForSeconds(_waveInterval);
            }
        }
    }

    IEnumerator SpawnFishPatternTriangle(float delay)
    {
        yield return new WaitForSeconds(delay);

        while (fishSpawned < maxFish)
        {
            for (int i = 0; i < _waveFishCount; i++)
            {
                GameObject fish = Instantiate(fishPrefab, _startPoint.position, Quaternion.identity, parentTF);
                Move move = fish.GetComponent<Move>();
                move.SetPointsForTriangleShape(_triangleShapePoints); //no curve
                fishSpawned++;
                move.isPatternFish = true;
                yield return new WaitForSeconds(_interval);
            }

            yield return new WaitForSeconds(_waveInterval);
        }
    }

    public void SpawnStatic()
    {
        GameObject fish = Instantiate(fishPrefab, _startPoint.position, Quaternion.identity, parentTF);
        Move move = fish.GetComponent<Move>();
        move.MakeSpeedZero();
        move.isPatternFish = true;
    }

    protected override void OnEnterBonusStage(int index)
    {
        _isBonusRound = true;
        fishSpawned = 0;
        SpawnFishFromStart();
    }

    protected override void OnEnterNormalStage(int index)
    {
        _isBonusRound = false;
        if (_bonusPatternCoroutine != null)
        {
            StopCoroutine(_bonusPatternCoroutine);
            _bonusPatternCoroutine = null;
        }
    }
}
