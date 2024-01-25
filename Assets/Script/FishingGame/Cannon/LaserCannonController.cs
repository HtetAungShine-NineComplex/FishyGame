using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCannonController : MonoBehaviour
{
    [SerializeField] private float _damageRate;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private Camera _camera;

    private int _damageAmount;
    private FishHealth _targetFish;

    private float _damageTimer = 0f;

    private void Start()
    {
        
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
        /*Vector2 ShootPos = new Vector2(((_shootPoint.position.x / Screen.width) * 1920f), (_shootPoint.position.y / Screen.height) * 1080f) ;
        Vector2 TargetPos = new Vector2((targetPosition.x / Screen.width) * 1920, (targetPosition.y / Screen.height) * 1080);

        _lineRenderer.Points.SetValue(ShootPos, 0);
        _lineRenderer.Points.SetValue(TargetPos, 1);


        Debug.Log(targetPosition);

        *//*_lineRenderer.Points.SetValue((Vector2)_shootPoint.position, 0);
        _lineRenderer.Points.SetValue(targetPosition, 1);*//*

        _lineRenderer.SetVerticesDirty();*/
    }
}
