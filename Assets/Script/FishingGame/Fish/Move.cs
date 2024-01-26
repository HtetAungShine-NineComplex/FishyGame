using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour
{
    [SerializeField] private FishSO fishSO;
    private float speed;
    private AnimationCurve curve;
    private float elapsedTime;
    private float desiredDuration = 7f;
    private Vector3 _startPoint;
    private Vector3 _endPoint;
    private FishHealth _health;

    private void Start()
    {
        speed = fishSO.speed;
        curve = fishSO.SpeedCurve;

        // Start the coroutine to update the endpoint every 2 seconds
        StartCoroutine(ChangeEndpointPeriodically(2f));
    }

    private void Update()
    {
        if (_startPoint != null && _endPoint != null)
        {
            MoveFish(_startPoint, _endPoint);
        }
    }

    public void SetPoints(Vector3 startPoint_T, Vector3 destroyPoint_T)
    {
        _startPoint = startPoint_T;
        _endPoint = destroyPoint_T;
    }

    public Vector3 GetEndPoint()
    {
        return _endPoint;
    }

    private void MoveFish(Vector3 startPoint, Vector3 destroyPoint)
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
    }

    private IEnumerator ChangeEndpointPeriodically(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            // Update the endpoint periodically
            _endPoint = SpawnpointManager.Instance.GetRandomEndPoint(_startPoint);
        }
    }
}
