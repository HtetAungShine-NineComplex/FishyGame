using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialFishMove : Move
{
    [SerializeField] private GameObject _coinFX;

    public override void OnDead()
    {
        base.OnDead();
        StartCoroutine(MoveFishToPlayer(CoinManager.Instance.PlayerTransform.position + new Vector3(0f, 500f, 0f)));
    }

    IEnumerator MoveFishToPlayer(Vector3 endPoint)
    {
        Vector3 startPoint = transform.position;

        float time = 0;
        while (time < 3)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPoint, endPoint, time);
            yield return null;
        }

        Instantiate(_coinFX, transform.position, Quaternion.identity, CanvasInstance.Instance.GetMidGroundSpawn());
    }
}
