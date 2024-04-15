using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallPhoenixManager : Singleton<SmallPhoenixManager>
{
    [SerializeField] private GameObject smallPhoenixGroup;
    [SerializeField] private Transform parentTransform;

    private float spawnInterval= 5f;
    private int fishInstantDie_count = 0;

    public void SpawnSmallPhoenixGroup()
    {
        StartCoroutine(ISpawnSmallPhoenixGroup());
    }

    public void FishesInstantDieRepeately()
    {
        StartCoroutine(IFishesInstantDieRepeately());
    }

    IEnumerator IFishesInstantDieRepeately()
    {
        while(fishInstantDie_count < 5)
        {
            yield return new WaitForSeconds(2.5f);
            fishesInstantDie();
            fishInstantDie_count++;
        }
    }
    public void fishesInstantDie()
    {
        List<FishHealth> fishList = GeneratedFishManager.Instance.GetGeneratedFishList();

        for (int a = 0; a < fishList.Count; a++)
        {
            fishList[a].InstantDie();
        }
    }

    IEnumerator ISpawnSmallPhoenixGroup()
    {
        GameObject group_1 = Instantiate(smallPhoenixGroup, parentTransform);
        fishesInstantDie();
        yield return new WaitForSeconds(spawnInterval);
        GameObject group_2 = Instantiate(smallPhoenixGroup, parentTransform);
        fishesInstantDie();
        yield return new WaitForSeconds(spawnInterval);
        GameObject group_3 = Instantiate(smallPhoenixGroup, parentTransform);
        fishesInstantDie();
    }

}
