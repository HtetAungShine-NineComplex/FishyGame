using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private Transform[] _netSpawnPoints;
    [SerializeField] private GameObject _netPrefab;

    private PlayerManager _playerManager;

    private int _damageAmount;

    void Update()
    {
        MoveBullet();
    }

    public void SetPlayerManager(PlayerManager playerManager)
    {
        _playerManager = playerManager;
    }

    public void SetDamageAmount(int amount)
    {
        _damageAmount = amount;
    }

    void MoveBullet()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime * 100);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Fish"))
        {
            if(collision.GetComponent<IDamageable>().Damage(_damageAmount))
            {
                

                if(collision.TryGetComponent<Fish>(out Fish fish))
                {
                    OnCaughtFish(fish);
                }
                else
                {
                    OnCaughtFish(collision.GetComponentInChildren<Fish>());
                }
            }

            OnHitFish();

            Destroy(gameObject);
        }
    }

    private void OnHitFish()
    {
        foreach (Transform t in _netSpawnPoints)
        {
            Instantiate(_netPrefab, t.position, Quaternion.identity, CanvasInstance.Instance.GetMainCanvas().transform);
        }
    }

    private void OnCaughtFish(Fish caughtFish)
    {
        //CoinManager.Instance.ShowCoin(transform.position, Random.Range(1, 3));
        if(_playerManager != null)
        {
            _playerManager.AddCoin(caughtFish.Score);
        }
    }
}
