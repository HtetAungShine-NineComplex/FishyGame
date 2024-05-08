using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using System.Net;

public class Move : MonoBehaviour
{
    [SerializeField] private FishSO fishSO;
    protected float speed;
    protected AnimationCurve curve;
    protected float elapsedTime;
    protected float desiredDuration = 7f;
    private Vector3 _startPoint;
    private Vector3 _centerPoint; //only for fishes move in circle shape
    private Vector3 _endPoint;
    protected Vector3 _controlPoint;
    private FishHealth _health;
    private Transform[] _points; //for triangleShape
    private int _pointIndex = 1;

    public SpawnPosition spawnPosition;
    private float _curveDistance = 500f;

    public float CurveDistance
    {
        get { return _curveDistance; }
        set
        {
            _curveDistance = value * Screen.height / 1080f;
        }
    }

    protected bool _isDead = false;

    private void Awake()
    {
        _points = new Transform[0];
        _curveDistance *= Screen.height / 1080f;
    }

    protected virtual void Start()
    {
        speed = fishSO.speed;
        curve = fishSO.SpeedCurve;

        WaveManager.Instance.EnterBonusStage += OnEnterBonusStage;
    }

    protected virtual void Update()
    {
        if(_isDead)
        {
            return;
        }

        if (_startPoint != Vector3.zero && _endPoint != Vector3.zero && _centerPoint == Vector3.zero && _points.Length == 0)
        {
            
            MoveFish(_startPoint, _endPoint);
        }
        else if(_startPoint != Vector3.zero && _endPoint != Vector3.zero && _centerPoint != Vector3.zero)
        {
            MoveFishInCircleShape(_startPoint, _centerPoint, _endPoint);
        }
        else if (_points.Length > 0)
        {
            Debug.Log("Moving in triangle shape" + _curveDistance);
            MoveFishInTriangleShape();
        }
    }

    private void OnDestroy()
    {
        WaveManager.Instance.EnterBonusStage -= OnEnterBonusStage;
    }

    public virtual void OnDead()
    {
        _isDead = true;
        WaveManager.Instance.EnterBonusStage -= OnEnterBonusStage;
    }

    public virtual void SetPoints(Vector3 startPoint_T, Vector3 destroyPoint_T)
    {
        _startPoint = startPoint_T;
        _endPoint = destroyPoint_T;

        _controlPoint = SpawnpointManager.Instance.GetControlPoint(_startPoint, _endPoint, _curveDistance);
    }

    public virtual void SetPoints(Vector3 startPoint_T, Vector3 destroyPoint_T, float curveDistance)
    {
        _startPoint = startPoint_T;
        _endPoint = destroyPoint_T;

        _controlPoint = SpawnpointManager.Instance.GetControlPoint(_startPoint, _endPoint, curveDistance);
    }

    public virtual void SetPointsForCircleShape(Vector3 startPoint_T, Vector3 centerPoint, Vector3 endPoint) //for circular movement
    {
        _startPoint = startPoint_T;
        _centerPoint = centerPoint;
        _endPoint = endPoint;
    }

    public virtual void SetPointsForTriangleShape(Transform[] points)
    {
        desiredDuration = 1.5f;

        _points = points;
        _curveDistance = 0;
        
        _startPoint = _points[0].position;
    }

    public Vector3 GetEndPoint()
    {
        return _endPoint;
    }

    protected virtual void MoveFish(Vector3 startPoint, Vector3 destroyPoint)
    {
        

        if (_health != null && _health._isDead)
        {
            return;
        }

        elapsedTime += Time.deltaTime;
        float completePercent = elapsedTime / desiredDuration;

        // Define the control point
        



        // Calculate the current position along the curve
        Vector2 position = Bezier.GetPoint(startPoint, _controlPoint, destroyPoint, curve.Evaluate(completePercent * speed));
        Vector2 direction = Bezier.GetDerivative(startPoint, _controlPoint, destroyPoint, curve.Evaluate(completePercent * speed));

        transform.position = position;
        transform.up = transform.up = direction.normalized; 

        float distance = Vector3.Distance(transform.position, destroyPoint);

        if (distance <= 0.1f)
        {
            Destroy(this.gameObject);
        }
    }

    protected virtual void MoveFishInCircleShape(Vector3 startPoint, Vector3 centerPoint, Vector3 endPoint)
    {
        if (_health != null && _health._isDead)
        {
            return;
        }

        elapsedTime += Time.deltaTime;
        float completePercent = elapsedTime / (desiredDuration / 2);

        // Calculate the initial angle based on the startPoint and centerPoint
        Vector3 radius = startPoint - centerPoint;
        float initialAngle = Mathf.Atan2(radius.y, radius.x);

        // Calculate the current position along the circle
        float angle = initialAngle + completePercent * Mathf.PI * 2;
        Vector3 position = centerPoint + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius.magnitude;

        transform.position = position;

        Vector3 direction = new Vector3(-Mathf.Sin(angle), Mathf.Cos(angle), 0);
        transform.up = direction;

        StartCoroutine(SwitchTargetPosition(transform.position, endPoint));
    }

    private void MoveFishInTriangleShape()
    {
        if (_health != null && _health._isDead)
        {
            return;
        }

        

        elapsedTime += Time.deltaTime;
        float completePercent = elapsedTime / desiredDuration;

        _controlPoint = SpawnpointManager.Instance.GetControlPoint(_startPoint, _points[_pointIndex].position, _curveDistance);

        Vector2 position = Bezier.GetPoint(_startPoint, _controlPoint, _points[_pointIndex].position, curve.Evaluate(completePercent * speed));
        Vector2 direction = Bezier.GetDerivative(_startPoint, _controlPoint, _points[_pointIndex].position, curve.Evaluate(completePercent * speed));

        transform.position = position;
        transform.up = transform.up = direction.normalized;

        float distance = Vector3.Distance(transform.position, _points[_pointIndex].position);

        if (distance <= 0.01f && elapsedTime > 0)
        {
            _startPoint = transform.position;
            _pointIndex = _pointIndex + 1;
            elapsedTime = 0;

            if(_pointIndex >= _points.Length)
            {
                Destroy(this.gameObject);
            }
        }

    }

    private IEnumerator SwitchTargetPosition(Vector3 startPoint, Vector3 endPoint)
    {
        yield return new WaitForSeconds(desiredDuration);
        
        SetPoints(transform.position, endPoint, 0);
        _centerPoint = Vector3.zero;

        elapsedTime = 0;
        speed = 2f;
    }

    public IEnumerator ChangeSpeedSmoothly(float newDuration)
    {
        while (true)
        {
            desiredDuration = Mathf.Lerp(desiredDuration, newDuration, Time.deltaTime * 1.5f);
            yield return null;
        }
    }

    private void OnEnterBonusStage(int index)
    {
        StartCoroutine(ChangeSpeedSmoothly(1f)); // Change speed smoothly
    }


    /*private void MoveFish(Vector3 startPoint, Vector3 destroyPoint)
    {
        if (_health != null && _health._isDead)
        {
            return;
        }

        elapsedTime += Time.deltaTime;
        float completePercent = elapsedTime / desiredDuration;
        transform.position = Vector2.Lerp(startPoint, destroyPoint, curve.Evaluate(completePercent * speed));
        transform.up = (destroyPoint - transform.position).normalized;

        float distance = Vector3.Distance(transform.position, destroyPoint);

        if (distance <= 0.1f)
        {
            Destroy(this.gameObject);
        }
    }*/

}
