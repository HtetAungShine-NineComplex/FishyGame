using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCrabManager : FishManager
{
    [SerializeField] private Transform[] _wayPoints;

    private GameObject[] _crabs;

    protected override void Start()
    {
        base.Start();

        WaveManager.Instance.ExitBossStage += ClearCrabs;
        _crabs = new GameObject[2];
    }

    public override void SpawnFishFromStart()
    {
        //base.SpawnFishFromStart();

        StartCoroutine(CrabSpawn());
    }

    IEnumerator CrabSpawn()
    {
        for (int i = 0; i < 2; i++)
        {
            _crabs[i] = Instantiate(fishPrefab, _wayPoints[0].position, Quaternion.identity, parentTF);
            BossCrabMove move = _crabs[i].GetComponent<BossCrabMove>();
            move.SetWayPoints(_wayPoints);

            yield return new WaitForSeconds(1f);
        }
    }

    public void ClearCrabs(int index)
    {
        if(index == 1)
        {
            foreach (GameObject crab in _crabs)
            {
                Destroy(crab);
            }

            _crabs = new GameObject[2];
        }
    }
}
