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

        if (_startPoint != null && _endPoint != null)
        {

            MoveFish(_startPoint, _endPoint);
        }
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
        transform.up = transform.up = direction.normalized; ;

        float distance = Vector3.Distance(transform.position, destroyPoint);

        if (distance <= 0.1f)
        {
            Destroy(this.gameObject);
        }
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
