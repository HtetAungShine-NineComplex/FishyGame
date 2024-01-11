using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(CoinToPlayer());
    }

    IEnumerator CoinToPlayer()
    {
        yield return new WaitForSeconds(0.5f);

        float elapsedTime = 0f;
        float duration = 1f;

        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = CoinManager.Instance.PlayerTransform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.position = targetPosition;

        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }

}
