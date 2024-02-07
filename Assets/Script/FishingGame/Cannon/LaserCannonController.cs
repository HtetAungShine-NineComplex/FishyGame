using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCannonController : MonoBehaviour
{
    [SerializeField] private float _damageRate;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private LaserRenderer _laserRenderer;

    private int _damageAmount;
    private FishHealth _targetFish;

    private float _damageTimer = 0f;

    private void OnEnable()
    {
        _laserRenderer.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        _laserRenderer.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_targetFish != null)
        {
            
            _damageTimer += Time.deltaTime;
            if (_damageTimer >= _damageRate)
            {
                _targetFish.Damage(_damageAmount);
                _damageTimer = 0f;
            }
        }
        else
        {
            _targetFish = GeneratedFishManager.Instance.GetRandomFish();
        }
        //_lineRenderer.gameObject.SetActive(_targetFish != null);

        if (_targetFish != null)
        {
            ShootLaser(_targetFish.transform.position);
        }

    }

    private void LateUpdate()
    {
        
    }

    private void ShootLaser(Vector2 targetPosition)
    {
        _laserRenderer.SetLaser(_shootPoint.transform.position, targetPosition);
    }
}
