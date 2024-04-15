using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialFishMove : Move
{
    [SerializeField] private GameObject _coinFX;
    [SerializeField] private Transform[] _coinSpawnPlaces;

    public override void OnDead()
    {
        base.OnDead();
        DeadPhase();
    }

    protected virtual void DeadPhase()
    {
        StartCoroutine(MoveFishToPlayer(CoinManager.Instance.PlayerTransform.position + new Vector3(0f, 300f, 0f)));
    }

    IEnumerator MoveFishToPlayer(Vector3 endPoint)
    {
        Vector3 startPoint = transform.position;

        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPoint, endPoint, time);
            yield return null;
        }

        ShowCoinFX();
    }

    protected void ShowCoinFX()
    {
        for (int i = 0; i < _coinSpawnPlaces.Length; i++)
        {
            Instantiate(_coinFX, _coinSpawnPlaces[i].position, Quaternion.identity, CanvasInstance.Instance.GetMidGroundSpawn());
        }
    }
}
