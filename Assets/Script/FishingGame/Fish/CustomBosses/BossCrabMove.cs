using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BossCrabMove : Move
{
    [SerializeField] private Transform[] _wayPoints;

    private int _currentWayPointIndex = 1;
    private int _previousWayPointIndex = 0;

    private bool _isClockWise = false;

    protected override void Start()
    {
        base.Start();

        _curveDistance = -100f;

        _controlPoint = SpawnpointManager.Instance.GetControlPoint(
            _wayPoints[_previousWayPointIndex].position, 
            _wayPoints[_currentWayPointIndex].position,
            _curveDistance);

        //_controlPoint = new Vector3(_controlPoint.x, _controlPoint.y, _controlPoint.z);
    }

    protected override void Update()
    {
        CrabMove();
    }

    public void SetWayPoints(Transform[] wayPoints)
    {
        _wayPoints = wayPoints;
    }

    private void CrabMove()
    {
        elapsedTime += Time.deltaTime;
        float completePercent = elapsedTime / desiredDuration;

        // Calculate the current position along the curve
        Vector2 position = Bezier.GetPoint(_wayPoints[_previousWayPointIndex].position, _controlPoint, _wayPoints[_currentWayPointIndex].position, curve.Evaluate(completePercent * speed));
        Vector2 direction = Bezier.GetDerivative(_wayPoints[_previousWayPointIndex].position, _controlPoint, _wayPoints[_currentWayPointIndex].position, curve.Evaluate(completePercent * speed));

        transform.position = position;
        if(_isClockWise)
        {
            transform.up = Vector3.Lerp(transform.up, -direction.normalized, 10f * Time.deltaTime);
        }
        else
        {
            transform.up = Vector3.Lerp(transform.up, direction.normalized, 10f * Time.deltaTime);
        }

        float distance = Vector3.Distance(transform.position, _wayPoints[_currentWayPointIndex].position);

        if (distance <= 0.01f)
        {


            // get random value for _isClockWise
            _isClockWise = (Random.value > 0.5f);


            if(_isClockWise)
            {
                _previousWayPointIndex = _currentWayPointIndex;
                _currentWayPointIndex = _currentWayPointIndex - 1;

                Debug.Log("Reached Distanation");

                if(_currentWayPointIndex < 0)
                {
                    _currentWayPointIndex = _wayPoints.Length - 1;
                }

                _curveDistance = 100;
            }
            else
            {
                Debug.Log("Reached Distanation and turned counter clock");
                _previousWayPointIndex = _currentWayPointIndex;
                _currentWayPointIndex  = (_currentWayPointIndex + 1) % _wayPoints.Length;

                _curveDistance = -100;
            }

            /*if((_currentWayPointIndex == 1 && _previousWayPointIndex == 0) || (_currentWayPointIndex == 0 && _previousWayPointIndex == 1))
            {
                _curveDistance = -100;
            }
            else if((_currentWayPointIndex == 3 && _previousWayPointIndex == 2) || (_currentWayPointIndex == 2 && _previousWayPointIndex == 3))
            {
                _curveDistance = 100;
            }
            else if((_currentWayPointIndex == 2 && _previousWayPointIndex == 1) || (_currentWayPointIndex == 1 && _previousWayPointIndex == 2))
            {
                _curveDistance = 10;
            }
            else if((_currentWayPointIndex == 0 && _previousWayPointIndex == 3) || (_currentWayPointIndex == 3 && _previousWayPointIndex == 0))
            {
                _curveDistance = -10;
            }*/

            _controlPoint = SpawnpointManager.Instance.GetControlPoint(
            _wayPoints[_previousWayPointIndex].position, 
            _wayPoints[_currentWayPointIndex].position,
            _curveDistance);

            elapsedTime = 0;
        }
    }
}
