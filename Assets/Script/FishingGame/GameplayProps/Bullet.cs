using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private Transform[] _netSpawnPoints;
    [SerializeField] private GameObject _netPrefab;

    void Update()
    {
        MoveBullet();
    }

    void MoveBullet()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime * 100);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Fish"))
        {
            OnCaughtFish();
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

    private void OnCaughtFish()
    {
        CoinManager.Instance.ShowSilverCoin(transform.position, 5);

        foreach(Transform t in _netSpawnPoints)
        {
            Instantiate(_netPrefab, t.position, Quaternion.identity, CanvasInstance.Instance.GetMainCanvas().transform);
        }
    }
}
