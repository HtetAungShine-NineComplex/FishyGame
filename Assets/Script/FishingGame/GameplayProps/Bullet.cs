using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private Transform[] _netSpawnPoints;
    [SerializeField] private GameObject _netPrefab;
    [SerializeField] protected int _totalBounce = 5; //default value

    private PlayerManager _playerManager;

    protected bool _move = true;

    private int _damageAmount;
    protected int _bounceCount = 0; 

    protected virtual void Update()
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
        if(_move)
        {
            transform.Translate(Vector2.up * speed * Time.deltaTime * 100);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
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

    public virtual void ReflectDirection(bool isTop)
    {
        _bounceCount++;

        if(_bounceCount >= _totalBounce)
        {
            Destroy(gameObject);
            return;
        }

        if(isTop)
        {
            transform.Rotate(180, 0, 0, Space.World);
        }
        else
        {
            transform.Rotate(0, 180, 0, Space.World);
        }
        
    }
}
