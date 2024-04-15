using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallDragonManager : Singleton<SmallDragonManager>
{
    [SerializeField] private Transform parentTransform;

    [SerializeField] private GameObject smallDragon_Left;
    [SerializeField] private GameObject smallDragon_Right;

    [SerializeField] private GameObject sec_smallDragon_Left;
    [SerializeField] private GameObject sec_smallDragon_Right;

    [SerializeField] private GameObject smallDragon_Top;
    [SerializeField] private GameObject smallDragon_Bot;

    [SerializeField] private GameObject sec_smallDragon_Top;
    [SerializeField] private GameObject sec_smallDragon_Bot;

    public void SpawnSmallDragon_Left()
    {
        GameObject fish = Instantiate(smallDragon_Left, parentTransform);
    }

    public void SpawnSmallDragon_Right()
    {
        GameObject fish = Instantiate(smallDragon_Right, parentTransform);
    }
    public void SpawnSecSmallDragon_Left()
    {
        GameObject fish = Instantiate(sec_smallDragon_Left, parentTransform);
    }

    public void SpawnSecSmallDragon_Right()
    {
        GameObject fish = Instantiate(sec_smallDragon_Right, parentTransform);
    }

    public void SpawnSmallDragon_Top()
    {
        GameObject fish = Instantiate(smallDragon_Top, parentTransform);
    }

    public void SpawnSmallDragon_Bot()
    {
        GameObject fish = Instantiate(sec_smallDragon_Bot, parentTransform);
        Debug.LogWarning(sec_smallDragon_Bot.transform.position);
    }
    public void SpawnSecSmallDragon_Top()
    {
        GameObject fish = Instantiate(sec_smallDragon_Top, parentTransform);
    }

    public void SpawnSecSmallDragon_Bot()
    {
        GameObject fish = Instantiate(sec_smallDragon_Bot, parentTransform);
    }
}
