using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class PatternFishManager : FishManager
{
    [SerializeField] private int _delay ;
    [SerializeField] private int _interval ;
    [SerializeField] private Transform _startPoint; //vertical
    [SerializeField] private Transform _endPoint; //vertical

    private Coroutine _bonusPatternCoroutine;

    public override void SpawnFishFromStart()
    {
        _bonusPatternCoroutine = StartCoroutine(SpawnFishPatternVertical(_delay));
    }

    IEnumerator SpawnFishPatternVertical(int delay)
    {
        yield return new WaitForSeconds(delay);

        while (fishSpawned < maxFish)
        {
            //fishMove.startPoint_T.position = spawnPoint;
            //fishMove.destoryPoint_T.position = endpoint;

            GameObject fish = Instantiate(fishPrefab, _startPoint.position, Quaternion.identity, parentTF);
            Move move = fish.GetComponent<Move>();
            move.SetPoints(_startPoint.position, _endPoint.position, 0); //no curve
            //move.spawnPosition = SpawnpointManager.Instance.GetSpawnPosition();

            //GeneratedFishManager.Instance.AddFish(fish.GetComponent<FishHealth>());

            fishSpawned++;
            yield return new WaitForSeconds(_interval);
        }
    }

    protected override void OnEnterBonusStage(int index)
    {
        _isBonusRound = true;
        SpawnFishFromStart();
    }

    protected override void OnEnterNormalStage(int index)
    {
        _isBonusRound = false;
        if(_bonusPatternCoroutine != null)
        {
            StopCoroutine(_bonusPatternCoroutine);
            _bonusPatternCoroutine = null;
        }
    }
}
