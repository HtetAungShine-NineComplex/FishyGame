using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

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

    public SpawnPosition spawnPosition;
    public float _curveDistance = 500f;

    protected bool _isDead = false;

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

        if (_startPoint != Vector3.zero && _endPoint != Vector3.zero && _centerPoint == Vector3.zero)
        {

            MoveFish(_startPoint, _endPoint);
        }
        else if(_startPoint != Vector3.zero && _endPoint != Vector3.zero && _centerPoint != Vector3.zero)
        {
            MoveFishInCircleShape(_startPoint, _centerPoint, _endPoint);
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

        StartCoroutine(SwitchFromCircleToStraight(transform.position, endPoint));
    }

    private IEnumerator SwitchFromCircleToStraight(Vector3 startPoint, Vector3 endPoint)
    {
        yield return new WaitForSeconds(desiredDuration);
        
        SetPoints(startPoint, endPoint, 0);
        _centerPoint = Vector3.zero;

        elapsedTime = 0;
        speed = 2f;
    }

    private IEnumerator ChangeSpeedSmoothly(float newDuration)
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
