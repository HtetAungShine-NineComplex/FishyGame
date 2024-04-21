using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private Transform _netSpawnPoint;
    [SerializeField] private GameObject _netPrefab;

    private PlayerManager _playerManager;

    protected bool _move = true;

    private int _damageAmount;
    protected int _bounceCount = 0;

    private FishHealth targetFish;
    private bool _toTarget = false;

    protected virtual void Update()
    {
        if(_toTarget && GeneratedFishManager.Instance.HasFish(targetFish))
        { 
            MoveBulletTowardTarget();
        }
        else
        {
            MoveBullet();
        }
    }

    public void SetPlayerManager(PlayerManager playerManager)
    {
        _playerManager = playerManager;
    }

    public void SetDamageAmount(int amount)
    {
        _damageAmount = amount;
    }

    public void SetTargetFish(FishHealth target)
    {
        targetFish = target;
        _toTarget = true;
    }

    void MoveBulletTowardTarget()
    {
        if (_move)
        {
            // Calculate the direction to the target position
            Vector3 directionToTarget = (targetFish.transform.position - transform.position).normalized;

            // Calculate the rotation needed to point the bullet to the target position
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            // Apply the rotation to the bullet's transform
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * Time.deltaTime);
            transform.up = directionToTarget;

            // Move the bullet forward
            float normalizedSpeed = speed * Time.deltaTime * Screen.height / 1080f; // 1080 is the base resolution
            transform.Translate(Vector2.up * normalizedSpeed * 100);
        }
    }

    void MoveBullet()
    {
        if (_move)
        {
            float normalizedSpeed = speed * Time.deltaTime * Screen.height / 1080f; // 1080 is the base resolution
            transform.Translate(Vector2.up * normalizedSpeed * 100);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Fish"))
        {
            if(_toTarget && collision.GetComponent<FishHealth>() != targetFish && GeneratedFishManager.Instance.HasFish(targetFish))
            {
                return;
            }

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
        Instantiate(_netPrefab, _netSpawnPoint.position, _netSpawnPoint.rotation, CanvasInstance.Instance.GetMainCanvas().transform);
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
