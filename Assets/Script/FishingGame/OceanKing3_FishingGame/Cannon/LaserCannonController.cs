using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaserCannonController : MonoBehaviour
{
    [SerializeField] private float _damageRate;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private LaserRenderer _laserRenderer;
    [SerializeField] private Image _fishIconShower;

    public event Action LaserShoot;

    private int _damageAmount;
    private FishHealth _targetFish;

    private float _damageTimer = 0f;

    private void OnEnable()
    {
        //_laserRenderer.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        _laserRenderer.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_targetFish != null && Input.GetMouseButton(0))
        {
            
            if (_targetFish.FishIcon != null)
            {
                _fishIconShower.gameObject.SetActive(true);
                _fishIconShower.sprite = _targetFish.FishIcon;
            }
            else
            {
                _fishIconShower.gameObject.SetActive(false);
            }
            
            _laserRenderer.gameObject.SetActive(true);
            _damageTimer += Time.deltaTime;
            if (_damageTimer >= 1/_damageRate)
            {
                if(!GeneratedFishManager.Instance.HasFish(_targetFish))
                {
                    _targetFish = GeneratedFishManager.Instance.GetRandomFish();
                    
                }

                _targetFish.Damage(_damageAmount);
                LaserShoot?.Invoke();
                _damageTimer = 0f;
            }
        }
        else
        {
            _targetFish = GeneratedFishManager.Instance.GetRandomFish();
            _laserRenderer.gameObject.SetActive(false);
        }
        //_lineRenderer.gameObject.SetActive(_targetFish != null);

        if (_targetFish != null)
        {
            ShootLaser(_targetFish.transform.position);
        }
        else
        {
            _laserRenderer.gameObject.SetActive(false);
            _fishIconShower.gameObject.SetActive(false);
        }

    }

    private void LateUpdate()
    {
        
    }

    public void SetDamageAmount(int amount)
    {
        _damageAmount = amount;
    }

    private void ShootLaser(Vector2 targetPosition)
    {
        _laserRenderer.SetLaser(_shootPoint.transform.position, targetPosition);
    }
}